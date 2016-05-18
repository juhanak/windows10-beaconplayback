using System;

namespace BeaconPlayback.Player
{
    /// <summary>
    /// Contains data for single beacon event.
    /// </summary>
    public class Event
    {
        public UInt32 Duration
        {
            get;
            set;
        }

        public UInt32 Sleep
        {
            get;
            set;
        }

        public Beacon Beacon
        {
            get;
            set;
        }
    }
}
