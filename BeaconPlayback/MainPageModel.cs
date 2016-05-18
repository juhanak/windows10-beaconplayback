using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;

namespace BeaconPlayback
{
    class MainPageModel : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private members
        ObservableCollection<LogEntryItem> _logEntries;
        bool _repeat = false;
        bool _startStopEnabled = false;
        bool _repeatToggleEnabled = true;
        bool _fileOpenEnabled = true;
        #endregion

        #region Properties
        /// <summary>
        /// True if the PlayPause button is enabled, false otherwise
        /// </summary>
        public bool StartStopEnabled
        {
            get
            {
                return _startStopEnabled;
            }
            set
            {
                _startStopEnabled = value;
                NotifyPropertyChanged("StartStopEnabled");
            }
        }

        /// <summary>
        /// True if the Repeat toggle button is enabled, false otherwise
        /// </summary>
        public bool RepeatToggleEnabled
        {
            get
            {
                return _repeatToggleEnabled;
            }
            set
            {
                _repeatToggleEnabled = value;
                NotifyPropertyChanged("RepeatToggleEnabled");
            }
        }

        /// <summary>
        /// True if the FileOpen button is enabled, false otherwise
        /// </summary>
        public bool FileOpenEnabled
        {
            get
            {
                return _fileOpenEnabled;
            }
            set
            {
                _fileOpenEnabled = value;
                NotifyPropertyChanged("FileOpenEnabled");
            }
        }

        /// <summary>
        /// True if repeat is toggled, false otherwise
        /// </summary>
        public bool Repeat
        {
            get
            {
                return _repeat;
            }
            set
            {
                _repeat = value;
                NotifyPropertyChanged("Repeat");
            }
        }
        #endregion

        public MainPageModel()
        {
            LogEntryItemCollection = new ObservableCollection<LogEntryItem>();
        }

        /// <summary>
        /// Collection of the log entries
        /// </summary>
        public ObservableCollection<LogEntryItem> LogEntryItemCollection
        {
            get
            {
                return _logEntries;
            }
            set
            {
                _logEntries = value;
                NotifyPropertyChanged("LogEntryItemCollection");
            }
        }

        /// <summary>
        /// Adds new log entry into the LogEntryItemCollection. Maximum number of items 
        /// on the list is 20.
        /// </summary>
        public async void AddLogEntry(string message)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    // Removes the last item from the log entries list
                    if (LogEntryItemCollection.Count > 20)
                    {
                        LogEntryItemCollection.RemoveAt(LogEntryItemCollection.Count - 1);
                    }
                    LogEntryItem logEntryItem = new LogEntryItem(message);
                    LogEntryItemCollection.Insert(0, logEntryItem);
                });
        }


        /// <summary>
        /// Executes when the property has changed
        /// </summary>
        /// <param name="propertyName">Property which will be changed</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LogEntryItem
    {
        public string Timestamp
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            set;
        }

        public LogEntryItem(string message)
        {
            Timestamp = string.Format("{0:H:mm:ss}", DateTime.Now);
            Message = message;
        }
    }
}
