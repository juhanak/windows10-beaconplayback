using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaconPlayback.Player
{
    public enum MoveNextResult { Ok, NewReplyRound, Finished };
    
    /// <summary>
    /// Maintains a list of beacon events and the position of currently selected event. 
    /// </summary>
    public class EventSource
    {

        protected const UInt16 ManufacturerId = 0x004c;
        protected const UInt16 BeaconCode = 0x0215;

        protected List<Event> _events = new List<Event>();
        protected int _nextToAdvertise = 0;
        protected uint _replaysConsumed = 0;

        public uint EventCount
        {
            get
            {
                return (uint)_events.Count;
            }
        }

        public uint ReplayCount
        {
            get; set;
        }

        public void AddEvents(JObject settingsObject)
        {
            _events.Clear();
            ReplayCount = settingsObject["settings"]["repeat"].Value<uint>();
            foreach (JObject ev in settingsObject["events"])
            {
                var id1 = ev["id1"].Value<String>();
                var id2 = ev["id2"].Value<String>();
                var id3 = ev["id3"].Value<String>();
                var duration = ev["duration"].Value<uint>();
                var sleep = ev["sleep"].Value<uint>();
                AddEvent(id1.Trim(), id2.Trim(), id3.Trim(), duration, sleep);
            }
        }

        public virtual bool Validate()
        {
            return (EventCount > 0) && (ReplayCount > 0) ? true : false;
        }

        public virtual MoveNextResult MoveToNextEvent()
        { 
            if (_nextToAdvertise + 1 < EventCount)
            {
                _nextToAdvertise = _nextToAdvertise + 1;
                return MoveNextResult.Ok;
            }
            else
            {
                _nextToAdvertise = 0;

                if (_replaysConsumed + 1 >= ReplayCount)
                {
                    return MoveNextResult.Finished;
                }
                else
                {
                    _replaysConsumed++;
                    return MoveNextResult.NewReplyRound;
                }
            }
        }

        public virtual Event GetCurrentEvent()
        {
            return _events[_nextToAdvertise];
        }

        public virtual async Task<bool> StartAsync()
        {
            _nextToAdvertise = 0;
            _replaysConsumed = 0;
            return true;
        }

        /// <summary>
        /// Adds event to the list that will be advertised.
        /// </summary>
        /// <param name="ID1">The beacon ID1</param>
        /// <param name="ID2">The beacon ID2</param>
        /// <param name="ID3">The beacon ID3</param>
        /// <param name="duration">Duration in seconds how long time the beacon is avertised</param>
        /// <param name="sleep">Duration in seconds how long time we wait until the next beacon event</param>
        /// <returns>True if event is successfully added</returns>
        public bool AddEvent(string ID1, string ID2, string ID3, UInt32 duration, UInt32 sleep)
        {
            Beacon beacon = new Beacon();
            beacon.ManufacturerId = ManufacturerId;
            beacon.Code = BeaconCode;
            beacon.Id1 = ID1;

            try
            {
                beacon.Id2 = UInt16.Parse(ID2);
                beacon.Id3 = UInt16.Parse(ID3);
            }
            catch (Exception)
            {
                return false;
            }

            beacon.MeasuredPower = -58;
            Event e = new Event() { Duration = duration, Beacon = beacon, Sleep = sleep };
            _events.Add(e);

            return true;
        }
    }
}
