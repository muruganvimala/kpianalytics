using Microsoft.AspNetCore.Mvc;
using BusinessIntelligence_API.Repository;
using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace BusinessIntelligence_API.Controllers
{
	[ApiController]
	public class PublisherController : BaseController
	{
		private readonly IPublisherRepository _publisherRepository;
		//private readonly JTSContext _jtscontext;

		public PublisherController(IPublisherRepository publisherRepository, IHttpContextAccessor httpContextAccessor)
        {
			//_jtscontext = jTSContext;
			_publisherRepository = publisherRepository;
			InitializeApiCallLog(httpContextAccessor);
		}


		[HttpPost("Publisher/Insert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Insert([FromBody] BiPublisher biPublisher)
		{
			var requestData = JsonConvert.SerializeObject(biPublisher);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{				
				await _publisherRepository.InsertAsync(biPublisher);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record Inserted Successfully";
				return Ok(new { Message = "Record Inserted Successfully", status = true, Data = biPublisher });
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Errors = "Error" });
			}
		}

		[HttpGet("Publisher/Display")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Display()
		{
			try
			{
				var result = await _publisherRepository.GetAllAsync();
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

		[HttpGet("Publisher/DisplayById/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DisplayById(int id)
		{
			try
			{
				_biApiCallLog.RequestData = id.ToString();
				var result = await _publisherRepository.GetByIdAsync(id);
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

		[HttpPut("Publisher/Update")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Update([FromBody] BiPublisher biPublisher)
		{
			try
			{
				await _publisherRepository.UpdateAsync(biPublisher);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = $"Record with ID {biPublisher.Id} updated successfully.";
				return Ok(new { Message = $"Record with ID {biPublisher.Id} updated successfully.", status = true });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = $"Unable to update record with ID {biPublisher.Id}";
				_biApiCallLog.Exception  = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
			}
		}

		[HttpDelete("Publisher/DeletebyId/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeletebyId(int id)
		{
			try
			{
				await _publisherRepository.DeleteAsync(id);
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
