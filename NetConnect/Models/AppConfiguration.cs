using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace NetConnect.Models
{
	public class AppConfiguration
	{
		public int Port { get; set; }

		public string DeviceId { get; set; }

	}
}
