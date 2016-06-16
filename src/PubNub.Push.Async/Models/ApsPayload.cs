using Newtonsoft.Json;

namespace PubNub.Push.Async.Models
{
    public class ApsPayload
    {
        [JsonProperty(PropertyName = "alert")]
        public string Alert { get; set; }
    }
}