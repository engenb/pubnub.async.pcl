using PubNub.Async;
using PubNub.Push.Async.Models;
using PubNub.Push.Async.Services;
using System.Threading.Tasks;

namespace PubNub.Push.Async.Extensions
{
    public static class PushExtensions
    {
        public static async Task<PushResponse> RegisterDeviceForPush(
            this IPubNubClient client,
            DeviceType type,
            string token)
        {
            return await PubNub.Async.PubNub.Environment
                .Resolve<IPushService>(client)
                .Register(type, token);
        }

        public static async Task<PushResponse> RevokeDeviceForPush(
            this IPubNubClient client,
            DeviceType type,
            string token)
        {
            return await PubNub.Async.PubNub.Environment
                .Resolve<IPushService>(client)
                .Revoke(type, token);
        }
    }
}