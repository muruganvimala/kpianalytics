using Microsoft.AspNetCore.Mvc;
using BusinessIntelligence_API.Repository;
using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;
using System.Data;

namespace BusinessIntelligence_API.Controllers
{
	[Route("api/")]
	[ApiController]
	public class ForexController : BaseController
    {
        private readonly IForexRepository _forexRepository;
        private readonly JTSContext _jtscontext;

        public ForexController(JTSContext jTSContext, IForexRepository forexRepository, IHttpContextAccessor httpContextAccessor)
        {
            _jtscontext = jTSContext;
            _forexRepository = forexRepository;
            InitializeApiCallLog(httpContextAccessor);
        }

        [HttpPost("forex/insert")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Insert([FromBody] BiForex biForex)
        {
            var requestData = JsonConvert.SerializeObject(biForex);
            _biApiCallLog.RequestData = requestData;
            if (ModelState.IsValid)
            {
                await _forexRepository.InsertAsync(biForex);
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Record Inserted Successfully";
                return Ok(new { Message = "Record Inserted Successfully", status = true, Data = biForex });
            }
            else
            {
                _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                _biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
                return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Errors = "Error" });
            }
        }
		[HttpPost("forex/bulkinsert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Create([FromBody] List<BiForex_mapper> items)
		{
			try
			{
				List<string> jsonStrings = new List<string>();
				foreach (var item in items)
				{
					var reqestDate = JsonConvert.SerializeObject(item);
					_biApiCallLog.RequestData = reqestDate;
					var dictionary = new Dictionary<string, object>
			{
				{ "Date", item.Date },
				{ "UsdInr", item.UsdInr },
				{ "GbpInr", item.GbpInr },
				{ "PhpInr", item.PhpInr },
				{ "UsdGbp", item.UsdGbp }
			};

					BiForex biForex = new BiForex
					{
						Date = Convert.ToDateTime(item.Date),
						UsdInr = Convert.ToDecimal(item.UsdInr),
						GbpInr = Convert.ToDecimal(item.GbpInr),
						PhpInr = Convert.ToDecimal(item.PhpInr),
						UsdGbp = Convert.ToDecimal(item.UsdGbp)
					};

					if (ModelState.IsValid)
					{
						await _forexRepository.InsertAsync(biForex);
						dictionary.Add("insert", true);
					}
					else
					{
						dictionary.Add("insert", false);
					}

					var jsonString = JsonConvert.SerializeObject(dictionary);
					jsonStrings.Add(jsonString);
				}

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Records Inserted Successfully";
				return Ok(new { Message = "Records Inserted Successfully", status = true, Data = jsonStrings });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to insert records", status = false, Error = ex.Message });
			}
		}


		[HttpPost("forex/validateexcel")]
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

				var items = new List<BiForex>();
				List<object> jsonObjects = new List<object>();
				foreach (DataRow row in dt.Rows)
				{
					var dictionary = new Dictionary<string, object>();
					foreach (DataColumn col in dt.Columns)
					{
						var columnName = col.ColumnName switch
						{
							"Forex Date" => "Date",
							"USD / INR" => "UsdInr",
							"GBP / INR" => "GbpInr",
							"PHP / INR" => "PhpInr",
							"USD / GBP" => "UsdGbp",
							_ => col.ColumnName // Keep the original column name if not renamed
						};

						dictionary[columnName] = row[col];
					}

					try
                    {
						var item = MapDataRowToBiForexData(row);						
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

		private BiForex MapDataRowToBiForexData(DataRow row)
		{
			return new BiForex
			{
				Date = row["Forex Date"] != DBNull.Value ? Convert.ToDateTime(row["Forex Date"]) : DateTime.MinValue,
				UsdInr = Convert.ToDecimal(row["USD / INR"]),
				GbpInr = Convert.ToDecimal(row["GBP / INR"]),
				PhpInr = Convert.ToDecimal(row["PHP / INR"]),
				UsdGbp = Convert.ToDecimal(row["USD / GBP"])
			};
		}

		
        [HttpGet("forex/display")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Display()
        {
            try
            {
                var result = await _forexRepository.GetAllAsync();
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

        [HttpGet("forex/displaybyid/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DisplayById(int id)
        {
            try
            {
                _biApiCallLog.RequestData = id.ToString();
                var result = await _forexRepository.GetByIdAsync(id);
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

        //public async Task<IActionResult> DisplayById(int id)
        //{
        //    try
        //    {
        //        _biApiCallLog.RequestData = id.ToString();
        //        BiForex result = await _forexRepository.GetByIdAsync(id);
        //        if (result != null)
        //        {
        //            _biApiCallLog.StatusCode = StatusCodes.Status200OK;
        //            _biApiCallLog.ResponseData = "Record fetched";
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            _biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
        //            _biApiCallLog.ResponseData = "No record found";
        //            return NotFound("No record found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception as needed
        //        _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
        //        _biApiCallLog.ResponseData = "Unable to get data";
        //        _biApiCallLog.Exception = ex.Message + ex.StackTrace;
        //        return BadRequest("Unable to get data");
        //    }
        //}

        [HttpPut("forex/update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update([FromBody] BiForex biForex)
        {
            try
            {
                await _forexRepository.UpdateAsync(biForex);
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = $"Record with ID {biForex.Id} updated successfully.";
                return Ok(new { Message = $"Record with ID {biForex.Id} updated successfully.", status = true });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = $"Unable to update record with ID {biForex.Id}";
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;
                return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
            }
        }

        [HttpDelete("forex/deletebyid/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeletebyId(int id)
        {
            try
            {
                await _forexRepository.DeleteAsync(id);
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

        [HttpPost("forex/range")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RangedDisplay(DateRange range)
        {
            try
            {
                var result = await _forexRepository.GetDateRanged(range);
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

        [HttpPost("forex/import")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Import([FromBody] IEnumerable<BiRawForex> biForex)
        {
            var requestData = JsonConvert.SerializeObject(biForex);
            _biApiCallLog.RequestData = requestData;
            if (ModelState.IsValid)
            {
                await _forexRepository.ImportAsync(biForex);
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Record Imported Successfully";
                return Ok(new { Message = "Record Imported Successfully", status = true, Data = biForex });
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
