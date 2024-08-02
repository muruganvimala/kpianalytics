using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BusinessIntelligence_API.Controllers
{
	[ApiController]
	public class UserMasterController : BaseController
	{
		private readonly IUserMasterRepository _userMasterRepository;
		private readonly JTSContext _jtscontext;

		public UserMasterController(JTSContext jTSContext , IUserMasterRepository userMasterRepository, IHttpContextAccessor httpContextAccessor)
		{
			_userMasterRepository = userMasterRepository;
			_jtscontext = jTSContext;
			InitializeApiCallLog(httpContextAccessor);
		}

		[HttpPost("Usermaster/Insert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Insert([FromBody] BiUserMaster biUserMaster)
		{
			var requestData = JsonConvert.SerializeObject(biUserMaster);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{
				await _userMasterRepository.InsertAsync(biUserMaster);

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

		[HttpGet("Usermaster/Display")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Display()
		{
			try
			{
				var result = await _userMasterRepository.GetAllAsync();
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

		[HttpGet("Usermaster/DisplayById/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DisplayById(int id)
		{
			try
			{				
				var result = await _userMasterRepository.GetByIdAsync(id);
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
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "Unable to get data";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpPut("Usermaster/Update")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Update([FromBody] BiUserMaster biUserMaster)
		{
			try
			{
				var requestData = JsonConvert.SerializeObject(biUserMaster);
				_biApiCallLog.RequestData = requestData;

				await _userMasterRepository.UpdateAsync(biUserMaster);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = $"Record with Username {biUserMaster.Username} updated successfully.";

				return Ok(new { Message = $"Record with Username {biUserMaster.Username} updated successfully.", status = true });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = $"Unable to update record  {biUserMaster.Username}";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
			}
		}

		[HttpDelete("Usermaster/DeleteById/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteById(int id)
		{
			try
			{
				_biApiCallLog.RequestData = id.ToString();

				await _userMasterRepository.DeleteAsync(id);

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
