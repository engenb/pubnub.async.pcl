using System.Threading.Tasks;
using PubNub.Async.Models.Channel;
using PubNub.Async.Models.Subscribe;
using PubNub.Async.Services.Subscribe;

namespace PubNub.Async.Extensions
{
    public static class SubscribeExtensions
    {
        public static Task<SubscribeResponse> Subscribe(
            this string channel,
            MessageReceivedHandler handler)
        {
            return new PubNubClient(channel)
                .Subscribe(handler);
        }
        public static Task<SubscribeResponse> Subscribe(
            this Channel channel,
            MessageReceivedHandler handler)
        {
            return new PubNubClient(channel)
                .Subscribe(handler);
        }

        public static async Task<SubscribeResponse> Subscribe(
               this IPubNubClient client,
               MessageReceivedHandler handler)
        {
            return await PubNub.Environment
                .Resolve<ISubscribeService>(client)
                .Subscribe(handler)
                .ConfigureAwait(false);
        }

        public static async Task Unsubscribe(
            this string channel)
        {
            await new PubNubClient(channel)
                .Unsubscribe();
        }

        public static async Task Unsubscribe(
            this Channel channel)
        {
            await new PubNubClient(channel)
                .Unsubscribe();
        }

        public static async Task Unsubscribe(
               this IPubNubClient client)
        {
            await PubNub.Environment
                .Resolve<ISubscribeService>(client)
                .Unsubscribe()
                .ConfigureAwait(false);
        }
    }
}
