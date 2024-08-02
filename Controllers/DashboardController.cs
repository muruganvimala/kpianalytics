using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BusinessIntelligence_API.Controllers
{
	[Route("api/")]
	[ApiController]
	public class DashboardController : BaseController
	{
		private readonly IDashboardRepository _dashboardRepository;

		private readonly JTSContext _jtscontext;

		//Constructor
		public DashboardController(JTSContext jTSContext, IDashboardRepository dashboardRepository, IHttpContextAccessor httpContextAccessor)
		{
			_dashboardRepository = dashboardRepository;
			_jtscontext = jTSContext;
			InitializeApiCallLog(httpContextAccessor);
		}

		//apex chart api
		[HttpGet("dashboard/apexkpi")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Apexkpi(string? PublisherName, string? Metrics, string fromMonthYear, string toMonthYear)
		{
			if (ModelState.IsValid)
			{				
				var data = await _dashboardRepository.ExecuteSSPBIReportAsync(PublisherName, Metrics, fromMonthYear, toMonthYear);				
				// Assuming you want to convert MonthYear to a specific format

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record Inserted Successfully";

				return Ok(new { Message = "Record Inserted Successfully", status = true, Data = data });
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false });
			}
		}

		[HttpGet("dashboard/kpirecentshortfall")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> KpiRecentShortfall(string fromMonthYear, string toMonthYear)
		{
			if (ModelState.IsValid)
			{
				var data = await _dashboardRepository.ExecuteSSPBIReportShortfallsAsync(fromMonthYear, toMonthYear);
				// Assuming you want to convert MonthYear to a specific format

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record Inserted Successfully";

				return Ok(new { Message = "Record Inserted Successfully", status = true, Data = data });
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false });
			}
		}

		[HttpGet("dashboard/underperforming")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getGetUnderperforming(string? variable1, string? variable2, string toporBottom, string startMonthYear, string endMonthYear, string metricorPublisher)
		{
			try
			{
				var  data = await _dashboardRepository.GetUnderperformingServices(variable1, variable2, toporBottom, startMonthYear, endMonthYear, metricorPublisher);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = data });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		//jagdish
		[HttpGet("dashboard/ConsistentlyPerforming")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getConsistentlyPerforming(string top, string sMonthYear, string eMonthYear, string metric)
		{
			try
			{
				List<BiDashboardMetricChart> biDashboardMetricCharts = await _dashboardRepository.GetConsistentlyPerformingServices(top, sMonthYear, eMonthYear, metric);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biDashboardMetricCharts });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		[HttpGet("dashboard/LeadingReliableContributors")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getLeadingReliableContributors(string top, string sMonthYear, string eMonthYear, string Publisher)
		{
			try
			{
				List<BiDashboardChart> biDashboardChart = await _dashboardRepository.GetLeadingReliableContributors(top, sMonthYear, eMonthYear, Publisher);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biDashboardChart });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		[HttpGet("dashboard/UnderperformingPublishers")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getUnderperformingPublishers(string bottom, string sMonthYear, string eMonthYear, string Publisher)
		{
			try
			{
				List<BiDashboardChart> biDashboardChart = await _dashboardRepository.GetUnderperformingPublishers(bottom, sMonthYear, eMonthYear, Publisher);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biDashboardChart });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		[HttpGet("dashboard/UnderPerformingServices")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getUnderPerformingServices(string bottom, string sMonthYear, string eMonthYear, string Metric)
		{
			try
			{
				List<BiDashboardMetricChart> biDashboardChart = await _dashboardRepository.GetUnderPerformingServices(bottom, sMonthYear, eMonthYear, Metric);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biDashboardChart });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		[HttpGet("dashboard/LastMonthChart")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getLastMonthChart(string sMonthyear, string eMonthyear)
		{
			try
			{
				List<BiDashboardMonthyearChart> biDashboardMonthyearCharts = await _dashboardRepository.GetDashboardMonthyear(sMonthyear, eMonthyear);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biDashboardMonthyearCharts });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		[HttpGet("dashboard/Comparisoncharts")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getComparisoncharts(string sMonthyear, string eMonthyear)
		{
			try
			{
				List<BiDashboardMonthyearChart> frombiDashboardMonthyearCharts = await _dashboardRepository.GetDashboardMonthyear(sMonthyear, sMonthyear);

				List<BiDashboardMonthyearChart> tobiDashboardMonthyearCharts = await _dashboardRepository.GetDashboardMonthyear(eMonthyear, eMonthyear);

				var Comparisonchartdata = frombiDashboardMonthyearCharts.Concat(tobiDashboardMonthyearCharts);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = Comparisonchartdata });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while comparison chart data";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		//finance
		//[HttpPost("dashboard/financechart1")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> GetFinanceChart1([FromBody] FinanceChartRequest request, string chartType)
		//{
		//	try
		//	{
		//		List<FinanceChartResponse> biDashboardMonthyearCharts = await _dashboardRepository.GetDashboardFinance(
		//		  request.Heading,
		//		  request.ServiceLine,
		//		  request.Customers,
		//		  request.StartMonth,
		//		  request.EndMonth
		//		);

		//		if (chartType == "pie" || chartType == "pyramid" || chartType == "bar")
		//		{
		//			var pieChartData = biDashboardMonthyearCharts
		//			  .GroupBy(x => x.ServiceLine)
		//			  .Select(g => new {
		//				  ServiceLine = g.Key,
		//				  CostCtcSum = g.Sum(x => x.CostCtc) // Convert default value to decimal
		//			  })
		//			  .ToList();

		//			var totalCostCtc = pieChartData.Sum(x => x.CostCtcSum);

		//			var labels = pieChartData.Select(x => x.ServiceLine).ToArray();
		//			var series = pieChartData.Select(x => totalCostCtc > 0 ? (int)((x.CostCtcSum / totalCostCtc) * 100) : 0).ToArray();

		//			return Ok(new
		//			{
		//				Status = true,
		//				Data = new
		//				{
		//					Labels = labels,
		//					Series = series
		//				}
		//			});
		//		}
		//		else if (chartType == "bar")
		//		{

		//			var barChartData = biDashboardMonthyearCharts
		//			  .GroupBy(x => x.ServiceLine)
		//			  .Select(g => new {
		//				  ServiceLine = g.Key,
		//				  CostCtc = g.Sum(x => x.CostCtc)
		//			  })
		//			  .ToList();

		//			var labels = barChartData.Select(x => x.ServiceLine).ToArray();
		//			var series = barChartData.Select(x => x.CostCtc).ToArray();

		//			return Ok(new
		//			{
		//				Status = true,
		//				Data = new
		//				{
		//					Labels = labels,
		//					Series = series
		//				}
		//			});
		//		}

		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = "Data retrieved successfully";

		//		return Ok(new
		//		{
		//			Status = true,
		//			Data = biDashboardMonthyearCharts
		//		});
		//	}
		//	catch (Exception ex)
		//	{
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

		//		return StatusCode(500, new
		//		{
		//			Message = "An error occurred while fetching publishers.",
		//			Status = false,
		//			Error = ex.Message
		//		});
		//	}
		//}


		//	[HttpPost("dashboard/financecharttest1")]
		//	[Authorize(Roles = "User")]
		//	public async Task<IActionResult> GetFinancetestChart1([FromBody] FinanceChartRequest request, string chartType)
		//	{
		//		try
		//		{
		//			if (request.ServiceLine.Contains("All"))
		//			{
		//				request.ServiceLine = new[]
		//				{
		//	"Content Composition",
		//	"Multimedia Solutions",
		//	"Digital Transformation",
		//	"Editorial Solutions",
		//	"Project Management"
		//};
		//			}
		//			List<ServiceResult> biDashboardMonthyearCharts = await _dashboardRepository.GetFinanceServiceData1(
		//			  request.Heading,
		//			  request.ServiceLine,
		//			  request.Customers,
		//			  request.StartMonth,
		//			  request.EndMonth
		//			);

		//			var labels = biDashboardMonthyearCharts.Select(x => x.Labels).ToArray();
		//			var series = biDashboardMonthyearCharts.Select(x => x.Series).ToArray();

		//			_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//			_biApiCallLog.ResponseData = "Data retrieved successfully";

		//			return Ok(new
		//			{
		//				Status = true,
		//				Data = new
		//				{
		//					Labels = labels,
		//					Series = series
		//				}
		//			});



		//		}
		//		catch (Exception ex)
		//		{
		//			_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//			_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

		//			return StatusCode(500, new
		//			{
		//				Message = "An error occurred while fetching publishers.",
		//				Status = false,
		//				Error = ex.Message
		//			});
		//		}
		//	}

		[HttpPost("dashboard/financechart1")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetFinanceChartLive1([FromBody] FinanceChartRequest request, string chartType)
		{
			try
			{
				DirectCostParam directCostParam = new DirectCostParam
				{
					heading= request.Heading,
					serviceLine = request.ServiceLine,
					customerName = request.Customers,
					valueType = request.ValueType,
					startMonth = request.StartMonth,
					endMonth = request.EndMonth
				};

				List<BiFinDashReportResponse> biDashboardMonthyearCharts = await _dashboardRepository.GetFinDashReportProcedure1(directCostParam);
				var labels = biDashboardMonthyearCharts.Select(x => x.ServiceLine).ToArray();
				var series = biDashboardMonthyearCharts.Select(x => x.PercentageCostCtc).ToArray();

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";

				return Ok(new
				{
					Status = true,
					Data = new
					{
						Labels = labels,
						Series = series
					}
				});

			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

				return StatusCode(500, new
				{
					Message = "An error occurred while fetching publishers.",
					Status = false,
					Error = ex.Message
				});
			}
		}

		[HttpPost("dashboard/financechart2")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetFinanceChartLive2([FromBody] FinanceChartRequest request, string chartType)
		{
			try
			{
				if (request.ServiceLine.Contains("All"))
				{
					request.ServiceLine = "Content Composition,Multimedia Solutions,Digital Transformation,Editorial Solutions,Project Management";
				}

				DirectCostParam directCostParam = new DirectCostParam
				{
					serviceLine = request.ServiceLine,
					customerName = request.Customers,
					valueType = request.ValueType,
					startMonth = request.StartMonth,
					endMonth = request.EndMonth
				};

				List<BiFinDashReportResponse> biDashboardMonthyearCharts = await _dashboardRepository.GetFinDashReportProcedure2(directCostParam);
				var labels = biDashboardMonthyearCharts.Select(x => x.ServiceLine).ToArray();
				var series = biDashboardMonthyearCharts.Select(x => x.TotalCostCtc).ToArray();

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";

				return Ok(new
				{
					Status = true,
					Data = new
					{
						Labels = labels,
						Series = series
					}
				});

			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

				return StatusCode(500, new
				{
					Message = "An error occurred while fetching publishers.",
					Status = false,
					Error = ex.Message
				});
			}
		}

		[HttpPost("dashboard/financecharttop10customer")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetFinanceChartLiveTop10customer([FromBody] FinanceChartRequest request, string chartType)
		{
			try
			{
				if (request.ServiceLine.Contains("All"))
				{
					request.ServiceLine = "Content Composition,Multimedia Solutions,Digital Transformation,Editorial Solutions,Project Management";
				}

				DirectCostParam directCostParam = new DirectCostParam
				{
					serviceLine = request.ServiceLine,
					customerName = request.Customers,
					valueType = request.ValueType,
					startMonth = request.StartMonth,
					endMonth = request.EndMonth
				};

				List<BiFinDashReportResponse> biDashboardMonthyearCharts = await _dashboardRepository.GetFinDashReporttop10customer(directCostParam);
				var labels = biDashboardMonthyearCharts.Select(x => x.ServiceLine).ToArray();
				var series = biDashboardMonthyearCharts.Select(x => x.PercentageCostCtc).ToArray();

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";

				return Ok(new
				{
					Status = true,
					Data = new
					{
						Labels = labels,
						Series = series
					}
				});

			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

				return StatusCode(500, new
				{
					Message = "An error occurred while fetching publishers.",
					Status = false,
					Error = ex.Message
				});
			}
		}

		[HttpPost("dashboard/financechartexistingandnewdata")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetFinanceChartLiveExistingAndNewData([FromBody] FinanceChartRequest request, string chartType)
		{
			try
			{
				if (request.ServiceLine.Contains("All"))
				{
					request.ServiceLine = "Content Composition,Multimedia Solutions,Digital Transformation,Editorial Solutions,Project Management";
				}

				DirectCostParam directCostParam = new DirectCostParam
				{
					serviceLine = request.ServiceLine,
					customerName = request.Customers,
					valueType = request.ValueType,
					startMonth = request.StartMonth,
					endMonth = request.EndMonth
				};

				List<BiFinDashReportBarChartDataResponse> biDashboardMonthyearCharts = await _dashboardRepository.GetFinDashReportExistingAndNewData(directCostParam);
				//var labels = biDashboardMonthyearCharts.Select(x => x.ServiceLine).ToArray();
				//var series = biDashboardMonthyearCharts.Select(x => x.PercentageCostCtc).ToArray();
				var labels = new List<string> { "Existing", "New" };
				var series = new List<long>{
					biDashboardMonthyearCharts.Sum(x => x.existingData),
					biDashboardMonthyearCharts.Sum(x => x.newData)
				};

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";

				return Ok(new
				{
					Status = true,
					Data = new
					{
						Labels = labels,
						Series = series
					}
				});

			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

				return StatusCode(500, new
				{
					Message = "An error occurred while fetching publishers.",
					Status = false,
					Error = ex.Message
				});
			}
		}


		[HttpGet("dashboard/MetricsWisePublishers")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getMetricsWisePublishers(string metrics, string top, string sMonthYear, string eMonthYear, string Publisher)
		{
			try
			{
				List<BiDashboardChart> biDashboardChart = await _dashboardRepository.GetMatrixwisePublishers(metrics, top, sMonthYear, eMonthYear, Publisher);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = biDashboardChart });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "An error occurred while fetching publishers";
				return StatusCode(500, new { Message = "An error occurred while fetching publishers.", Status = false, Error = ex.Message });
			}
		}

		//	[HttpPost("dashboard/financecharttest2")]
		//	[Authorize(Roles = "User")]
		//	public async Task<IActionResult> GetFinancetestChart2([FromBody] FinanceChartRequest request, string chartType)
		//	{
		//		try
		//		{
		//			if (request.ServiceLine.Contains("All"))
		//			{
		//				request.ServiceLine = new[]
		//				{
		//	"Content Composition",
		//	"Multimedia Solutions",
		//	"Digital Transformation",
		//	"Editorial Solutions",
		//	"Project Management"
		//};
		//			}

		//			List<ServiceResult> biDashboardMonthyearCharts = await _dashboardRepository.GetFinanceServiceData2(
		//			  request.Heading,
		//			  request.ServiceLine,
		//			  request.Customers,
		//			  request.StartMonth,
		//			  request.EndMonth
		//			);

		//			var labels = biDashboardMonthyearCharts.Select(x => x.Labels).ToArray();
		//			var series = biDashboardMonthyearCharts.Select(x => x.Series).ToArray();

		//			_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//			_biApiCallLog.ResponseData = "Data retrieved successfully";

		//			return Ok(new
		//			{
		//				Status = true,
		//				Data = new
		//				{
		//					Labels = labels,
		//					Series = series
		//				}
		//			});

		//		}
		//		catch (Exception ex)
		//		{
		//			_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//			_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

		//			return StatusCode(500, new
		//			{
		//				Message = "An error occurred while fetching publishers.",
		//				Status = false,
		//				Error = ex.Message
		//			});
		//		}
		//	}

		//[HttpPost("dashboard/financechart2")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> GetFinanceChart2([FromBody] FinanceChartRequest request, string chartType)
		//{
		//	try
		//	{
		//		List<FinanceChartResponse> biDashboardMonthyearCharts = await _dashboardRepository.GetDashboardFinance(
		//		  request.Heading,
		//		  request.ServiceLine,
		//		  request.Customers,
		//		  request.StartMonth,
		//		  request.EndMonth
		//		);

		//		var barChartData = biDashboardMonthyearCharts
		//			  .GroupBy(x => x.ServiceLine)
		//			  .Select(g => new {
		//				  ServiceLine = g.Key,
		//				  CostCtc = g.Sum(x => x.CostCtc)
		//			  })
		//			  .ToList();

		//		var labels = barChartData.Select(x => x.ServiceLine).ToArray();
		//		var series = barChartData.Select(x => x.CostCtc).ToArray();

		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = "Data retrieved successfully";

		//		return Ok(new
		//		{
		//			Status = true,
		//			Data = new
		//			{
		//				Labels = labels,
		//				Series = series
		//			}
		//		});


		//	}
		//	catch (Exception ex)
		//	{
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = "An error occurred while fetching publishers";

		//		return StatusCode(500, new
		//		{
		//			Message = "An error occurred while fetching publishers.",
		//			Status = false,
		//			Error = ex.Message
		//		});
		//	}
		//}


	}
}
