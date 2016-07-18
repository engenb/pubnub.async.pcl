using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubNub.Async.Configuration;
using PubNub.Async.Models;
using PubNub.Async.Models.Subscribe;

namespace PubNub.Async.Services.Subscribe
{
    public interface IResolveSubscription
    {
        Subscription<TMessage> Resolve<TMessage>(IPubNubEnvironment environment, Channel channel);
    }
}
