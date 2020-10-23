using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetConnect.Models;
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
		private readonly ILogger<ConnectionWorker> _logger;
		UdpClient _broadcaster;
		UdpClient _listener;

		ConcurrentBag<NetHost> _hosts = new ConcurrentBag<NetHost>();


		public ConnectionWorker(IOptionsMonitor<AppConfiguration> appConfig,
			ILogger<ConnectionWorker> logger)
		{
			_appConfig = appConfig;
			_logger = logger;
		}



		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_broadcaster = new UdpClient();
			StartListening(stoppingToken);
			while (!stoppingToken.IsCancellationRequested)
			{
				await SendBroadcastAsync();

				await Task.Delay(1000);
			}
		}


		private void StartListening(CancellationToken token)
		{
			int port = _appConfig.CurrentValue.Port;
			var ep = new IPEndPoint(IPAddress.Any, port);
			_listener = new UdpClient(port);

			Task.Run(() =>
			{
				while (!token.IsCancellationRequested)
				{
					try
					{
						var data = _listener.Receive(ref ep);
						_ = Task.Run(() =>
						{
							var msg = new DiscoveryMessage(data);
							_logger.LogInformation("Host found: " + msg.Host.DeviceId);
							_hosts.Add(msg.Host);
						});
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error listening to replies");
					}
				}
			}, token);

		}

		private async Task SendBroadcastAsync()
		{
			var data = DiscoveryMessage.Create(_appConfig.CurrentValue);
			await _broadcaster.SendAsync(data, data.Length, IPAddress.Broadcast.ToString(), _appConfig.CurrentValue.Port);
		}
	}
}
