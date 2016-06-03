using System;
using Autofac;
using PubNub.Async.Configuration;

namespace PubNub.Async.Autofac
{
	public class PubNubSettingsBootstrapper : IStartable
	{
		private Lazy<IPubNubSettings> SettingsFactory { get; } 

		public PubNubSettingsBootstrapper(Lazy<IPubNubSettings> settingsFactory)
		{
			SettingsFactory = settingsFactory;
		}

		public void Start()
		{
			PubNub.Settings = SettingsFactory;
		}
	}
}
