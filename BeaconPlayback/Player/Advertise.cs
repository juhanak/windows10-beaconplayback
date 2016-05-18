using System;
using System.Threading;
using Windows.Devices.Bluetooth.Advertisement;

namespace BeaconPlayback.Player
{
    /// <summary>
    /// Advertises a beacon event for given amount of time and signals when the advertisement is over. 
    /// </summary>
    class Advertise
    {
        public event EventHandler<bool> Finished;

        private Timer _timer;
        private Event _advertisementEvent;
        private BluetoothLEAdvertisementPublisher _bLEdvertisementPublisher;

        public Advertise(Event advertiseEvent)
        {
            _advertisementEvent = advertiseEvent;
        }

        public bool Start()
        {
            _bLEdvertisementPublisher = new BluetoothLEAdvertisementPublisher();
            BluetoothLEAdvertisementDataSection dataSection = 
                BeaconFactory.BeaconToSecondDataSection(_advertisementEvent.Beacon);
            _bLEdvertisementPublisher.Advertisement.DataSections.Add(dataSection);

            try
            {
                _bLEdvertisementPublisher.Start();
            }
            catch (Exception)
            {
                _bLEdvertisementPublisher = null;
                return false;
            }
            var timespan = TimeSpan.FromSeconds(_advertisementEvent.Duration).TotalMilliseconds;
            _timer = new Timer(advertisementOver, null, (int)timespan, Timeout.Infinite);

            return true;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            if (_bLEdvertisementPublisher != null)
            {
                _bLEdvertisementPublisher.Stop();
                _bLEdvertisementPublisher = null;
            }
        }

        private void advertisementOver(object state)
        {
            _timer.Dispose();

            if (_bLEdvertisementPublisher != null)
            {
                _bLEdvertisementPublisher.Stop();
                _bLEdvertisementPublisher = null;
            }

            var timespan = TimeSpan.FromSeconds(_advertisementEvent.Sleep).TotalMilliseconds;
            _timer = new Timer(readyToAdvertiseNext, null, (int)timespan, Timeout.Infinite);
        }

        private void readyToAdvertiseNext(object state)
        {
            Finished?.Invoke(this, true);
        }
    }
}
