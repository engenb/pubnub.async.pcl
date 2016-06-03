using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.History;
using PubNub.Async.Services.Crypto;

namespace PubNub.Async.Services.History
{
    public class HistoryService : IHistoryService
    {
        private ICryptoService Crypto { get; }

		private IPubNubSettings Settings { get; }
		private Channel Channel { get; }

        public HistoryService(PubNubClient client, ICryptoService crypto)
        {
            Crypto = crypto;
	        Settings = client.Settings;
	        Channel = client.Channel;
        }

        public async Task<HistoryResponse<TContent>> History<TContent>(
            long? start = null,
            long? end = null,
            int? count = null,
            bool reverse = false,
            bool includeTime = true)
        {
            var requestUrl = Settings.Host
                .AppendPathSegments("v2", "history")
                .AppendPathSegments("sub-key", Settings.SubscribeKey)
                .AppendPathSegments("channel", Channel.Name)
                .SetQueryParams(new
                {
                    count = (count ?? -1) < 0 ? 100 : count,
                    pnsdk = Settings.SdkVersion,
                    uuid = Settings.SessionUuid
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
            if (!string.IsNullOrWhiteSpace(Settings.AuthenticationKey))
            {
                requestUrl.SetQueryParam("auth", Settings.AuthenticationKey);
            }
            var rawResponse = await requestUrl.GetAsync()
                .ProcessResponse()
                .ReceiveString();

            return Deserialize<TContent>(Channel, rawResponse, Settings.CipherKey);
        }

        private HistoryResponse<TContent> Deserialize<TContent>(Channel channel, string rawResponse, string cipherKey)
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

        private HistoryMessage<TContent> Decrypt<TContent>(string cipherKey, JToken token)
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
