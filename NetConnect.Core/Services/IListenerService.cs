using NetConnect.Core.Models;
using System;
using System.Threading;

namespace NetConnect.Core.Services
{
	public interface IListenerService
	{
		event EventHandler<NetHost> OnHostDiscovered;

		void StartListening(CancellationToken token);
	}
}