using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace BeaconPlayback.Player
{
    /// <summary>
    /// Creates an event source from provided source file.
    /// </summary>
    public class SourceFactory
    {
        public static async Task<EventSource> InitializeSourceAsync(StorageFile file)
        {
            var text = await Windows.Storage.FileIO.ReadTextAsync(file);
            JObject settingsObject = JObject.Parse(text);

            var mode = settingsObject["settings"]["mode"].Value<String>();
            
            if (mode == "local")
            {
                EventSource localSrc = new EventSource();
                localSrc.AddEvents(settingsObject);
                return localSrc;
            }
            else if(mode == "remote")
            {
                var url = settingsObject["settings"]["url"].Value<String>();
                RemoteEventSource remotelSrc = new RemoteEventSource(url);
                return remotelSrc;
            }
            return null;
        }
    }
}
