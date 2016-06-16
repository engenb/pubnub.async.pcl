using Flurl;
using Flurl.Http;
using PubNub.Async;
using PubNub.Async.Configuration;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Publish;
using PubNub.Async.Services.Publish;
using PubNub.Push.Async.Models;
using System;
using System.Threading.Tasks;

namespace PubNub.Push.Async.Services
{
    public class PushService : IPushService
    {
        private readonly IPubNubEnvironment _environment;
        private readonly Channel _channel;
        private readonly IPublishService _publish;

        public PushService(IPubNubClient client, IPublishService publish)
        {
            _environment = client.Environment;
            _channel = client.Channel;
            _publish = publish;
        }

        public async Task<PushResponse> Register(DeviceType type, string token)
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(token));
            }

            var requestUrl = BuildUrl(type, token, "add", _channel.Name);
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
            if (String.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(token));
            }

            var requestUrl = BuildUrl(type, token, "remove", _channel.Name);
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

        public Task<PublishResponse> PublishPushNotification(string message, bool isDebug = false)
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

            return PublishPushNotification(payload);
        }

        public Task<PublishResponse> PublishPushNotification(PushPayload payload)
        {
            if (_channel.Encrypted)
            {
                throw new InvalidOperationException("Push notifications should not be sent using an encrypted channel");
            }

            return _publish.Publish(payload, false);
        }

        private Url BuildUrl(DeviceType type, string token, string action, string channel)
        {
            var pushService = String.Empty;
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

            return _environment.Host
                .AppendPathSegments("v1", "push")
                .AppendPathSegments("sub-key", _environment.SubscribeKey)
                .AppendPathSegments("devices", token)
                .SetQueryParam("type", pushService)
                .SetQueryParam("action", channel);
        }
    }
}