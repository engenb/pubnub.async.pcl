using Newtonsoft.Json;

namespace PubNub.Push.Async.Models
{
    public class GcmPayload
    {
        [JsonProperty(PropertyName = "data")]
        public GcmDataPayload Data { get; set; }
    }
}