using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BusinessIntelligence_API.Middleware
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ErrorHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = HttpStatusCode.InternalServerError; // 500 if unexpected

			// Customize the response based on the exception type
			if (exception is UnauthorizedAccessException)
			{
				code = HttpStatusCode.Unauthorized;
			}
			else
			{
				code = HttpStatusCode.NotFound;
			}

			var result = JsonConvert.SerializeObject(new { error = exception.Message });
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			await context.Response.WriteAsync(result);
		}
	}

	public static class ErrorHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ErrorHandlingMiddleware>();
		}
	}
}
