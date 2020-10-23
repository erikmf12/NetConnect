using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NetConnect.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetConnect.Core.Services
{
	public class ConnectionManager
	{
		private readonly IMemoryCache _memoryCache;
		private readonly IOptionsMonitor<AppConfiguration> _appConfig;
		private List<string> _keys = new List<string>();

		public ConnectionManager(IMemoryCache memoryCache, 
			IOptionsMonitor<AppConfiguration> appConfig)
		{
			_memoryCache = memoryCache;
			_appConfig = appConfig;
		}

		public IEnumerable<NetHost> Hosts
		{
			get
			{
				foreach (var key in _keys)
					yield return (NetHost)_memoryCache.Get(key);
			}
		}

		public void AddHost(NetHost host)
		{
			var key = host.DeviceId + host.MacAddress;
			if (!_keys.Contains(key))
			{
				_keys.Add(key);
			}

			_memoryCache.Set(key, host, CreateOptions());
		}


		private MemoryCacheEntryOptions CreateOptions()
		{
			var options = new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_appConfig.CurrentValue.DisconnectTimeout)
			};
			var callback = new PostEvictionDelegate((key, value, reason, state) =>
			{
				_keys.Remove((string)key);
			});
			options.RegisterPostEvictionCallback(callback);
			return options;
		}

	}
}
