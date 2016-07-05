using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PubNub.Async.Models.Subscribe
{
    public class PubNubSubscribeResponseMessage
    {
        //"a": "3"
        //"f": 0

        [JsonProperty("i")]
        public string SessionUuid { get; set; }
        [JsonProperty("p")]
        public PubNubSubscribeResponseTime Processed { get; set; }
        [JsonProperty("k")]
        public string SubscribeKey { get; set; }
        [JsonProperty("c")]
        public string Channel { get; set; }
        [JsonProperty("d")]
        public JToken Data { get; set; }

        //"b": "test2"
    }
}
