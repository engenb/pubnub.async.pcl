using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Presence.Models;
using PubNub.Async.Services.Crypto;

namespace PubNub.Async.Presence.Services
{
    public class PresenceService : IPresenceService
	{
		private ICryptoService Crypto { get; }

		private IPubNubEnvironment Environment { get; }
		private Channel Channel { get; }

		public PresenceService(IPubNubClient client, ICryptoService crypto)
		{
			Environment = client.Environment;
			Channel = client.Channel;

			Crypto = crypto;
		}

	    public async Task<TState> GetState<TState>() where TState : class
	    {
		    var response = await Environment.Host
			    .AppendPathSegments("v2", "presence")
			    .AppendPathSegments("sub_key", Environment.SubscribeKey)
			    .AppendPathSegments("channel", Channel.Name)
			    .AppendPathSegments("uuid", Environment.SessionUuid)
			    .GetAsync()
			    .ProcessResponse()
			    .ReceiveJson<StateResponse>();

			//TODO: check for !success and do something about it

		    return response.Payload.ToObject<TState>();
	    }

		public async Task SetState<TState>(TState state) where TState : class
		{
			var payload = JsonConvert.SerializeObject(state);

			if (Channel.Encrypted)
			{
				payload = Crypto.Encrypt(Channel.Cipher ?? Environment.CipherKey, payload);
				payload = JsonConvert.SerializeObject(payload);
			}

			await Environment.Host
				.AppendPathSegments("v2", "presence")
				.AppendPathSegments("sub_key", Environment.SubscribeKey)
				.AppendPathSegments("channel", Channel.Name)
				.AppendPathSegments("uuid", Environment.SessionUuid)
				.AppendPathSegment("data")
				.SetQueryParam("state", payload)
				.GetAsync();
		}
    }
}
