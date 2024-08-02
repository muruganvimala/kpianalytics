using BusinessIntelligence_API.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; // Add this using directive
using System.Threading.Tasks;

namespace BusinessIntelligence_API.Middleware
{
	public class CustomAuthenticationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly PathString[] excludedPaths = { "/api/Login/GetToken", "/api/Login/validate" };
		private readonly JwtService _jwtService;

		public CustomAuthenticationMiddleware(RequestDelegate next, JwtService jwtService)
		{
			_next = next;
			_jwtService = jwtService;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Skip authentication for the specified paths
			if (ShouldSkipAuthentication(context.Request.Path))
			{
				await _next(context);
				return;
			}

			// Implement your custom authentication logic here
			if (!IsAuthenticated(context.Request.Headers["Authorization"]))
			{			
				await context.ChallengeAsync("Bearer", new AuthenticationProperties { RedirectUri = "/path-to-redirect-if-auth-fails" });
				return;
			}

			// Call the next middleware in the pipeline
			await _next(context);
			return;
		}

		private bool ShouldSkipAuthentication(PathString path)
		{
			// Check if the path is in the excluded paths
			return Array.Exists(excludedPaths, p => path.StartsWithSegments(p));
		}

		private bool IsAuthenticated(string authorizationHeader)
		{
			var token = authorizationHeader?.Replace("Bearer ", string.Empty);
			return token != null && _jwtService.ValidateToken(token) != null;
		}
	}
}
