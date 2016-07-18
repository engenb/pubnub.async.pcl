using System.Threading.Tasks;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public interface ISubscriptionMonitor
    {
        long? SubscribeTimeToken { get; set; }
        Task<SubscribeResponse> Start();
        Task Stop();
    }
}