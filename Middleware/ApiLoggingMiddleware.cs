using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;

namespace BusinessIntelligence_API.Middleware
{
	public class ApiLoggingMiddleware
	{
		private readonly RequestDelegate _next;

		public ApiLoggingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, ILogServiceRepository logService)
		{
			var apiCallLog = new BiApiCallLog
			{
				Method = context.Request.Method,
				Endpoint = context.Request.Path.ToString()
			};
			context.Items["ApiCallLog"] = apiCallLog;

			await _next(context);

			await logService.InsertApiLog(apiCallLog, context.Response.StatusCode);
		}
	}
}
