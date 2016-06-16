using PubNub.Push.Async.Models;
using System.Threading.Tasks;

namespace PubNub.Push.Async.Services
{
    public interface IPushService
    {
        Task<PushResponse> Register(DeviceType type, string token);

        Task<PushResponse> Revoke(DeviceType type, string token);
    }
}