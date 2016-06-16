using Newtonsoft.Json;

namespace PubNub.Push.Async.Models
{
    public class ApnsPayload
    {
        [JsonProperty(PropertyName = "aps")]
        public ApsPayload Aps { get; set; }
    }
}