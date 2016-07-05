using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PubNub.Async.Services.Access;
using PubNub.Async.Services.Crypto;
using PubNub.Async.Services.History;
using PubNub.Async.Services.Publish;
using PubNub.Async.Services.Subscribe;

namespace PubNub.Async.Configuration
{
	public class DefaultPubNubEnvironment : AbstractPubNubEnvironment, IRegisterService
	{
		private static readonly object AccessRegistrySyncRoot = new object();
		private static IAccessRegistry _accessRegistry;
		public static IAccessRegistry AccessRegistryInstance
		{
			get
			{
				if (_accessRegistry == null)
				{
					lock (AccessRegistrySyncRoot)
					{
						if (_accessRegistry == null)
						{
                            _accessRegistry = new AccessRegistry();
						}
					}
				}
				return _accessRegistry;
			}
		}

		private static readonly object CryptoSyncRoot = new object();
		private static ICryptoService _crypto;
		public static ICryptoService CryptoInstance
		{
			get
			{
				if (_crypto == null)
				{
					lock (CryptoSyncRoot)
					{
						if (_crypto == null)
						{
							_crypto = new CryptoService();
						}
					}
				}
				return _crypto;
			}
		}

	    private static readonly object SubscriptionRegistrySyncRoot = new object();
	    private static ISubscriptionRegistry _subscriptionRegistry;

	    public static ISubscriptionRegistry SubscriptionRegistryInstance
        {
	        get
	        {
	            if (_subscriptionRegistry == null)
	            {
	                lock (SubscriptionRegistrySyncRoot)
	                {
	                    if (_subscriptionRegistry == null)
	                    {
	                        _subscriptionRegistry = new SubscriptionRegistry();
	                    }
	                }
	            }
	            return _subscriptionRegistry;
	        }
	    }
        
		private ConcurrentDictionary<Type, Func<IPubNubClient, object>> Services { get; }

		/// <summary>
		/// DefaultPubNubEnvironment utilizes a flavor of the infamous service locator pattern.
		/// Since we all know this to be an anti-pattern, check out one of pubnub.async's IoC
		/// packages like pubnub.async.autofac.
		/// </summary>
		public DefaultPubNubEnvironment()
		{
			Services = new ConcurrentDictionary<Type, Func<IPubNubClient, object>>();

			Register<ICryptoService>(client => CryptoInstance);
            Register<IAccessRegistry>(client => AccessRegistryInstance);
            Register<ISubscriptionRegistry>(client => SubscriptionRegistryInstance);

            Register<IAccessManager>(client => new AccessManager(client, Resolve<IAccessRegistry>(client)));
			Register<IHistoryService>(client => new HistoryService(client, Resolve<ICryptoService>(client), Resolve<IAccessManager>(client)));
			Register<IPublishService>(client => new PublishService(client, Resolve<ICryptoService>(client), Resolve<IAccessManager>(client)));
            Register<ISubscribeService>(client => new SubscribeService(client, (host, subKey) => GetMonitor(client, host, subKey), Resolve<ISubscriptionRegistry>(client)));
        }

        public void Register<TService>(Func<IPubNubClient, TService> resolver)
		{
			Services[typeof (TService)] = client => resolver(client);
		} 

		public override TService Resolve<TService>(IPubNubClient client)
		{
			return (TService) Services[typeof (TService)](client);
        }

        private readonly IDictionary<Tuple<string, string>, ISubscriptionMonitor> _monitors = new ConcurrentDictionary<Tuple<string, string>, ISubscriptionMonitor>();
        private ISubscriptionMonitor GetMonitor(IPubNubClient client, string host, string subscribeKey)
        {
            var key = Tuple.Create(host, subscribeKey);
            if (!_monitors.ContainsKey(key))
            {
                _monitors[key] = new SubscriptionMonitor(host, subscribeKey, Resolve<ISubscriptionRegistry>(client));
            }
            return _monitors[key];
        }
    }
}