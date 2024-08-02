using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BusinessIntelligence_API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class QMSFeedbackController : BaseController
    {
        private readonly IQMSFeedbackRepository _repository;
        public QMSFeedbackController(IQMSFeedbackRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            //_jtscontext = jTSContext;
            _repository = repository;
            InitializeApiCallLog(httpContextAccessor);
        }

        [HttpPost("qmsfeedback/insert")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([FromBody] BiQmsFeedback item)
        {
            try
            {
                var requestData = JsonConvert.SerializeObject(item);
                _biApiCallLog.RequestData = requestData;

                if (ModelState.IsValid)
                {
                    await _repository.Create(item);
                    _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                    _biApiCallLog.ResponseData = "Record Inserted Successfully";
                    return Ok(new { Message = "Record Inserted Successfully", status = true, Data = item });
                }
                else
                {
                    _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                    _biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
                    return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Errors = "Error" });
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = ex.Message;
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;
                return StatusCode(500, new { Message = "Unable to insert record", status = false, Error = ex.Message });
            }
        }

        [HttpGet("qmsfeedback/display")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _repository.GetAll();
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Data retrieved successfully";
                return Ok(new { Status = true, Data = items });
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

        [HttpGet("qmsfeedback/displaybyid/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                _biApiCallLog.RequestData = id.ToString();
                var result = await _repository.GetById(id);
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

        [HttpPut("qmsFeedback/update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update(BiQmsFeedback item)
        {
            try
            {
                _biApiCallLog.RequestData = JsonConvert.SerializeObject(item);
                await _repository.Update(item);
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = $"Record with ID {item.Id} updated successfully.";
                return Ok(new { Message = $"Record with ID {item.Id} updated successfully.", status = true });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = $"Unable to update record with ID {item.Id}";
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;
                return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
            }
        }

        [HttpDelete("qmsfeedback/deletebyid/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _repository.Delete(id);
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
