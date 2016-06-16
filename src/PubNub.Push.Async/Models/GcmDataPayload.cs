using Newtonsoft.Json;

namespace PubNub.Push.Async.Models
{
    public class GcmDataPayload
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}