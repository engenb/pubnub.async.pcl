using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using PubNub.Async.Extensions.HttpResponseMessage;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.History;

namespace PubNub.Async.Extensions
{
	public static class HistoryExtensions
	{
		public static async Task<HistoryResponse<TContent>> History<TContent>(this PubNub pubNub, Channel channel, long? start = null, long? end = null, int? count = null, bool reverse = false, bool includeTime = true)
		{
			var requestUrl = pubNub.PrepareUrl()
				.AppendPathSegments("v2", "history")
				.AppendPathSegments("sub-key", pubNub.SubscribeKey)
				.AppendPathSegments("channel", channel.Name)
				.SetQueryParams(new
				{
					count = (count ?? -1) < 0 ? 100 : count,
					pnsdk = pubNub.SdkVersion,
					uuid = pubNub.SessionUuid
				});

			if (includeTime)
			{
				requestUrl.SetQueryParam("include_token", includeTime);
			}
			if (reverse)
			{
				requestUrl.SetQueryParam("reverse", reverse);
			}
			if (start.HasValue && start > -1)
			{
				requestUrl.SetQueryParam("start", start);
			}
			if (end.HasValue && end > -1)
			{
				requestUrl.SetQueryParam("end", end);
			}
			if (!string.IsNullOrWhiteSpace(pubNub.AuthenticationKey))
			{
				requestUrl.SetQueryParam("auth", pubNub.AuthenticationKey);
			}
			var rawResponse = await requestUrl.GetAsync()
				.ProcessResponse()
				.ReceiveString();

			return Deserialize<TContent>(channel, rawResponse, pubNub.CipherKey);
		}

		private static HistoryResponse<TContent> Deserialize<TContent>(Channel channel, string rawResponse, string cipherKey)
		{
			var array = JArray.Parse(rawResponse);
			if (!array.HasValues || array.Count != 3)
			{
				return null;
			}

			var messages = array[0];
			var start = array[1].Value<long>();
			var end = array[2].Value<long>();

			if (start == 0 && end == 0 && messages.Count() == 1)
			{
				// we probably have an error
				return new HistoryResponse<TContent>
				{
					Error = messages[0].Value<string>()
				};
			}

		    return new HistoryResponse<TContent>
		    {
		        Start = start,
		        End = end,
		        Messages = messages.Children()
		            .Select(x => channel.Encrypted
		                ? Decrypt<TContent>(channel.Cipher ?? cipherKey, x)
		                : x.ToObject<HistoryMessage<TContent>>())
		    };
		}

	    private static HistoryMessage<TContent> Decrypt<TContent>(string cipherKey, JToken token)
	    {
	        var encryptedMsg = token.ToObject<HistoryMessage<string>>();
	        var encryptedContent = encryptedMsg.Content;

	        var decryptedContent = Crypto.Decrypt(cipherKey, encryptedContent);

	        return new HistoryMessage<TContent>
	        {
	            Sent = encryptedMsg.Sent,
	            Content = JsonConvert.DeserializeObject<TContent>(decryptedContent)
	        };
	    }
	}
}
