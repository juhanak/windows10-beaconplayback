using BeaconPlayback.Player;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BeaconPlayback
{
    /// <summary>
    /// The main page of the application.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private EventPlayer _player = null;
        private MainPageModel _model = new MainPageModel();

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = _model;
        }

        /// <summary>
        /// Reads JSON file, and processes it and creates a player and 
        /// event source object from the events.
        /// </summary>
        private async void OnOpen(object sender, RoutedEventArgs e)
        {

            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".JSON");
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                _player?.Stop();
                _player = null;

                EventSource src = await SourceFactory.InitializeSourceAsync(file);
                if (src != null)
                {
                    _player = new EventPlayer();
                    _player.Message += OnMessage;
                    _player.StateChanged += OnAdvertiserStateChanged;
                    if (_player.Initialize(src))
                    {
                        _model.AddLogEntry("File was loaded successfully.");
                        return;
                    }
                }
                if (_player == null)
                {
                    _model.AddLogEntry("Failed to process the file.");
                }
            }
 
        }

        private async void OnAdvertiserStateChanged(object sender, EventPlayer.State e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (e == EventPlayer.State.Initialized)
                {
                    _model.RepeatToggleEnabled = true;
                    _model.StartStopEnabled = true;
                    _model.FileOpenEnabled = true;
                    StarStop.Label = "Start";
                    StarStop.Icon = new SymbolIcon(Symbol.Play);
                }
                else if (e == EventPlayer.State.Playing)
                {
                    _model.RepeatToggleEnabled = false;
                    _model.FileOpenEnabled = false;
                    StarStop.Label = "Stop";
                    StarStop.Icon = new SymbolIcon(Symbol.Stop);
                }
            });
        }

        private void OnMessage(object sender, string e)
        {
            _model.AddLogEntry(e);
        }

        private void OnStartStop(object sender, RoutedEventArgs e)
        {
            if(_player.AdvertiserState == EventPlayer.State.Initialized)
            {
                _player.StartAsync(_model.Repeat);
            }
            else if (_player.AdvertiserState == EventPlayer.State.Playing)
            {
                _player.Stop();
            }
        }

    }
}
