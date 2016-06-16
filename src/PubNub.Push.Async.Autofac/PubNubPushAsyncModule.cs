using Autofac;
using PubNub.Async;
using PubNub.Async.Services.Publish;
using PubNub.Push.Async.Services;
using System;

namespace PubNub.Push.Async.Autofac
{
    public class PubNubPushAsyncModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register<IPushService>((c, p) =>
                {
                    var client = p.TypedAs<IPubNubClient>();
                    if (client == null)
                    {
                        throw new InvalidOperationException($"{typeof(IPubNubClient).Name} is required to resolve ${typeof(IPushService).Name}");
                    }

                    var context = c.Resolve<IComponentContext>();
                    var publishFn = context.Resolve<Func<IPubNubClient, IPublishService>>();
                    return new PushService(client, publishFn(client));
                });
        }
    }
}