using Microsoft.Extensions.Options;
using NetConnect.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetConnect.Core.Services
{
	public class BroadcastService : IBroadcastService
	{
		private readonly IOptionsMonitor<AppConfiguration> _appConfig;

		private UdpClient _broadcaster;

		public BroadcastService(IOptionsMonitor<AppConfiguration> appConfig)
		{
			_appConfig = appConfig;
			_broadcaster = new UdpClient();
		}

		public void SendBroadcast()
		{
			var data = DiscoveryMessage.Create(_appConfig.CurrentValue);
			Task.Run(() => _broadcaster.Send(data, data.Length, IPAddress.Broadcast.ToString(), _appConfig.CurrentValue.Port));
		}
	}
}
