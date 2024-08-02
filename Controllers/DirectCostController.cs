using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;

namespace BusinessIntelligence_API.Controllers
{
	[Route("api/")]
	[ApiController]
	public class DirectCostController : BaseController
	{
		private readonly IDirectCostRepository _repository;
		public DirectCostController(IDirectCostRepository repository, IHttpContextAccessor httpContextAccessor)
		{
			_repository = repository;
			InitializeApiCallLog(httpContextAccessor);
		}

		[HttpGet("directcost/display")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var items = await _repository.GetAll();
				string msg = "";
				if (items.Count()>0)
				{
					msg = "Data retrieved successfully";
					_biApiCallLog.ResponseData = msg;
				}
				else
				{
					msg = "Data Not found";
					_biApiCallLog.ResponseData = msg;
				}
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				return Ok(new { Status = true, Data = items , Message = msg });
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

		[HttpGet("directcost/displaybyid/{id}")]
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

		[HttpPost("directcost/insert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Create([FromBody] BiDirectCost item)
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

		[HttpPost("directcost/bulkinsert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Create([FromBody] List<BiDirectCost> items)
		{
			try
			{
				List<string> jsonStrings = new List<string>();
				foreach (var item in items)
				{
					var reqestDate= JsonConvert.SerializeObject(item);
					_biApiCallLog.RequestData = reqestDate;
					var dictionary = new Dictionary<string, object>
			{
				{ "Year", item.Year },
				{ "Type", item.Type },
				{ "Department", item.Department },
				{ "ServiceLine", item.ServiceLine },
				{ "Customer", item.Customer },
				{ "Fc", item.Fc },
				{ "FxRate", item.FxRate },
				{ "CostCtc", item.CostCtc },
				{ "Branch", item.Branch },
				{ "NoOfManDate", item.NoOfManDate }
			};
					if (ModelState.IsValid)
					{
						await _repository.Create(item);
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
				return Ok(new { Message = "Records Inserted Successfully", status = true, Data= jsonStrings });
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

		[HttpPost("directcost/validateexcel")]
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

				var items = new List<BiDirectCost>();
				List<object> jsonObjects = new List<object>();
				foreach (DataRow row in dt.Rows)
				{
					var dictionary = row.Table.Columns.Cast<DataColumn>()
						.ToDictionary(col => col.ColumnName, col => row[col]);
					try
					{
						var item = MapDataRowToBiDirectCost(row);
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


		private BiDirectCost MapDataRowToBiDirectCost(DataRow row)
		{
			return new BiDirectCost
			{
				Year = row.Field<string>("year"),
				Type = row.Field<string>("type"),
				Department = row.Field<string>("department"),
				ServiceLine = row.Field<string>("serviceLine"),
				Customer = row.Field<string>("customer"),
				Fc = row.Field<string>("fc"),
				FxRate = ConvertToNullableDecimal(row, "fxRate"),
				CostCtc = Convert.ToDecimal(row["costCtc"]),
				Branch = row.Field<string>("branch"),
				NoOfManDate = ConvertToNullableDecimal(row, "noOfManDate")
			};
		}

		private decimal? ConvertToNullableDecimal(DataRow row, string columnName)
		{
			if (row.IsNull(columnName))
			{
				return null;
			}
			return Convert.ToDecimal(row[columnName]);
		}


		[HttpPut("directcost/update")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Update(BiDirectCost item)
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

		[HttpDelete("directcost/deletebyid/{id}")]
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
        [HttpPost("directCost/filter")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetFilter(DirectCostFilterParam filterParam)
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
