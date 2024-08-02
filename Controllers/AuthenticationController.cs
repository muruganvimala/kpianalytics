//using BusinessIntelligence_API.DAL;
using BusinessIntelligence_API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BusinessIntelligence_API.Repository;
using Serilog;
using Microsoft.AspNetCore.Http;

namespace BusinessIntelligence_API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	[EnableCors("CorsPolicy")]
	public class AuthenticationController : BaseController
	{
		private readonly JwtService _jwtService;
		private readonly JTSContext _jTSContext;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly IUserMasterRepository _userMasterRepository;

		public AuthenticationController(JwtService jwtService, JTSContext jTSContext, ILogger<AuthenticationController> logger, IUserMasterRepository userMasterRepository, IHttpContextAccessor httpContextAccessor)
        {
			_jwtService = jwtService;
			_logger = logger;
			_jTSContext = jTSContext;
			_userMasterRepository = userMasterRepository;
			InitializeApiCallLog(httpContextAccessor);
		}

		[HttpPost("GetToken")]
		[AllowAnonymous]
		public async Task<IActionResult> GetTokenAsync(string username, string password)
		{
			if (isValidUser(username, password))
			{				
				string rolename = string.Empty;
				var tokenResult = _jwtService.GenerateToken(username);
				var result = await _userMasterRepository.GetByIdAsync(username);
				
				BiUserMaster userdata = await _jTSContext.BiUserMasters.FirstOrDefaultAsync(item => item.Username == username);
				if (userdata != null)
				{
					int roleid = Convert.ToInt32(userdata.Userrole);					
					rolename = (await _jTSContext.BiRoleMasters.FirstOrDefaultAsync(item => item.RoleId == roleid))?.RoleName;
				}

				_biApiCallLog.StatusCode = 200;
				return Ok(new { Token = tokenResult.Token, Expires = tokenResult.Expiry,User= result,UserRole= rolename });
			}
			else 
			{
				_biApiCallLog.StatusCode = StatusCodes.Status401Unauthorized;
				_biApiCallLog.ResponseData = "Invalid username and password";
				return Unauthorized();
			}
			
		}

		
		[HttpPost("GetTokenInfo")]
		[AllowAnonymous]
		public async Task<IActionResult> GetTokenInfoAsync()
		{
			var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
			var tokenResult = _jwtService.GetTokenExpiration(token);
			_biApiCallLog.ResponseData = tokenResult.ToString();
			return Ok(new { Expires = tokenResult });
		}


		[HttpGet("validate")]
		[Authorize]
		public async Task<IActionResult> ValidateTokenAsync()
		{
			var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
			var validationResult = _jwtService.ValidateToken(token);

			if (validationResult.IsValid)
			{
				_biApiCallLog.ResponseData = "Token is valid";
				return Ok(new { Message = "Token is valid" });
			}
			else
			{
				_biApiCallLog.ResponseData = "Invalid token";
				_biApiCallLog.StatusCode= StatusCodes.Status401Unauthorized;
				return Unauthorized(new { Message = validationResult.FailureReason });
			}
		}


		private bool isValidUser(string username, string password)
		{			
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{				
				if (username.ToLower() == "biuser" && (password == "biuser2023"))
				{
					return true;
				}
				else 
				{
					string encodedPassword = EncodeandDecode.EncodePassword(password);
					//string decodedPassword = EncodeandDecode.DecodePassword("dGVzdA==");
					Boolean isFound = _jTSContext.BiUserMasters.Any(item => item.Username == username && item.Password == encodedPassword && item.Activestatus == true);
					if (isFound)
					{
						return true;
					}
					else
					{
						return false;
					}

				}
				
			}
			return false;
		}


	}
}
