using System;

namespace BeaconPlayback.Player
{
    class EventPlayer
    {
        public enum State { Uninitialized, Initialized, Playing };
        public event EventHandler<string> Message;
        public event EventHandler<State> StateChanged;

        private Advertise _advertise;
        private EventSource _source;
        private State _state = State.Uninitialized;
        private bool _repeat = false;
        private uint _repeatRound = 0;
        private uint _totalNumberofEvents = 0;

        public bool Initialize(EventSource source)
        {
            if(source.Validate())
            {
                _source = source;
                AdvertiserState = State.Initialized;
                return true;
            }
            return false;
        }

        public State AdvertiserState
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                StateChanged?.Invoke(this, _state);
            }
        }

        public async void StartAsync(bool repeat)
        {
            _repeat = repeat;
            _repeatRound = 1;
            _totalNumberofEvents = 0;
            if (await _source.StartAsync())
            {
                var currentEvent = _source.GetCurrentEvent();
                if (currentEvent != null)
                {
                    StartToAdvertise(currentEvent);
                    AdvertiserState = State.Playing;
                    return;
                }
            }
            Message?.Invoke(this, "Failed to start. ");
        }

        public void Stop()
        {
            if (AdvertiserState == State.Playing)
            {
                Message?.Invoke(this, "Finished. Total number of events: " + _totalNumberofEvents);
            }

            AdvertiserState = State.Initialized;
            _advertise?.Stop();
        }

        private void StartToAdvertise(Event ev)
        {
            _advertise = new Advertise(ev);
            _advertise.Finished += OnAdvertisingFinished;
            _advertise.Start();

            string msg = "Event Number: " + (_totalNumberofEvents+1) + "\n" +
                ev.Beacon.Id1 + " " + ev.Beacon.Id2 + " " + ev.Beacon.Id3 +
                "\nDuration: " + ev.Duration + ", Sleep: " + ev.Sleep;

            Message?.Invoke(this, msg);
            _totalNumberofEvents++;
        }

        private async void OnAdvertisingFinished(object sender, bool e)
        {
            Event eventToAdvertise = null;
            var result = _source.MoveToNextEvent();

            if (result == MoveNextResult.Ok)
            {
                eventToAdvertise = _source.GetCurrentEvent();
            }
            else if(result == MoveNextResult.NewReplyRound)
            {
                eventToAdvertise = _source.GetCurrentEvent();
            }
            else if(result == MoveNextResult.Finished)
            {
                if(_repeat)
                {
                    _repeatRound++;
                    Message?.Invoke(this, "Repeat mode is on. Round number: " + _repeatRound);
                    await _source.StartAsync();
                    eventToAdvertise = _source.GetCurrentEvent();
                }
                else
                {
                    Message?.Invoke(this, "Finished. Total number of events: " + _totalNumberofEvents);
                    AdvertiserState = State.Initialized;
                }
            }

            if(eventToAdvertise != null)
            {
                StartToAdvertise(eventToAdvertise);
            }
        }

    }
}
