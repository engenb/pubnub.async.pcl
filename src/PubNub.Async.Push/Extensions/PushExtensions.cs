using PubNub.Async.Models.Publish;
using PubNub.Async.Push.Models;
using PubNub.Async.Push.Services;
using System.Threading.Tasks;

namespace PubNub.Async.Push.Extensions
{
    public static class PushExtensions
    {
        public static async Task<PushResponse> RegisterDeviceForPush(
            this IPubNubClient client,
            DeviceType type,
            string token)
        {
            return await PubNub.Environment
                .Resolve<IPushService>(client)
                .Register(type, token);
        }

        public static async Task<PushResponse> RevokeDeviceForPush(
            this IPubNubClient client,
            DeviceType type,
            string token)
        {
            return await PubNub.Environment
                .Resolve<IPushService>(client)
                .Revoke(type, token);
        }

        public static async Task<PublishResponse> PublishPush(
            this IPubNubClient client,
            string message)
        {
            return await PubNub.Environment
                .Resolve<IPushService>(client)
                .PublishPush(message);
        }

        public static async Task<PublishResponse> PublishPush(
            this IPubNubClient client,
            PushPayload payload)
        {
            return await PubNub.Environment
                .Resolve<IPushService>(client)
                .PublishPush(payload);
        }
    }
}