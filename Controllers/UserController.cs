using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessIntelligence_API.Model;
using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BusinessIntelligence_API.Repository;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Cors;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BusinessIntelligence_API.Controllers
{
	[ApiController]
	public class UserController : BaseController
	{
		private readonly IBiPerformanceMetricRepository _biPerformanceMetricRepository;

		private readonly JTSContext _jtscontext;
		
		//Constructor
		public UserController(JTSContext jTSContext, IBiPerformanceMetricRepository biPerformanceMetricRepository, IHttpContextAccessor httpContextAccessor)
		{
			_biPerformanceMetricRepository = biPerformanceMetricRepository;
			_jtscontext = jTSContext;
			InitializeApiCallLog(httpContextAccessor);
		}
		
		
		[HttpGet("User/getpublisher")]
		[Authorize(Roles = "User")]
		public IActionResult getpublisher()
		{
			try
			{
				List<BiPublisher> biPublishers = _jtscontext.BiPublishers.OrderBy(p => p.Acronym).ToList();
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biPublishers });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false,Error = ex.Message });
			}
		}

		//Crud

		[HttpPost("User/KPIDataInsert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> KPIDataInsert([FromBody] BiPerformanceMetric biPerformanceMetric)
		{
			var requestData = JsonConvert.SerializeObject(biPerformanceMetric);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{
				await _biPerformanceMetricRepository.InsertAsync(biPerformanceMetric);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record Inserted Successfully";

				return Ok(new { Message = "Record Inserted Successfully",status=true, Data = biPerformanceMetric });
			}			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Errors = GetModelErrors(biPerformanceMetric) });
			}
		}

		[HttpGet("User/KPIDataDisplay")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> KPIDataDisplay()
		{
			try
			{
				var result = await _biPerformanceMetricRepository.GetAllAsync();
				if (result != null && result.Any())
				{
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = "Data retrieved successfully";
					return Ok(new { Status = true, Data = result });
				}
				else
				{
					_biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
					_biApiCallLog.ResponseData = "No data found";
					return NotFound(new { Message = "No data found", status = false });
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpGet("User/KPIDataDisplaybyConfig")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> KPIDataDisplaybyConfig()
		{
			try
			{
				var result = await _biPerformanceMetricRepository.GetAllConfigAsync();

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";

				return Ok(new { Status = true, Data = result });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				Log.Error(ex, "An error occurred while fetching KPI data by config.");

				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}


		[HttpGet("User/KPIDataDisplayById/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> KPIDataDisplayById(int id)
		{
			try
			{
				var result = await _biPerformanceMetricRepository.GetByIdAsync(id);
				if (result != null)
				{
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = "Record fetched";
					return Ok(new { Status = true, Data = result });
				}
				else
				{
					_biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
					_biApiCallLog.ResponseData = "No record found";
					return NotFound(new { Message = "No record found", status = false });
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "Unable to get data";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpPut("User/UpdateKPI")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> UpdateKPI([FromBody] BiPerformanceMetric biPerformanceMetric)
		{
			try
			{
				var requestData = JsonConvert.SerializeObject(biPerformanceMetric);
				_biApiCallLog.RequestData = requestData;

				await _biPerformanceMetricRepository.UpdateAsync(biPerformanceMetric);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = $"Record with RoleId {biPerformanceMetric.Id} updated successfully.";

				return Ok(new { Message = $"Record with ID {biPerformanceMetric.Id} updated successfully.", status = true });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = $"Unable to update record with ID {biPerformanceMetric.Id}";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
			}
		}

		[HttpDelete("User/DeleteKPI/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteKPI(int id)
		{
			try
			{
				_biApiCallLog.RequestData = id.ToString();
				await _biPerformanceMetricRepository.DeleteAsync(id);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = $"Record with ID {id} deleted successfully.";

				return Ok(new { Message = $"Record with ID {id} deleted successfully.", status = true });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = $"Unable to delete record ID {id}";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to delete record", status = false, Error = ex.Message });
			}
		}
		//crud end

		[HttpGet("Admin/resource")]
		[Authorize(Roles = "Admin")]
		public IActionResult AdminResource()
		{
			_biApiCallLog.StatusCode = StatusCodes.Status200OK;
			_biApiCallLog.ResponseData = $"Admin resource accessed.";
			return Ok(new { Message = "Admin resource accessed." });
		}


		[HttpGet("User/resource")]
		[Authorize(Roles = "User")]
		public IActionResult UserResource()
		{
			_biApiCallLog.StatusCode = StatusCodes.Status200OK;
			_biApiCallLog.ResponseData = $"User resource accessed.";
			return Ok(new { Message = "User resource accessed." });
		}

		[HttpGet]
		[Authorize]
		[Route("GetAllUser")]
		public async Task<IActionResult> GetAllUser()
		{
			UserModel user = new UserModel();
			user.FullName = "Admin";
			user.UserMessage = "Welcome";
			user.CreatedDate = DateTime.Now;
			user.EmailId = "murugan@gmail.com";
			return Ok(user);
		}

		private bool isModelValid(KPIDataModel kPIDataModel)
		{
			return false;
		}

		private IEnumerable<string> GetModelErrors(BiPerformanceMetric biPerformanceMetric)
		{			
			List<string> errors = new List<string>();
			if (string.IsNullOrEmpty(biPerformanceMetric.PublisherId))
			{
				errors.Add("PublisherId is required.");
			}
			if (string.IsNullOrEmpty(biPerformanceMetric.MonthYear))
			{
				errors.Add("MonthYear is required.");
			}

			yield break;
		}

		
	}
}
