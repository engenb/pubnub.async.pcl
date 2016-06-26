using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PubNub.Async.Presence.Models
{
	public class StateResponse
	{
		[JsonProperty("status")]
		public HttpStatusCode Status { get; set; }

		[JsonProperty("uuid")]
		public string Uuid { get; set; }

		[JsonProperty("service")]
		public string Service { get; set; }

		[JsonProperty("channel")]
		public string Channel { get; set; }

		[JsonProperty("payload")]
		public JToken Payload { get; set; }
	}
}
