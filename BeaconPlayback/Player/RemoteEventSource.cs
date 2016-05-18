using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeaconPlayback.Player
{
    /// <summary>
    /// Downloads the list of beacon events from the HTTP server.
    /// </summary>
    class RemoteEventSource : EventSource
    {
        private Uri _url;

        public RemoteEventSource(string url)
        {
            _url = new Uri(url);
        }

        public override bool Validate()
        {
            return true;
        }

        public override async Task<bool> StartAsync()
        {
            var httpClient = new HttpClient();

            try
            {
                var result = await httpClient.GetStringAsync(_url);
                JObject settingsObject = JObject.Parse(result);
                base.AddEvents(settingsObject);
                return await base.StartAsync();
            }
            catch
            {
                // Details in ex.Message and ex.HResult.
            }
            return false;
        }
    }
}
