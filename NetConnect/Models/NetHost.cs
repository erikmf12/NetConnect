using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace NetConnect.Models
{
	public class NetHost
	{
		public string IpAddress { get; set; }
		public string MacAddress { get; set; }
		public string DeviceId { get; set; }


		private static NetHost _localHost;
		public static NetHost GetLocalHost(AppConfiguration appConfig)
		{
			if (_localHost == null)
			{
				_localHost = new NetHost
				{
					DeviceId = appConfig.DeviceId
				};
				var hostEntry = Dns.GetHostEntry(Dns.GetHostName());

				var mac = NetworkInterface.GetAllNetworkInterfaces()
					.FirstOrDefault(x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
						x.OperationalStatus != OperationalStatus.Down)
					?.GetPhysicalAddress();

				if (mac != null)
					_localHost.MacAddress = mac.ToString();

				foreach (var he in hostEntry.AddressList)
				{
					if (he.AddressFamily == AddressFamily.InterNetwork)
					{
						_localHost.IpAddress = he.ToString();
						break;
					}
				}
			}

			return _localHost;
		}
	}
}
