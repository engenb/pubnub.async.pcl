using System;
using Ploeh.AutoFixture;
using Xunit;

namespace PubNub.Async.Tests
{
	public class PubNubTests : AbstractTest
	{
		[Fact]
		public void ctor__Given_PubAndSubKeys__Then_EmptyNonProvidedSSLTrue()
		{
			var expectedPubKey = Fixture.Create<string>();
			var expectedSubKey = Fixture.Create<string>();
			var expectedSecKey = string.Empty;
			var expectedCiphKey = string.Empty;
			var expectedSsl = true;

			var subject = new PubNub(expectedPubKey, expectedSubKey);

			Assert.Equal(expectedPubKey, subject.PublishKey);
			Assert.Equal(expectedSubKey, subject.SubscribeKey);
			Assert.Equal(expectedSecKey, subject.SecretKey);
			Assert.Equal(expectedCiphKey, subject.CipherKey);
			Assert.Equal(expectedSsl, subject.SslEnabled);

			Assert.NotNull(subject.SessionUUID);
			Assert.NotEmpty(subject.SessionUUID);
			Guid outGuid;
			Assert.True(Guid.TryParse(subject.SessionUUID, out outGuid));
		}

		[Fact]
		public void ctor__Given_PubSubAndSecKeys__Then_EmptyNonProvidedSSLTrue()
		{
			var expectedPubKey = Fixture.Create<string>();
			var expectedSubKey = Fixture.Create<string>();
			var expectedSecKey = Fixture.Create<string>();
			var expectedCiphKey = string.Empty;
			var expectedSsl = true;

			var subject = new PubNub(expectedPubKey, expectedSubKey, expectedSecKey);

			Assert.Equal(expectedPubKey, subject.PublishKey);
			Assert.Equal(expectedSubKey, subject.SubscribeKey);
			Assert.Equal(expectedSecKey, subject.SecretKey);
			Assert.Equal(expectedCiphKey, subject.CipherKey);
			Assert.Equal(expectedSsl, subject.SslEnabled);

			Assert.NotNull(subject.SessionUUID);
			Assert.NotEmpty(subject.SessionUUID);
			Guid outGuid;
			Assert.True(Guid.TryParse(subject.SessionUUID, out outGuid));
		}

		[Fact]
		public void ctor__Given_PubSubSecAndCiphKeys__Then_EmptyNonProvidedSSLTrue()
		{
			var expectedPubKey = Fixture.Create<string>();
			var expectedSubKey = Fixture.Create<string>();
			var expectedSecKey = Fixture.Create<string>();
			var expectedCiphKey = Fixture.Create<string>();
			var expectedSsl = true;

			var subject = new PubNub(expectedPubKey, expectedSubKey, expectedSecKey, expectedCiphKey, expectedSsl);

			Assert.Equal(expectedPubKey, subject.PublishKey);
			Assert.Equal(expectedSubKey, subject.SubscribeKey);
			Assert.Equal(expectedSecKey, subject.SecretKey);
			Assert.Equal(expectedCiphKey, subject.CipherKey);
			Assert.Equal(expectedSsl, subject.SslEnabled);

			Assert.NotNull(subject.SessionUUID);
			Assert.NotEmpty(subject.SessionUUID);
			Guid outGuid;
			Assert.True(Guid.TryParse(subject.SessionUUID, out outGuid));
		}
	}
}
