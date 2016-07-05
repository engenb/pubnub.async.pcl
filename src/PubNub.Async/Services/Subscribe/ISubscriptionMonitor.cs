using System.Threading.Tasks;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public interface ISubscriptionMonitor
    {
        Task<SubscribeResponse> Start();
        Task Stop();
    }
}