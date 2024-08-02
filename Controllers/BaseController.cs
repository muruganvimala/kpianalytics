using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BusinessIntelligence_API.Controllers
{
	public abstract class BaseController : ControllerBase
	{
		protected BiApiCallLog _biApiCallLog;

		protected void InitializeApiCallLog(IHttpContextAccessor httpContextAccessor)
		{
			_biApiCallLog = httpContextAccessor.HttpContext.Items["ApiCallLog"] as BiApiCallLog ?? new BiApiCallLog();
		}

		protected void SetLogStatusAndData(int statusCode, string responseData)
		{
			_biApiCallLog.StatusCode = statusCode;
			_biApiCallLog.ResponseData = responseData;
		}
	}
}
