using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using PubNub.Async.Extensions;
using PubNub.Async.Presence.Services;
using PubNub.Async.Services.Crypto;
using PubNub.Async.Tests.Common;
using PubNub.Async.Tests.Common.Properties;
using Xunit;

namespace PubNub.Async.Presence.Tests.Services
{
	public class PresenceServiceTests : AbstractTest
	{
		[Fact]
		[Trait("Category", "integration")]
		public async Task SetState__Given_ConfiguredClientAndState__Then_SetStateGetState()
		{
			var state = Fixture.Create<PresenceTestState>();

			var client = Settings.Default.PresenceDecryptedChannel
				.ConfigurePubNub(c =>
				{
					c.SessionUuid = "presence-test";
					c.SubscribeKey = Settings.Default.SubscribeKey;
				});

			var subject = new PresenceService(client, Mock.Of<ICryptoService>());

			await subject.SetState(state);

			var result = await subject.GetState<PresenceTestState>();

			Assert.NotSame(state, result);
			Assert.Equal(state.Foo, result.Foo);
			Assert.Equal(state.Bar, result.Bar);
			Assert.Equal(state.Fubar, result.Fubar);
		}

		public class PresenceTestState
		{
			public string Foo { get; set; }
			public long Bar { get; set; }
			public string[] Fubar { get; set; }
		}
	}
}
