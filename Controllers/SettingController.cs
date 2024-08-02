using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessIntelligence_API.Model;
using BusinessIntelligence_API.Models;
using Microsoft.IdentityModel.Tokens;
using BusinessIntelligence_API.Repository;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace BusinessIntelligence_API.Controllers
{
	[ApiController]
	public class SettingController : BaseController
	{
		private readonly ISettingRepository _settingRepository;
		private readonly JTSContext _jtscontext;

        public SettingController(JTSContext jTSContext, ISettingRepository settingRepository, IHttpContextAccessor httpContextAccessor)
        {
			_settingRepository = settingRepository;
			_jtscontext = jTSContext;
			InitializeApiCallLog(httpContextAccessor);
		}

		[HttpPost("PublisherConfig/Insert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Insert([FromBody] BiPublisherConfig biPublisherConfig)
		{
			var requestData = JsonConvert.SerializeObject(biPublisherConfig);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{
				await _settingRepository.InsertAsync(biPublisherConfig);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record Inserted Successfully";
				return Ok(new { Message = "Record Inserted Successfully", status = true });
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Error = "Error" });
			}
		}

		[HttpGet("PublisherConfig/Display")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Display()
		{
			try
			{
				var result = await _settingRepository.GetAllAsync();
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = result });
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

		[HttpGet("PublisherConfig/DisplayById/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DisplayById(int id)
		{
			try
			{
				_biApiCallLog.RequestData = id.ToString();
				var result = await _settingRepository.GetByIdAsync(id);
				if (result == null)
				{
					_biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
					_biApiCallLog.ResponseData = "Record not found";
					return NotFound(new { Message = "Record not found", status = false });
				}

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record fetched";
				return Ok(new { Status = true, Data = result });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "Unable to get data";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}


		[HttpPut("PublisherConfig/Update")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Update([FromBody] BiPublisherConfig biPublisherConfig)
		{
			try
			{
				if (biPublisherConfig == null)
				{
					_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
					_biApiCallLog.ResponseData = "Invalid request body";
					return BadRequest(new { Message = "Invalid request body", status = false });
				}

				var requestData = JsonConvert.SerializeObject(biPublisherConfig);
				_biApiCallLog.RequestData = requestData;

				await _settingRepository.UpdateAsync(biPublisherConfig);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = $"Record with ID {biPublisherConfig.Id} updated successfully.";

				return Ok(new { Message = $"Record with ID {biPublisherConfig.Id} updated successfully.", status = true });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = $"Unable to update record with ID {biPublisherConfig.Id}";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
			}
		}

		[HttpDelete("PublisherConfig/DeleteById/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteById(int id)
		{
			try
			{
				_biApiCallLog.RequestData = id.ToString();

				await _settingRepository.DeleteAsync(id);

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
	}
}
