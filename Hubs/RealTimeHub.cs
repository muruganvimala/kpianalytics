using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace BusinessIntelligence_API.Hubs
{
	public class RealTimeHub : Hub
	{
		public async Task SendRealTimeData(object data)
		{
			await Clients.All.SendAsync("UpdateRealTimeData", data);
		}
	}
}
