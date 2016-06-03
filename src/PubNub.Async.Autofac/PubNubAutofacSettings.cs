using System;
using PubNub.Async.Configuration;
using PubNub.Async.Models.Channel;
using PubNub.Async.Services.Crypto;
using PubNub.Async.Services.History;

namespace PubNub.Async.Autofac
{
	public class PubNubAutofacSettings : AbstractPubNubSettings
	{
		public override Func<ICryptoService> CryptoFactory { get; }
		public override Func<PubNubClient, IHistoryService> HistoryFactory { get; }

		public PubNubAutofacSettings(
			Func<ICryptoService> cryptoFactory,
			Func<PubNubClient, IHistoryService> historyFactory)
		{
			CryptoFactory = cryptoFactory;
			HistoryFactory = historyFactory;
		}
	}
}
