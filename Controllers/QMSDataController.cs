using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;

namespace BusinessIntelligence_API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class QMSDataController : BaseController
    {

        private readonly IQMSDataRepository _repository;
        private readonly Models.JTSContext _jtscontext;
        public QMSDataController(Models.JTSContext jTSContext, IQMSDataRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _jtscontext = jTSContext;
            _repository = repository;
            InitializeApiCallLog(httpContextAccessor);
        }

        [HttpPost("qmsdata/insert")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([FromBody] BiQmsDatum item)
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

        [HttpGet("qmsdata/display")]
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

        [HttpPost("qms/report")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BiQMSReport(BiQmsFilter fParam)
        {
            try
            {
                // Retrieve data from the repository
                var item1 = await _repository.QMSDataDashboardReport(fParam);
                var item2 = await _repository.QMSFeedbackDashboardReport(fParam);

                // Log successful retrieval
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Data retrieved successfully";

                // Return success response with the data
                return Ok(new { Status = true, Data = new { QMSData = item1 ,QMSFeedback = item2 } });
            }
            catch (Exception ex)
            {
                // Log the exception details
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = ex.Message;
                _biApiCallLog.Exception = $"{ex.Message}{Environment.NewLine}{ex.StackTrace}";

                // Return error response with the exception message
                return StatusCode(500, new { Status = false, Message = "Unable to get data", Error = ex.Message });
            }
        }



        [HttpGet("qmsdata/displaybyid/{id}")]
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

        [HttpPut("qmsdata/update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update(BiQmsDatum item)
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

        [HttpDelete("qmsdata/deletebyid/{id}/{loginId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(long id,int loginId)
        {
            try
            {
                await _repository.Delete(id,loginId);
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


        [HttpPost("qmsdata/validateexcel")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ValidateExcel([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                    _biApiCallLog.ResponseData = "No file uploaded";
                    return BadRequest(new { Message = "No file uploaded", status = false });
                }

                DataTable dt;
                using (var stream = file.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                            _biApiCallLog.ResponseData = "Excel file is empty";
                            return BadRequest(new { Message = "Excel file is empty", status = false });
                        }

                        dt = new DataTable();
                        foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                        {
                            dt.Columns.Add(firstRowCell.Text.Trim());
                        }

                        for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                        {
                            var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                            var newRow = dt.Rows.Add();
                            foreach (var cell in row)
                            {
                                newRow[cell.Start.Column - 1] = cell.Text;
                            }
                        }
                    }
                }

                var items = new List<BiQmsDatum>();
                List<object> jsonObjects = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    var dictionary = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var columnName = col.ColumnName switch
                        {
                            "Publisher Name" => "publisherName",
                            "EPP ( FP )" => "eppFp",
                            "EPP ( Rev )" => "eppRev",
                            "Feedback" => "feedback",
                            "EPP" => "epp",
                            "RFT" => "rft",
                            "Positive" => "positive",
                            "PE ( EPP )" => "peEpp",
                            "CE ( EPP )" => "ceEpp",
                            "TYP ( EPP )" => "typEpp",
                            "MC ( EPP )" => "mcEpp",
                            "Escalations" => "escalations",
                            "TTP" => "ttp",
                            "Zero Error" => "zeroError",
                            "Author Survey" => "authorSurvey",
                            _ => col.ColumnName // Keep the original column name if not renamed
                        };

                        dictionary[columnName] = row[col];
                    }

                    try
                    {
                        var item = MapDataRowToBiQMSData(row);
                        items.Add(item);
                        dictionary.Add("action", true);
                    }
                    catch (Exception ex)
                    {
                        dictionary.Add("action", false);
                    }

                    jsonObjects.Add(dictionary);
                }

                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Excel validated Successfully";
                return Ok(new { Message = "Excel validated Successfully", status = true, Data = jsonObjects });
            }
            catch (Exception ex)
            {
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = ex.Message;
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;
                return StatusCode(500, new { Message = "Unable to validate records", status = false, Error = ex.Message });
            }
        }

        private BiQmsDatum MapDataRowToBiQMSData(DataRow row)
        {
            return new BiQmsDatum
            {
                PublisherName = Convert.ToString(row["Publisher Name"]),
                EppFp = Convert.ToDecimal(row["EPP ( FP )"] == DBNull.Value ? "0" : row["EPP ( FP )"]),
                EppRev = Convert.ToDecimal(row["EPP ( Rev )"] == DBNull.Value ? "0" : row["EPP ( Rev )"]),
                Feedback = Convert.ToDecimal(row["Feedback"] == DBNull.Value ? "0" : row["Feedback"]),
                Epp = Convert.ToDecimal(row["EPP"] == DBNull.Value ? "0" : row["EPP"]),
                Rft = Convert.ToDecimal(row["RFT"] == DBNull.Value ? "0" : row["RFT"]),
                Positive = Convert.ToDecimal(row["Positive"] == DBNull.Value ? "0" : row["Positive"]),
                PeEpp = Convert.ToDecimal(row["PE ( EPP )"] == DBNull.Value ? "0" : row["PE ( EPP )"]),
                CeEpp = Convert.ToDecimal(row["CE ( EPP )"] == DBNull.Value ? "0" : row["CE ( EPP )"]),
                TypEpp = Convert.ToDecimal(row["TYP ( EPP )"] == DBNull.Value ? "0" : row["TYP ( EPP )"]),
                McEpp = Convert.ToDecimal(row["MC ( EPP )"] == DBNull.Value ? "0" : row["MC ( EPP )"]),
                Escalations = Convert.ToDecimal(row["Escalations"] == DBNull.Value ? "0" : row["Escalations"]),
                Ttp = Convert.ToDecimal(row["TTP"] == DBNull.Value ? "0" : row["TTP"]),
                ZeroError = Convert.ToDecimal(row["Zero Error"] == DBNull.Value ? "0" : row["Zero Error"]),
                AuthorSurvey = Convert.ToDecimal(row["Author Survey"] == DBNull.Value ? "0" : row["Author Survey"])
            };
        }

        [HttpPost("qmsdata/import")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Import([FromBody] IEnumerable<BiRawQmsData> biQmsData)
        {
            var requestData = JsonConvert.SerializeObject(biQmsData);
            _biApiCallLog.RequestData = requestData;
            if (ModelState.IsValid)
            {
                await _repository.ImportAsync(biQmsData);
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Record Imported Successfully";
                return Ok(new { Message = "Record Imported Successfully", status = true, Data = biQmsData });
            }
            else
            {
                _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                _biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
                return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Errors = "Error" });
            }
        }


    }
}
