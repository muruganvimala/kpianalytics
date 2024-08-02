using BusinessIntelligence_API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BusinessIntelligence_API.Repository;
using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace BusinessIntelligence_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RealTimeController : BaseController
	{
		private readonly JTSContext _jtscontext;
		private readonly IBiPerformanceMetricRepository _biPerformanceMetricRepository;

		private readonly IHubContext<RealTimeHub> _hubContext;

		public RealTimeController(IHubContext<RealTimeHub> hubContext, JTSContext jTSContext, IBiPerformanceMetricRepository biPerformanceMetricRepository, IHttpContextAccessor httpContextAccessor)
        {
			_biPerformanceMetricRepository = biPerformanceMetricRepository;
			_jtscontext = jTSContext;
			_hubContext = hubContext;
			InitializeApiCallLog(httpContextAccessor);
		}


		[HttpGet("realtime")]
		[AllowAnonymous]
		public async Task<IActionResult> GetRealTimeData()
		{
			var realTimeData = await _biPerformanceMetricRepository.GetAllAsync(); ;

			await _hubContext.Clients.All.SendAsync("UpdateRealTimeData", realTimeData);
			_biApiCallLog.StatusCode = StatusCodes.Status200OK;
			_biApiCallLog.ResponseData = $"Record fetched successfully.";
			return Ok(realTimeData);
		}
	}
}
