using Flurl;
using Flurl.Http;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Publish;
using PubNub.Async.Services.Publish;
using PubNub.Async.Push.Models;
using System;
using System.Threading.Tasks;

namespace PubNub.Async.Push.Services
{
    public class PushService : IPushService
    {
        private IPubNubEnvironment Environment { get; }
        private Channel Channel { get; }
        private IPublishService Publish { get; }

        public PushService(IPubNubClient client, IPublishService publish)
        {
			Environment = client.Environment;
			Channel = client.Channel;
			Publish = publish;
        }

        public async Task<PushResponse> Register(DeviceType type, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(token));
            }

            var requestUrl = BuildUrl(type, token, "add", Channel.Name);
            var response = new PushResponse();
            var result = await requestUrl.GetAsync()
                .ProcessResponse()
                .ReceiveJsonList();

            if (result == null)
            {
                response.Error = "Error occurred while attempting to register device";
            }

            return response;
        }

        public async Task<PushResponse> Revoke(DeviceType type, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(token));
            }

            var requestUrl = BuildUrl(type, token, "remove", Channel.Name);
            var response = new PushResponse();
            var result = await requestUrl.GetAsync()
                .ProcessResponse()
                .ReceiveJsonList();

            if (result == null)
            {
                response.Error = "Error occurred while attempting to revoke device";
            }

            return response;
        }

        public Task<PublishResponse> PublishPush(string message, bool isDebug = false)
        {
            var payload = new PushPayload
            {
                Apns = new ApnsPayload
                {
                    Aps = new ApsPayload
                    {
                        Alert = message
                    }
                },
                Gcm = new GcmPayload
                {
                    Data = new GcmDataPayload
                    {
                        Message = message
                    }
                },
                IsDebug = isDebug
            };

            return PublishPush(payload);
        }

        public Task<PublishResponse> PublishPush(PushPayload payload)
        {
            if (Channel.Encrypted)
            {
                throw new InvalidOperationException("Push notifications should not be sent using an encrypted channel");
            }

            return Publish.Publish(payload, false);
        }

        private Url BuildUrl(DeviceType type, string token, string action, string channel)
        {
            var pushService = string.Empty;
            switch (type)
            {
                case DeviceType.Android:
                    pushService = "gcm";
                    break;

                case DeviceType.iOS:
                    pushService = "apns";
                    break;

                case DeviceType.Windows:
                    pushService = "mpns";
                    break;
            }

            return Environment.Host
                .AppendPathSegments("v1", "push")
                .AppendPathSegments("sub-key", Environment.SubscribeKey)
                .AppendPathSegments("devices", token)
                .SetQueryParam("type", pushService)
                .SetQueryParam("action", channel);
        }
    }
}