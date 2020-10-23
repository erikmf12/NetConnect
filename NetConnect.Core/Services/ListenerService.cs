using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetConnect.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetConnect.Core.Services
{
	public class ListenerService : IListenerService
	{
		private readonly IOptionsMonitor<AppConfiguration> _appConfig;
		private readonly ILogger<ListenerService> _logger;

		private UdpClient _listener;


		public event EventHandler<NetHost> OnHostDiscovered;


		public ListenerService(IOptionsMonitor<AppConfiguration> appConfig,
			ILogger<ListenerService> logger)
		{
			_appConfig = appConfig;
			_logger = logger;
		}


		public void StartListening(CancellationToken token)
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
							OnHostDiscovered?.Invoke(this, msg.Host);
						});
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error listening to replies");
					}
				}
			}, token);

		}
	}
}
