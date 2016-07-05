using System.Linq;
using Moq;
using Ploeh.AutoFixture;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Services.Subscribe;
using PubNub.Async.Tests.Common;
using Xunit;

namespace PubNub.Async.Tests.Services.Subscribe
{
    public class SubscriptionRegistryTests : AbstractTest
    {
        [Fact]
        public void Register__Given_EnvironmentAndChannel__When_EmptyRegistry__Then_AddSubscription()
        {
            var subscribeKey = Fixture.Create<string>();
            var channelName = Fixture.Create<string>();
            var sessionUuid = Fixture.Create<string>();

            MessageReceivedHandler handler = args => { };

            var mockEnvironment = new Mock<IPubNubEnvironment>();
            mockEnvironment
                .SetupGet(x => x.SubscribeKey)
                .Returns(subscribeKey);
            mockEnvironment
                .Setup(x => x.SessionUuid)
                .Returns(sessionUuid);
            mockEnvironment
                .Setup(x => x.Clone())
                .Returns(mockEnvironment.Object);

            var channel = new Channel(channelName);

            var subject = new SubscriptionRegistry();

            var handlers = subject.MessageHandlers(mockEnvironment.Object, channel);

            Assert.Empty(handlers);

            subject.Register(mockEnvironment.Object, channel, handler);

            handlers = subject.MessageHandlers(mockEnvironment.Object, channel);

            Assert.NotEmpty(handlers);
            Assert.Equal(1, handlers.Count());
            Assert.Same(handler, handlers.FirstOrDefault());
        }

        [Fact]
        public void Register__Given_EnvironmentAndChannel__When_PreRegistered__Then_AddHandler()
        {
            var subscribeKey = Fixture.Create<string>();
            var channelName = Fixture.Create<string>();
            var sessionUuid = Fixture.Create<string>();

            MessageReceivedHandler handler1 = args => { };
            MessageReceivedHandler handler2 = args => { };

            var mockEnvironment = new Mock<IPubNubEnvironment>();
            mockEnvironment
                .SetupGet(x => x.SubscribeKey)
                .Returns(subscribeKey);
            mockEnvironment
                .Setup(x => x.SessionUuid)
                .Returns(sessionUuid);
            mockEnvironment
                .Setup(x => x.Clone())
                .Returns(mockEnvironment.Object);

            var channel = new Channel(channelName);

            var subject = new SubscriptionRegistry();

            var handlers = subject.MessageHandlers(mockEnvironment.Object, channel);

            Assert.Empty(handlers);

            subject.Register(mockEnvironment.Object, channel, handler1);

            handlers = subject.MessageHandlers(mockEnvironment.Object, channel);

            Assert.NotEmpty(handlers);
            Assert.Equal(1, handlers.Count());
            Assert.Same(handler1, handlers.FirstOrDefault(x => x == handler1));

            subject.Register(mockEnvironment.Object, channel, handler2);

            handlers = subject.MessageHandlers(mockEnvironment.Object, channel);

            Assert.NotEmpty(handlers);
            Assert.Equal(2, handlers.Count());
            Assert.Same(handler2, handlers.FirstOrDefault(x => x == handler2));
        }
    }
}
