using Newtonsoft.Json;

namespace PubNub.Async.Models.Subscribe
{
    public class PubNubSubscribeErrorPayload
    {
        [JsonProperty("channels")]
        public string[] Channels { get; set; }
    }
}
