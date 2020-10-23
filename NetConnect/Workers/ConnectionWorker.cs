using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetConnect.Core.Models;
using NetConnect.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetConnect.Workers
{
	public class ConnectionWorker : BackgroundService
	{
		private readonly IOptionsMonitor<AppConfiguration> _appConfig;
		private readonly IListenerService _listenerService;
		private readonly IBroadcastService _broadcastService;
		private readonly ConnectionManager _connectionManager;
		private readonly ILogger<ConnectionWorker> _logger;

		private readonly Random _random = new Random();
		private readonly NetHost _localHost;

		public ConnectionWorker(IOptionsMonitor<AppConfiguration> appConfig,
			IListenerService listenerService,
			IBroadcastService broadcastService,
			ConnectionManager connectionManager,
			ILogger<ConnectionWorker> logger)
		{
			_appConfig = appConfig;
			_listenerService = listenerService;
			_broadcastService = broadcastService;
			_connectionManager = connectionManager;
			_logger = logger;

			_localHost = NetHost.GetLocalHost(appConfig.CurrentValue);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_listenerService.OnHostDiscovered += _listenerService_OnHostDiscovered;
			_listenerService.StartListening(stoppingToken);
			while (!stoppingToken.IsCancellationRequested)
			{
				_broadcastService.SendBroadcast();

				await Task.Delay(3000 + _random.Next(-1000, 1000));
			}
			_listenerService.OnHostDiscovered -= _listenerService_OnHostDiscovered;
		}

		private void _listenerService_OnHostDiscovered(object sender, NetHost e)
		{
			if (e != _localHost)
			{
				_connectionManager.AddHost(e);
			}
		}


	}
}
