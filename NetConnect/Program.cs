using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetConnect.Core.Models;
using NetConnect.Core.Services;
using NetConnect.Workers;

namespace NetConnect
{
	public class Program
	{

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.Configure<AppConfiguration>(hostContext.Configuration);

					services.AddMemoryCache();

					services.AddTransient<IBroadcastService, BroadcastService>();
					services.AddSingleton<IListenerService, ListenerService>();
					services.AddSingleton<ConnectionManager>();

					services.AddHostedService<ConnectionWorker>();

				});
	}
}
