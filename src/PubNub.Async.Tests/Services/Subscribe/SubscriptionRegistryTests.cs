using System.Linq;
using Moq;
using PubNub.Async.Tests.Common;
using Xunit;

namespace PubNub.Async.Tests.Services.Subscribe
{
    public class SubscriptionRegistryTests : AbstractTest
    {
        [Fact]
        public void Register__Given_EnvironmentAndChannel__When_EmptyRegistry__Then_AddSubscription()
        {
        }

        [Fact]
        public void Register__Given_EnvironmentAndChannel__When_PreRegistered__Then_AddHandler()
        {
        }
    }
}
