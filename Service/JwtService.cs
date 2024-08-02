using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Service
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public string FailureReason { get; set; }
    }

    public class JwtService
    {
        private readonly string _secretKey;
        private readonly int _expiryInMinutes;
        private readonly ConcurrentDictionary<string, DateTime> _tokenStorage;
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _secretKey = configuration["JWT:Secret"];
            _expiryInMinutes = Convert.ToInt32(configuration["JWT:ExpiryInMinutes"]);
            _tokenStorage = new ConcurrentDictionary<string, DateTime>();
            _configuration = configuration;
        }

        public (string Token, DateTime? Expiry) GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            string role = GetRolesForUser(username);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim> {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("displayname", username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),

                Expires = DateTime.UtcNow.AddMinutes(_expiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Store the token and its expiration time in the dictionary
            _tokenStorage.TryAdd(tokenString, tokenDescriptor.Expires ?? DateTime.UtcNow.AddMinutes(_expiryInMinutes));

            // Return the token string and its expiration time
            return (Token: tokenString, Expiry: tokenDescriptor.Expires);
        }

        public DateTime? GetTokenExpiration(string token)
        {
            if (_tokenStorage.TryGetValue(token, out var expirationTime))
            {
                return expirationTime;
            }

            return null;
        }

        private string GetRolesForUser(string username)
        {
			// Add your logic to determine roles based on the username
			//         if (username.ToLower() == "Admin")
			//         {
			//	return "Admin";
			//         }           
			//         else
			//         {
			//	return "User";
			//}
			return "User";
		}

        public TokenValidationResult ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Check if the token is still valid based on the ValidTo property
                if (validatedToken.ValidTo > DateTime.UtcNow)
                {
                    return new TokenValidationResult
                    {
                        IsValid = true,
                        Principal = principal
                    };
                }

                return new TokenValidationResult
                {
                    IsValid = false,
                    FailureReason = "Token is expired"
                };
            }
            catch (SecurityTokenException ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    FailureReason = $"Token validation failed: {ex.Message}"
                };
            }
        }
	}

	
}
