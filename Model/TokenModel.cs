using System.Security.Claims;

namespace BusinessIntelligence_API.Model
{
	public class TokenValidationResult
	{
		public bool IsValid { get; set; }
		public ClaimsPrincipal Principal { get; set; }
		public string FailureReason { get; set; }
	}
}
