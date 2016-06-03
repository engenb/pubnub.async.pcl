using System;
using Moq;
using PubNub.Async.Configuration;
using PubNub.Async.Services.Crypto;
using PubNub.Async.Services.History;

namespace PubNub.Async.Testing
{
	public class TestablePubNubSettings : AbstractPubNubSettings
	{
		public TestablePubNubSettings(
			Func<ICryptoService> cryptoFactory = null,
			Func<PubNubClient, IHistoryService> historyFactory = null)
		{
			CryptoFactory = cryptoFactory ?? Mock.Of<ICryptoService>;
			HistoryFactory = historyFactory ?? (client => Mock.Of<IHistoryService>());
		}

		public override Func<ICryptoService> CryptoFactory { get; }
		public override Func<PubNubClient, IHistoryService> HistoryFactory { get; }
	}
}
