using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PubNub.Async.Extensions;
using PubNub.Async.Models.Channel;
using PubNub.Async.Tests.Properties;
using Xunit;

namespace PubNub.Async.Tests.Extensions
{
	public class HistoryExtensionsTests : AbstractTest
	{
	    public Channel UnencryptedChannel { get; } = new Channel("history-tests-unencrypted");
	    public Channel EncryptedChannel { get; } = new Channel("history-tests-encrypted")
            .EncryptedWith(Settings.Default.CipherKey);

		[Fact]
		[Category("integration")]
		public async Task History__Given_ConfiguredPubNubWithSSL__When_HistoryNotEnabled__Then_GetError()
		{
			var expectedError = "Use of the history API requires the Storage & Playback add-on which is not enabled for this subscribe key. " +
			                    "Login to your PubNub Dashboard Account and ADD the Storage & Playback add-on. " +
			                    "Contact support@pubnub.com if you require further assistance.";

			var subject = new PubNub(
				Settings.Default.NoFeaturesPublishKey,
				Settings.Default.NoFeaturesSubscribeKey);

			var response = await subject.History<HistoryTestMessage>(UnencryptedChannel, count: 3, reverse: true);

			Assert.Equal(0, response.Start);
			Assert.Equal(0, response.Start);
			Assert.Null(response.Messages);
			Assert.Equal(expectedError, response.Error);
		}

		[Fact]
		[Category("integration")]
		public async Task History__Given_ConfiguredPubNubWithSSL__When_UnencryptedCountIsThreeAndReverse__Then_GetFirstThree()
		{
			var expectedCount = 3;

			var subject = new PubNub(
				Settings.Default.PublishKey,
				Settings.Default.SubscribeKey);

			var response = await subject.History<HistoryTestMessage>(UnencryptedChannel, count: expectedCount, reverse: true);

			Assert.NotNull(response.Messages);
			Assert.Equal(expectedCount, response.Messages.Count());

			var messages = response.Messages.ToArray();
			Assert.Equal("one", messages[0].Content.Message);
			Assert.Equal(14621647024027759L, messages[0].Sent);
			Assert.Equal("two", messages[1].Content.Message);
			Assert.Equal(14621647057951202L, messages[1].Sent);
			Assert.Equal("three", messages[2].Content.Message);
			Assert.Equal(14621647091558573L, messages[2].Sent);

			Assert.Equal(14621647024027759L, response.Start);
			Assert.Equal(14621647091558573L, response.End);
		}

		[Fact]
		[Category("integration")]
		public async Task History__Given_ConfiguredPubNubWithSSL__When_EncryptedCountIsThreeAndReverse__Then_GetDecryptFirstThree()
		{
			var expectedCount = 3;

			var subject = new PubNub(
				Settings.Default.PublishKey,
				Settings.Default.SubscribeKey,
                cipherKey: Settings.Default.CipherKey);

			var response = await subject.History<HistoryTestMessage>(EncryptedChannel, count: expectedCount, reverse: true);

			Assert.NotNull(response.Messages);
			Assert.Equal(expectedCount, response.Messages.Count());

			var messages = response.Messages.ToArray();
			Assert.Equal("one", messages[0].Content.Message);
			Assert.Equal(14646739446049504L, messages[0].Sent);
			Assert.Equal("two", messages[1].Content.Message);
			Assert.Equal(14646739476339247L, messages[1].Sent);
			Assert.Equal("three", messages[2].Content.Message);
			Assert.Equal(14646739500961712L, messages[2].Sent);

			Assert.Equal(14646739446049504L, response.Start);
			Assert.Equal(14646739500961712L, response.End);
		}

		public class HistoryTestMessage
		{
			[JsonProperty("message")]
			public string Message { get; set; }
		}
	}
}
