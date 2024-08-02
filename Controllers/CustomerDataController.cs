using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;

namespace BusinessIntelligence_API.Controllers
{
	[Route("api/")]
	[ApiController]
	public class CustomerDataController : BaseController
	{
		private readonly ICustomerDatumRepository _repository;
		public CustomerDataController(ICustomerDatumRepository repository, IHttpContextAccessor httpContextAccessor)
		{
			_repository = repository;
			InitializeApiCallLog(httpContextAccessor);
		}

		[HttpGet("customerdata/display")]
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

		[HttpGet("customerdata/displaybyid/{id}")]
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

		[HttpPost("customerdata/insert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Create([FromBody] BiCustomerDatum item)
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

		[HttpPost("customerdata/validateexcel")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> ValidateCustomerData([FromForm] IFormFile file)
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

				var items = new List<BiCustomerDatum>();
				List<object> jsonObjects = new List<object>();
				foreach (DataRow row in dt.Rows)
				{
					var dictionary = row.Table.Columns.Cast<DataColumn>()
						.ToDictionary(col => col.ColumnName, col => row[col]);

					try
					{
						var item = MapDataRowToBiCustomerDatum(row);
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

		private BiCustomerDatum MapDataRowToBiCustomerDatum(DataRow row)
		{
			try
			{
				BiCustomerDatum btm = new BiCustomerDatum();
				btm.UA = row.Field<string>("ua");
				btm.Year = row.Field<string>("year");
				btm.InvoiceNo = row.Field<string>("invoiceNo");
				btm.InvoiceDate = DateTime.Parse(row.Field<string>("invoiceDate"));
				btm.CustomerName = row.Field<string>("customerName");
				btm.CustomerAcronym = row.Field<string>("customerAcronym");
				return btm;
				return new BiCustomerDatum
				{
					UA = row.Field<string>("ua"),
					Year = row.Field<string>("year"),
					InvoiceNo = row.Field<string>("invoiceNo"),
					InvoiceDate = DateTime.Parse(row.Field<string>("invoiceDate")),
					CustomerName = row.Field<string>("customerName"),
					CustomerAcronym = row.Field<string>("customerAcronym"),
					Tspage = row.Field<long>("tspage"),
					Ccytype = row.Field<string>("ccytype"),
					MajorHeadServiceLine = row.Field<string>("majorHeadServiceLine"),
					MinorHead = row.Field<string>("minorHead"),
					Quantity = row.Field<long>("quantity"),
					Uom = row.Field<string>("uom"),
					Rate = row.Field<decimal>("rate"),
					//GrowssValueFc = row.Field<decimal>("growssValueFc"),
					CollectionDate = DateTime.Parse(row.Field<string>("collectionDate")),
					CollectionValueFc = row.Field<decimal>("collectionValueFc"),
					//Fxrate = row.Field<decimal?>("fxrate"),
					CollectionValueInr = row.Field<decimal>("collectionValueInr"),
					ForexGainLoss = row.Field<decimal?>("forexGainLoss"),
					Irm = row.Field<string>("irm"),
					StpiSubmissionDate = DateTime.Parse(row.Field<string>("stpiSubmissionDate")),
					SoftexNo = row.Field<string>("softexNo"),
					EdpmsUploadDate = DateTime.Parse(row.Field<string>("edpmsUploadDate")),
					EdpmsRefNo = row.Field<string>("edpmsRefNo"),
					EdpmsClosureYnp = row.Field<string>("edpmsClosureYnp"),
					EbrcNo = row.Field<string>("ebrcNo"),
					EbrcDate = DateTime.Parse(row.Field<string>("ebrcDate")),
					AdBank = row.Field<string>("adBank"),
					NewBusiness = row.Field<string>("newBusiness") == "Yes",
					AgedDays = row.Field<decimal>("agedDays")
				};
			}
			catch (Exception ex)
			{
				// If an error occurs during mapping, include the error message in the dictionary
				var errorMessage = $"Error mapping row to BiCustomerDatum: {ex.Message}";
				var dictionary = row.Table.Columns.Cast<DataColumn>()
					.ToDictionary(col => col.ColumnName, col => row[col]);
				dictionary.Add("error", errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}



		[HttpPut("customerdata/update")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Update(BiCustomerDatum item)
		{			
			try
			{
				_biApiCallLog.RequestData = JsonConvert.SerializeObject(item);
				await _repository.Update(item.Id, item);
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

		[HttpDelete("customerdata/deletebyid/{id}")]
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

        [HttpPost("customerdata/filter")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetFilter(CustomerDataFilterParam filterParam)
        {
            try
            {
                var items = await _repository.GetDataByFilter(filterParam);
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
    }
}
