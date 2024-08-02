using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using AutoMapper;
using System.Linq;
using System.Data;
using OfficeOpenXml;
using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;

namespace BusinessIntelligence_API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class OtherCostController : BaseController
    {
        private readonly IOtherCostRepository _repository;
        private readonly IMapper _mapper;
        public OtherCostController(IOtherCostRepository repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _repository = repository;
            InitializeApiCallLog(httpContextAccessor);
            _mapper = mapper;
        }

        [HttpGet("othercost/display")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _repository.GetAll();
                string msg = "";
                if (items.Count() > 0)
                {
                    msg = "Data retrieved successfully";
                    _biApiCallLog.ResponseData = msg;
                    //var mapperResult = _mapper.Map<List<BiOtherCost_Mapper>>(items);
                    var mappedItems = items.Select(item => BiOtherCost_Mapper.Map(item)).ToList();
                    _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                    return Ok(new { Status = true, Data = mappedItems, Message = msg });
                }
                else
                {
                    msg = "Data Not found";
                    _biApiCallLog.ResponseData = msg;
                    _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                    return Ok(new { Status = true, Data = items, Message = msg });
                }

            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = ex.Message;
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;
                return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.InnerException.Message });
            }
        }

        [HttpGet("othercost/displaybyid/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                _biApiCallLog.RequestData = id.ToString();
                var result = await _repository.GetById(id);
                if (result != null)
                {
                    // Remove the time component from the result
                    var mapperResult = BiOtherCost_Mapper1.MapWithoutYesNo(result);

                    _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                    _biApiCallLog.ResponseData = "Record fetched";
                    return Ok(new { Status = true, Data = mapperResult });
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
                return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.InnerException.Message });
            }
        }

        [HttpPost("othercost/insert")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([FromBody] BiOtherCost item)
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
                return StatusCode(500, new { Message = "Unable to insert record", status = false, Error = ex.InnerException.Message });
            }
        }

        [HttpPost("othercost/validateexcel")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ValidateExcel([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                    _biApiCallLog.ResponseData = "No file uploaded";
                    return BadRequest(new
                    {
                        Message = "No file uploaded",
                        status = false
                    });
                }

                DataTable dt;
                using (var stream = file.OpenReadStream())
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            _biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
                            _biApiCallLog.ResponseData = "Excel file is empty";
                            return BadRequest(new
                            {
                                Message = "Excel file is empty",
                                status = false
                            });
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

                var items = new List<BiOtherCost>();
                var jsonObjects = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    var dictionary = new Dictionary<string, object>();

                    foreach (DataColumn col in dt.Columns)
                    {
                        var columnName = col.ColumnName switch
                        {
                            "VCH" => "vch",
                            "Supplier Name" => "supplierName",
                            "Type Of Expense" => "typeOfExpense",
                            "Service Line" => "serviceLine",
                            "Customer" => "customer",
                            "Description" => "description",
                            "Invoice No" => "invoiceNo",
                            "Invoice Date" => "invoiceDate",
                            "PO No" => "poNo",
                            "PO Date" => "poDate",
                            "RCM" => "rcm",
                            "HSN SAC" => "hsnSac",
                            "Quantity" => "qty",
                            "Rate" => "rate",
                            "Value" => "value",
                            "FX Rate" => "fxrate",
                            "Value INR" => "valueInr",
                            "Vat" => "vat",
                            "SGST" => "sgst",
                            "CGST" => "cgst",
                            "IGST" => "igst",
                            "Total Invoice Value INR" => "totalInvoiceValueInr",
                            "TDS Applicable" => "tdsApplicable",
                            "TDS Declaration" => "tdsDeclaration",
                            "TDS Section" => "tdsSection",
                            "TDS Rate" => "tdsRate",
                            "TDS Value" => "tdsValue",
                            "Budgeted" => "budgeted",
                            "Budgeted Amount" => "budgetedAmount",
                            "Variance" => "variance",
                            _ => col.ColumnName // Keep the original column name if not renamed
                        };

                        dictionary[columnName] = row[col] == DBNull.Value ? null : row[col];
                    }

                    var requiredKeys = new List<string>
            {
                "vch","supplierName","typeOfExpense","serviceLine","customer","description","invoiceNo","invoiceDate","poNo","poDate","rcm","hsnSac","qty","rate","value","fxrate","valueInr","vat","cgst","sgst","igst","totalInvoiceValueInr","tdsApplicable","tdsDeclaration","tdsSection","tdsRate","tdsValue","budgeted","budgetedAmount","variance"
            };

                    if (requiredKeys.Any(key => !dictionary.ContainsKey(key) || dictionary[key] == null))
                    {
                        dictionary.Add("action", false);
                    }
                    else
                    {
                        try
                        {
                            var item = MapDataRowToBiOtherCost(row);
                            items.Add(item);
                            dictionary.Add("action", true);
                        }
                        catch (Exception ex)
                        {
                            dictionary.Add("action", false);
                            // Log exception here if needed
                        }
                    }

                    jsonObjects.Add(dictionary);
                }

                _biApiCallLog.StatusCode = StatusCodes.Status200OK;
                _biApiCallLog.ResponseData = "Excel validated Successfully";
                return Ok(new
                {
                    Message = "Excel validated Successfully",
                    status = true,
                    Data = jsonObjects
                });
            }
            catch (Exception ex)
            {
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = ex.Message;
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;
                return StatusCode(500, new
                {
                    Message = "Unable to validate records",
                    status = false,
                    Error = ex.Message
                });
            }
        }

        private BiOtherCost MapDataRowToBiOtherCost(DataRow row)
        {
            try
            {
                var rcmValue = ConvertToBoolean(row["RCM"].ToString().Trim());
                var tdsApplicableValue = ConvertToBoolean(row["TDS Applicable"].ToString().Trim());
                var tdsDeclarationValue = ConvertToBoolean(row["TDS Declaration"].ToString().Trim());
                var budgetedValue = ConvertToBoolean(row["Budgeted"].ToString().Trim());

                return new BiOtherCost
                {
                    Vch = row["VCH"].ToString(),
                    SupplierName = row["Supplier Name"].ToString(),
                    TypeOfExpense = row["Type Of Expense"].ToString(),
                    ServiceLine = row["Service Line"].ToString(),
                    Customer = row["Customer"].ToString(),
                    Description = row["Description"].ToString(),
                    InvoiceNo = row["Invoice No"].ToString(),
                    InvoiceDate = Convert.ToDateTime(row["Invoice Date"]),
                    PoNo = row["PO No"].ToString(),
                    PoDate = row["PO Date"] != DBNull.Value ? Convert.ToDateTime(row["PO Date"]) : (DateTime?)null,
                    Rcm = rcmValue,
                    HsnSac = Convert.ToDecimal(row["HSN SAC"]),
                    Qty = Convert.ToDecimal(row["Quantity"]),
                    Rate = Convert.ToDecimal(row["Rate"]),
                    Value = Convert.ToDecimal(row["Value"]),
                    Fxrate = Convert.ToDecimal(row["FX Rate"]),
                    ValueInr = Convert.ToDecimal(row["Value INR"]),
                    Vat = row["Vat"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["Vat"]) : null,
                    Cgst = row["CGST"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["CGST"]) : null,
                    Sgst = row["SGST"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["SGST"]) : null,
                    Igst = Convert.ToDecimal(row["IGST"]),
                    TotalInvoiceValueInr = Convert.ToDecimal(row["Total Invoice Value INR"]),
                    TdsApplicable = tdsApplicableValue,
                    TdsDeclaration = tdsDeclarationValue,
                    TdsSection = row["TDS Section"].ToString(),
                    TdsRate = Convert.ToDecimal(row["TDS Rate"]),
                    TdsValue = Convert.ToDecimal(row["TDS Value"]),
                    Budgeted = budgetedValue,
                    BudgetedAmount = Convert.ToDecimal(row["Budgeted Amount"]),
                    Variance = row["Variance"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["Variance"]) : null,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = null
                };
            }
            catch (Exception ex)
            {
                // Log exception details here if needed
                throw new Exception("Error mapping DataRow to BiOtherCost: " + ex.Message, ex);
            }
        }

        private bool ConvertToBoolean(string value)
        {
            return value switch
            {
                "Yes" => true,
                "No" => false,
                _ => throw new Exception("Invalid value: " + value),
            };
        }

        [HttpPost("othercost/bulkinsert")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([FromBody] List<BiOtherCost> items)
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
                { "vch", item.Vch },
                { "supplierName", item.SupplierName },
                { "typeOfExpense", item.TypeOfExpense },
                { "serviceLine", item.ServiceLine },
                { "customer", item.Customer },
                { "description", item.Description },
                { "invoiceNo", item.InvoiceNo },
                { "invoiceDate", item.InvoiceDate },
                { "poNo", item.PoNo },
                { "poDate", item.PoDate },
                { "rcm", item.Rcm },
                { "hsnSac", item.HsnSac },
                { "qty", item.Qty },
                { "rate", item.Rate },
                { "value", item.Value },
                { "fxrate", item.Fxrate },
                { "valueInr", item.ValueInr },
                { "vat", item.Vat },
                { "cgst", item.Cgst },
                { "sgst", item.Sgst },
                { "igst", item.Igst },
                { "totalInvoiceValueInr", item.TotalInvoiceValueInr },
                { "tdsApplicable", item.TdsApplicable },
                { "tdsDeclaration", item.TdsDeclaration },
                { "tdsSection", item.TdsSection },
                { "tdsRate", item.TdsRate },
                { "tdsValue", item.TdsValue },
                { "budgeted", item.Budgeted },
                { "budgetedAmount", item.BudgetedAmount },
                { "variance", item.Variance }
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


        [HttpPut("othercost/update")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Update(BiOtherCost item)
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
                return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.InnerException.Message });
            }
        }

        [HttpDelete("othercost/deletebyid/{id}")]
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
                return StatusCode(500, new { Message = "Unable to delete record", status = false, Error = ex.InnerException.Message });
            }
        }

        [HttpPost("othercost/filter")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Filter([FromBody] OtherCostFilter filter)
        {
            try
            {
                var items = await _repository.Filter(filter);
                string msg = items.Any() ? "Data retrieved successfully" : "Data not found";

                _biApiCallLog.ResponseData = msg;
                _biApiCallLog.StatusCode = StatusCodes.Status200OK;

                return Ok(new { Status = true, Data = items, Message = msg });
            }
            catch (Exception ex)
            {
                _biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
                _biApiCallLog.ResponseData = ex.Message;
                _biApiCallLog.Exception = ex.Message + ex.StackTrace;

                return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.InnerException.Message });
            }
        }
    }
}
