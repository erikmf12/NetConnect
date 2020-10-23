using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetConnect.Host
{
	public class NetConnectHost
	{


		public void Start()
		{
		}


		public IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureServices((hostContext, services) =>
			{
				services.Configure<AppConfiguration>(hostContext.Configuration);

				services.AddHostedService<ConnectionWorker>();

			});

	}
}
