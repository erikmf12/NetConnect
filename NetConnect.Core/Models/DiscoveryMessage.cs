using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetConnect.Core.Models
{
	public class DiscoveryMessage
	{
		public byte[] Data { get; }

		public NetHost Host { get; set; }

		public static byte[] Create(AppConfiguration config)
		{
			var localHost = NetHost.GetLocalHost(config);
			var data = new byte[64];
			byte[] ipBytes = Encoding.Default.GetBytes(localHost.IpAddress);
			byte[] macBytes = Encoding.Default.GetBytes(localHost.MacAddress);
			byte[] idBytes = Encoding.Default.GetBytes(localHost.DeviceId);
			Array.Copy(ipBytes, data, ipBytes.GetUpperBound(0) + 1);
			Array.Copy(macBytes, 0, data, 16, macBytes.GetUpperBound(0) + 1);
			Array.Copy(idBytes, 0, data, 32, idBytes.GetUpperBound(0) + 1);
			return data;
		}

		public DiscoveryMessage(byte[] data)
		{
			Data = data;
			Host = new NetHost
			{
				IpAddress = Encoding.Default.GetString(data.TakeWhile(x => x != 0).ToArray()),
				MacAddress = Encoding.Default.GetString(data.Skip(16).TakeWhile(x => x != 0).ToArray()),
				DeviceId = Encoding.Default.GetString(data[32..64]).Trim('\0')
			};
		}

	}
}
