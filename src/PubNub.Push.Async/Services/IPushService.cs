using PubNub.Async.Models.Publish;
using PubNub.Push.Async.Models;
using System.Threading.Tasks;

namespace PubNub.Push.Async.Services
{
    public interface IPushService
    {
        Task<PushResponse> Register(DeviceType type, string token);

        Task<PushResponse> Revoke(DeviceType type, string token);

        Task<PublishResponse> PublishPush(string message, bool isDebug = false);

        Task<PublishResponse> PublishPush(PushPayload payload);
    }
}