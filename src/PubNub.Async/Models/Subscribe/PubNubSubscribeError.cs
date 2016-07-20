using System.Net;
using Newtonsoft.Json;

namespace PubNub.Async.Models.Subscribe
{
    public class PubNubSubscribeError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("payload")]
        public PubNubSubscribeErrorPayload Payload { get; set; }
        [JsonProperty("error")]
        public bool Error { get; set; }
        [JsonProperty("service")]
        public string Service { get; set; }
        [JsonProperty("status")]
        public HttpStatusCode Status { get; set; }
    }
}
