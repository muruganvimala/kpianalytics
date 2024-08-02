using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using BusinessIntelligence_API.Service;
using System.Security.Cryptography.Xml;
using static BusinessIntelligence_API.Mapping.MappingProfile;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;

namespace BusinessIntelligence_API.Repository
{
	public class DashboardRepository : IDashboardRepository
	{
		private readonly JTSContext _context;
        public DashboardRepository()
        {
			_context = new JTSContext();
        }	

		public async Task<List<SSP_BIReport_DetailedResult>> ExecuteSSPBIReportAsync(string publishername, string metrics, string Frommonthyear, string Tomonthyear)
		{
			var data= await _context.SSP_BIReportEntities.FromSqlInterpolated($"EXEC SSP_BIReport_Detailed {publishername}, {metrics}, {Frommonthyear}, {Tomonthyear}").ToListAsync();
			var formatedData = new List<SSP_BIReport_DetailedResult>();

			foreach (var item in data)
			{
				formatedData.Add(new SSP_BIReport_DetailedResult
				{
					PublisherName = item.PublisherName,
					Metrics = item.Metrics,
					MonthYear = item.MonthYear,
					Target = item.Target,
					Actual = item.Actual
				});
			}
			return formatedData;
		}

		

		public async Task<List<SSP_BIReportFormattedData>> ExecuteSSPBIReportShortfallsAsync(string Frommonthyear, string Tomonthyear)
		{
			var data = await _context.SSP_BIReportShortfallEntities.FromSqlInterpolated($"EXEC SSP_BIReport {Frommonthyear}, {Tomonthyear}").ToListAsync();

			var publisherCounts = data.GroupBy(x => x.PublisherName).Select(group => new { PublisherName = group.Key, Count = group.Count() }).ToList();
			var formattedData = new SSP_BIReportFormattedData
			{
				Metrics = data.Select(x => x.Metrics).ToList(),
				Target = data.Select(x => (decimal)x.Target).ToList(),
				Actual = data.Select(x => (decimal?)x.Actual).ToList(),
				BiGroups = publisherCounts.Select(publisher => new BiGroup { Title = publisher.PublisherName, Cols = publisher.Count }).ToList()
			};

			var resultList = new List<SSP_BIReportFormattedData>();
			resultList.Add(formattedData);

			return resultList;
		}

		public async Task<List<BiUnderperforming>> GetUnderperformingServices(string variable1, string variable2, string toporbottom, string sMonthYear, string eMonthYear, string metric)
		{
			var data =  await _context.SSP_BIUnderPerformingEntities.FromSqlInterpolated($"EXEC SSP_BIReport_Common {variable1}, {variable2},{toporbottom},{sMonthYear},{eMonthYear},{metric}").ToListAsync();
			return data.OrderBy(item => item.Count).ToList();

		}

		public async Task<List<BiDashboardMetricChart>> GetConsistentlyPerformingServices(string top, string sMonthYear, string eMonthYear, string metric)
		{
			List<BiDashboardMetricChart> biDashboardMetricCharts = new List<BiDashboardMetricChart>();
			var MetricChart = await _context.GetConsistentlyPerformingServices("", "", top, sMonthYear, eMonthYear, metric);
			if (MetricChart != null)
			{
				foreach (var item in MetricChart)
				{
					var userDto = new BiDashboardMetricChart
					{
						Metrics = item.Metrics,
						Count = item.Count
					};
					biDashboardMetricCharts.Add(userDto);
				}

			}
			return biDashboardMetricCharts;
		}

		public async Task<List<BiDashboardMonthyearChart>> GetDashboardMonthyear(string sMonthYear, string eMonthYear)
		{
			List<BiDashboardMonthyearChart> BiDashboardMonthyearChart = new List<BiDashboardMonthyearChart>();

			var MetricChart = await _context.GetMonthYearChart(sMonthYear, eMonthYear);
			if (MetricChart != null)
			{
				foreach (var item in MetricChart)
				{

					var dashboarddata = new BiDashboardMonthyearChart
					{
						PublisherName = item.PublisherName,
						Metrics = item.Metrics,
						MonthYear = item.MonthYear,
						Target = item.Target,
						Actual = item.Actual,
						Percentage = item.Percentage
				};
					BiDashboardMonthyearChart.Add(dashboarddata);

				}

			}
			return BiDashboardMonthyearChart;

		}


		public async Task<List<BiDashboardChart>> GetLeadingReliableContributors(string top, string sMonthYear, string eMonthYear, string publisher)
		{
			List<BiDashboardChart> biBiDashboardChart = new List<BiDashboardChart>();
			var MetricChart = await _context.GetLeadingReliableContributors("", "", top, sMonthYear, eMonthYear, publisher);
			if (MetricChart != null)
			{
				foreach (var item in MetricChart)
				{
					var userDto = new BiDashboardChart
					{
						PublisherName = item.PublisherName,
						Count = item.Count
					};
					biBiDashboardChart.Add(userDto);
				}

			}
			return biBiDashboardChart;
		}

		public async Task<List<BiDashboardChart>> GetUnderperformingPublishers(string Bottom, string sMonthYear, string eMonthYear, string publisher)
		{
			List<BiDashboardChart> biBiDashboardChart = new List<BiDashboardChart>();
			var MetricChart = await _context.GetUnderperformingPublishers("", "", Bottom, sMonthYear, eMonthYear, publisher);
			if (MetricChart != null)
			{
				foreach (var item in MetricChart)
				{
					var userDto = new BiDashboardChart
					{
						PublisherName = item.PublisherName,
						Count = item.Count
					};
					biBiDashboardChart.Add(userDto);
				}

			}
			return biBiDashboardChart;
		}

		public async Task<List<BiDashboardMetricChart>> GetUnderPerformingServices(string Bottom, string sMonthYear, string eMonthYear, string metric)
		{
			List<BiDashboardMetricChart> biDashboardMetricCharts = new List<BiDashboardMetricChart>();
			var MetricChart = await _context.GetUnderperformingServices("", "", Bottom, sMonthYear, eMonthYear, metric);
			if (MetricChart != null)
			{
				foreach (var item in MetricChart)
				{
					var userDto = new BiDashboardMetricChart
					{
						Metrics = item.Metrics,
						Count = item.Count
					};
					biDashboardMetricCharts.Add(userDto);
				}

			}
			return biDashboardMetricCharts;
		}

		//finance
		//public async Task<List<FinanceChartResponse>> GetDashboardFinance(string[] heading, string[] serviceline, string customers, string startMonth, string endMonth)
		//{
		//	List<BiDirectCost> data = new List<BiDirectCost>();
		//	List<FinanceChartResponse> mappedData = new List<FinanceChartResponse>();

		//	if (heading.Contains("Direct Cost") && customers.ToLower() == "all")
		//	{
		//		DateTime startDate = DateTime.ParseExact(startMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
		//		DateTime endDate = DateTime.ParseExact(endMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

		//		// Fetch the data from the database without the date filter
		//		var allData = await _context.BiDirectCosts
		//			.Where(x => serviceline.Contains(x.ServiceLine))
		//			.ToListAsync();

		//		// Perform the date filtering in memory
		//		data = allData
		//			.Where(x => DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) >= startDate &&
		//						DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) <= endDate)
		//			.ToList();

		//		//mapping
		//		mappedData = data.Select(x => new FinanceChartResponse
		//		{
		//			Year = x.Year,
		//			ServiceLine = x.ServiceLine,
		//			Customer = x.Customer,
		//			Fc = x.Fc,
		//			FxRate = x.FxRate,
		//			CostCtc = x.CostCtc
		//		}).ToList();
		//	}
		//	else if (heading.Contains("Indirect Cost") && customers.ToLower() == "all")
		//	{
		//		DateTime startDate = DateTime.ParseExact(startMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
		//		DateTime endDate = DateTime.ParseExact(endMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

		//		// Fetch the data from the database without the date filter
		//		var allData = await _context.BiIndirectLabourCosts
		//			.Where(x => serviceline.Contains(x.Department))
		//			.ToListAsync();

		//		// Perform the date filtering in memory
		//		//data = allData
		//		//	.Where(x => DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) >= startDate &&
		//		//				DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) <= endDate)
		//		//	.ToList();
		//	}
		//	else if (heading.Contains("Indirect Cost") && customers.ToLower() == "all")
		//	{
		//		DateTime startDate = DateTime.ParseExact(startMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
		//		DateTime endDate = DateTime.ParseExact(endMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

		//		// Fetch the data from the database without the date filter
		//		var allData = await _context.BiIndirectLabourCosts
		//			.Where(x => serviceline.Contains(x.Department))
		//			.ToListAsync();

		//		// Perform the date filtering in memory
		//		//data = allData
		//		//	.Where(x => DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) >= startDate &&
		//		//				DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) <= endDate)
		//		//	.ToList();
		//	}


		//	return mappedData;
		//}


		//public async Task<List<FinanceChartResponse>> GetDashboardFinancebk(string[] heading, string[] serviceline, string customers, string startMonth, string endMonth)
		//{
		//	List<BiDirectCost> data = new List<BiDirectCost>();
		//	List<FinanceChartResponse> mappedData = new List<FinanceChartResponse>();

		//	if (heading.Contains("Direct Cost") && customers.ToLower() == "all")
		//	{
		//		DateTime startDate = DateTime.ParseExact(startMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
		//		DateTime endDate = DateTime.ParseExact(endMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

		//		// Fetch the data from the database without the date filter
		//		var allData = await _context.BiDirectCosts
		//			.Where(x => serviceline.Contains(x.ServiceLine))
		//			.ToListAsync();

		//		// Perform the date filtering in memory
		//		data = allData
		//			.Where(x => DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) >= startDate &&
		//						DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) <= endDate)
		//			.ToList();

		//		//mapping
		//		mappedData = data.Select(x => new FinanceChartResponse
		//		{
		//			Year = x.Year,
		//			ServiceLine = x.ServiceLine,
		//			Customer = x.Customer,
		//			Fc = x.Fc,
		//			FxRate = x.FxRate,
		//			CostCtc = x.CostCtc
		//		}).ToList();
		//	}
		//	else if (heading.Contains("Indirect Cost") && customers.ToLower() == "all")
		//	{
		//		DateTime startDate = DateTime.ParseExact(startMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
		//		DateTime endDate = DateTime.ParseExact(endMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

		//		// Fetch the data from the database without the date filter
		//		var allData = await _context.BiIndirectLabourCosts
		//			.Where(x => serviceline.Contains(x.Department))
		//			.ToListAsync();

		//		// Perform the date filtering in memory
		//		//data = allData
		//		//	.Where(x => DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) >= startDate &&
		//		//				DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) <= endDate)
		//		//	.ToList();
		//	}
		//	else if (heading.Contains("Indirect Cost") && customers.ToLower() == "all")
		//	{
		//		DateTime startDate = DateTime.ParseExact(startMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
		//		DateTime endDate = DateTime.ParseExact(endMonth, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

		//		// Fetch the data from the database without the date filter
		//		var allData = await _context.BiIndirectLabourCosts
		//			.Where(x => serviceline.Contains(x.Department))
		//			.ToListAsync();

		//		// Perform the date filtering in memory
		//		//data = allData
		//		//	.Where(x => DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) >= startDate &&
		//		//				DateTime.ParseExact(x.Year, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) <= endDate)
		//		//	.ToList();
		//	}


		//	return mappedData;
		//}

//		public async Task<List<ServiceResult>> GetFinanceServiceData1(string heading, string serviceline, string customers, string startMonth, string endMonth)
//		{
//			List<ServiceData> DirectCost = new List<ServiceData>()
//{
//    new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 18 },
//    new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 24 },
//    new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 30 },
//    new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 33 },
//    new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 45 },
//    new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 22 },
//    new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 33 },
//    new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 34 },
//    new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 33 },
//    new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 36 },
//    new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 43 },
//    new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 34 },
//    new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 23 },
//    new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 33 },
//    new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 34 }
//};

//			List<ServiceData> IndirectCost = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 27 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 19 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 32 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 44 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 25 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 28 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 37 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 21 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 29 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 38 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 31 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 24 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 39 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 22 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 41 }
//};

//			List<ServiceData>OtherCost = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 37 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 21 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 28 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 44 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 30 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 25 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 39 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 32 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 29 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 40 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 27 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 22 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 38 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 26 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 35 }
//};
//			List<ServiceData> CustomerData = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 27 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 19 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 35 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 42 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 37 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 30 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 28 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 41 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 24 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 39 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 21 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 36 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 29 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 38 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 33 }
//};
//			List<ServiceData> serviceDataList5 = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 22 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 34 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 33 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 36 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 30 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 45 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 34 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 33 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 30 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 43 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 33 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 36 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 34 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 43 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 24 }
//};

			

//			List<ServiceData> result = new List<ServiceData>();
//			foreach (var item in heading)
//			{
//				List<ServiceData> data = null;
//				switch (item)
//				{
//					case "Direct Cost":
//						data = DirectCost;
//						break;
//					case "Indirect Cost":
//						data = IndirectCost;
//						break;
//					case "Other Cost":
//						data = OtherCost;
//						break;
//					case "Customer Data":
//						data = CustomerData;
//						break;
//				}
//				if (data != null)
//				{
//					foreach (var serviceLine in serviceline)
//					{
//						var filteredData = data.Where(d => CompareMonths(d.Year, startMonth) <= CompareMonths(d.Year, endMonth) && d.ServiceLine == serviceLine);
//						result.AddRange(filteredData);
//					}
//				}

//			}

//			//average value based on serviceLine
//			var averageData = result.GroupBy(d => d.ServiceLine)
//						   .Select(group => new ServiceResult
//						   {
//							   Labels = group.Key,
//							   Series = (int)group.Average(d => d.Series)
//						   })
//						   .ToList();
//			return averageData;

//		}

//		public async Task<List<ServiceResult>> GetFinanceServiceData2(string[] heading, string[] serviceline, string customers, string startMonth, string endMonth)
//		{
//			List<ServiceData> DirectCost = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 100000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 110000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 203030 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 303033 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 330405 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 230302 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 300003 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 300004 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 300003 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 300006 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 400003 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 300004 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 200003 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 300003 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 300004 }
//};

//			List<ServiceData> IndirectCost = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 200007 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 100009 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 300002 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 400004 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 200005 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 200008 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 300007 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 200001 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 200009 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 300008 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 300001 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 240000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 39000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 200002 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 400001 }
//};

//			List<ServiceData> OtherCost = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 370000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 210000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 280000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 44000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 300000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 25000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 390000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 32000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 290000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 400000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 200007 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 200002 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 300008 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 200006 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 350000 }
//};
//			List<ServiceData> CustomerData = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 270000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 100009 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 300005 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 400002 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 300007 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 300000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 200008 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 400001 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 200004 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 390000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 210000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 360000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 200009 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 300008 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 300003 }
//};
//			List<ServiceData> serviceDataList5 = new List<ServiceData>()
//{
//	new ServiceData { Year = "Apr-24", ServiceLine = "Content Composition", Series = 220000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Editorial Solutions", Series = 340000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Multimedia Solutions", Series = 330000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Digital Transformation", Series = 360000 },
//	new ServiceData { Year = "Apr-24", ServiceLine = "Project Management", Series = 300000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Content Composition", Series = 450000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Editorial Solutions", Series = 340000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Multimedia Solutions", Series = 330000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Digital Transformation", Series = 300000 },
//	new ServiceData { Year = "May-24", ServiceLine = "Project Management", Series = 430000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Content Composition", Series = 330000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Editorial Solutions", Series = 360000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Multimedia Solutions", Series = 340000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Digital Transformation", Series = 430000 },
//	new ServiceData { Year = "Sep-24", ServiceLine = "Project Management", Series = 240000 }
//};



//			List<ServiceData> result = new List<ServiceData>();
//			foreach (var item in heading)
//			{
//				List<ServiceData> data = null;
//				switch (item)
//				{
//					case "Direct Cost":
//						data = DirectCost;
//						break;
//					case "Indirect Cost":
//						data = IndirectCost;
//						break;
//					case "Other Cost":
//						data = OtherCost;
//						break;
//					case "Customer Data":
//						data = CustomerData;
//						break;
//				}
//				if (data != null)
//				{
//					foreach (var serviceLine in serviceline)
//					{
//						var filteredData = data.Where(d => CompareMonths(d.Year, startMonth) <= CompareMonths(d.Year, endMonth) && d.ServiceLine == serviceLine);
//						result.AddRange(filteredData);
//					}
//				}

//			}

//			//average value based on serviceLine
//			var averageData = result.GroupBy(d => d.ServiceLine)
//						   .Select(group => new ServiceResult
//						   {
//							   Labels = group.Key,
//							   Series = (int)group.Average(d => d.Series)
//						   })
//						   .ToList();
//			return averageData;

//		}

		private int CompareMonths(string year, string month)
		{
			string[] monthAbbreviations = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

			var yearParts = year.Split('-');
			var monthIndex = Array.IndexOf(monthAbbreviations, month.Substring(0, 3));

			if (yearParts.Length == 2 && monthIndex >= 0)
			{
				var yearNumber = int.Parse(yearParts[1]);
				var monthNumber = Array.IndexOf(monthAbbreviations, month.Substring(0, 3)) + 1;

				return yearNumber * 12 + monthNumber;
			}

			return -1;
		}

		public async Task<List<BiFinDashReportResponse>> GetFinDashReportProcedure1(DirectCostParam param)
		{
			List<BiFinDashReportResponse> biBiDashboardChart = new List<BiFinDashReportResponse>();

			var allCosts = new List<dynamic>();
			var headerKey = new Dictionary<string, bool>();

			string[] headerArray = { "direct cost", "indirect cost", "other cost", "customer data" };
			if (param.heading == null || param.heading.ToLower() == "all")
			{
				headerKey.Add("direct cost", true);
				headerKey.Add("indirect cost", true);
				headerKey.Add("other cost", true);
				headerKey.Add("customer data", true);
			}
			else
			{
				if (param.heading.Split(',').Count()>0) {
					foreach (var item in param.heading.Split(','))
					{
						headerKey.Add(item.ToLower(), true);
					}
				}
			}

			foreach(var item in headerKey) {
				if (item.Key == "direct cost")
				{
					var DirectCostData = await _context.GetFinDashDirectCostReportProcedure(param);
					if (DirectCostData != null)
					{
						var directGroupedCosts = DirectCostData.Select(dc => new
						{
							Service_Line = dc.ServiceLine,
							Total_Cost_CTC = dc.CostCtc
						});
						allCosts.AddRange(directGroupedCosts);
					}
				}
				else if (item.Key == "indirect cost")
				{ 
				}
				else if (item.Key == "other cost")
				{
					var OtherCostData = await _context.GetFinOtherCostReportProcedure(param);
					if (OtherCostData != null)
					{
						var otherGroupedCosts = OtherCostData.Select(oc => new
						{
							Service_Line = oc.ServiceLine,
							Total_Cost_CTC = oc.TotalInvoiceValueInr
						});
						allCosts.AddRange(otherGroupedCosts);
					}

				}
				else if (item.Key == "customer data")
				{
					var CustomerData = await _context.GetFinCustomerDataReportProcedure(param);
					if (CustomerData != null)
					{
						var customerGroupedCosts = CustomerData.Select(cd => new
						{
							Service_Line = cd.MajorHeadServiceLine,
							Total_Cost_CTC = cd.CollectionValueInr
						});
						allCosts.AddRange(customerGroupedCosts);
					}
				}
			}

			var groupedCosts = allCosts.GroupBy(ac => ac.Service_Line).Select(g => new
			{
				Service_Line = g.Key,
				Total_Cost_CTC = g.Sum(ac => (decimal)ac.Total_Cost_CTC)
			}).ToList();

			decimal overallTotalCost = groupedCosts.Sum(g => g.Total_Cost_CTC);

			var percentageCosts = groupedCosts.Select(g => new
			{
				g.Service_Line,
				Percentage_Cost_CTC = Math.Round((g.Total_Cost_CTC / overallTotalCost) * 100)
			}).ToList();

			biBiDashboardChart = percentageCosts.Select(pc => new BiFinDashReportResponse
			{
				ServiceLine = pc.Service_Line,
				PercentageCostCtc = (long)pc.Percentage_Cost_CTC
			}).ToList();

			return biBiDashboardChart;
		}

		public async Task<List<BiFinDashReportResponse>> GetFinDashReportProcedure2(DirectCostParam param)
		{
			List<BiFinDashReportResponse> biBiDashboardChart = new List<BiFinDashReportResponse>();

			var allCosts = new List<dynamic>();
			var headerKey = new Dictionary<string, bool>();

			string[] headerArray = { "direct cost", "indirect cost", "other cost", "customer data" };
			if (param.heading == null || param.heading.ToLower() == "all")
			{
				headerKey.Add("direct cost", true);
				headerKey.Add("indirect cost", true);
				headerKey.Add("other cost", true);
				headerKey.Add("customer data", true);
			}
			else
			{
				if (param.heading.Split(',').Count() > 0)
				{
					foreach (var item in param.heading.Split(','))
					{
						headerKey.Add(item.ToLower(), true);
					}
				}
			}

			foreach (var item in headerKey)
			{
				if (item.Key == "direct cost")
				{
					var DirectCostData = await _context.GetFinDashDirectCostReportProcedure(param);
					if (DirectCostData != null)
					{
						var directGroupedCosts = DirectCostData.Select(dc => new
						{
							Service_Line = dc.ServiceLine,
							Total_Cost_CTC = dc.CostCtc
						});
						allCosts.AddRange(directGroupedCosts);
					}
				}
				else if (item.Key == "indirect cost")
				{
				}
				else if (item.Key == "other cost")
				{
					var OtherCostData = await _context.GetFinOtherCostReportProcedure(param);
					if (OtherCostData != null)
					{
						var otherGroupedCosts = OtherCostData.Select(oc => new
						{
							Service_Line = oc.ServiceLine,
							Total_Cost_CTC = oc.TotalInvoiceValueInr
						});
						allCosts.AddRange(otherGroupedCosts);
					}

				}
				else if (item.Key == "customer data")
				{
					var CustomerData = await _context.GetFinCustomerDataReportProcedure(param);
					if (CustomerData != null)
					{
						var customerGroupedCosts = CustomerData.Select(cd => new
						{
							Service_Line = cd.MajorHeadServiceLine,
							Total_Cost_CTC = cd.CollectionValueInr
						});
						allCosts.AddRange(customerGroupedCosts);
					}
				}
			}

			var groupedCosts = allCosts.GroupBy(ac => ac.Service_Line).Select(g => new
			{
				Service_Line = g.Key,
				Total_Cost_CTC = g.Sum(ac => (decimal)ac.Total_Cost_CTC)
			}).ToList();

			biBiDashboardChart = groupedCosts.Select(pc => new BiFinDashReportResponse
			{
				ServiceLine = pc.Service_Line,
				TotalCostCtc = (long)pc.Total_Cost_CTC
			}).ToList();

			return biBiDashboardChart;
		}

		public async Task<List<BiFinDashReportResponse>> GetFinDashReporttop10customer(DirectCostParam param)
		{
			List<BiFinDashReportResponse> biBiDashboardChart = new List<BiFinDashReportResponse>();

			var allTopCustomers = new List<TopCustomer>();
			var headerKey = new Dictionary<string,
			  bool>();

			string[] headerArray = {"direct cost","indirect cost","other cost","customer data"};
			if (param.heading == null || param.heading.ToLower() == "all")
			{
				headerKey.Add("direct cost", true);
				headerKey.Add("indirect cost", true);
				headerKey.Add("other cost", true);
				headerKey.Add("customer data", true);
			}
			else
			{
				if (param.heading.Split(',').Count() > 0)
				{
					foreach (var item in param.heading.Split(','))
					{
						headerKey.Add(item.ToLower(), true);
					}
				}
			}

			foreach (var item in headerKey)
			{
				if (item.Key == "direct cost")
				{					
					var DirectCostData = await _context.GetFinDashDirectCostReportProcedure(param);
					if (DirectCostData != null)
					{
						var topCustomers = DirectCostData.GroupBy(i => i.Customer)
						  .Select(g => new TopCustomer
						  {
							  Customer = g.Key,
							  TotalInvoiceValue = g.Sum(i => i.CostCtc)
						  })
						  .OrderByDescending(c => c.TotalInvoiceValue)
						  .Take(10)
						  .ToList();

						var publisherData = await _context.BiPublishers.ToListAsync();
						foreach (var data in topCustomers)
						{
							var currentPublisher = publisherData.Where(x => x.PublisherName == data.Customer).FirstOrDefault();
							if (currentPublisher != null)
							{
								data.Customer = currentPublisher.Acronym;
							}
						}

						allTopCustomers.AddRange(topCustomers);
					}
				}
				else if (item.Key == "indirect cost") { }
				else if (item.Key == "other cost")
				{
					var OtherCostData = await _context.GetFinOtherCostReportProcedure(param);
					if (OtherCostData != null)
					{
						var topCustomers = OtherCostData.GroupBy(i => i.Customer)
						  .Select(g => new TopCustomer
						  {
							  Customer = g.Key,
							  TotalInvoiceValue = g.Sum(i => i.TotalInvoiceValueInr)
						  })
						  .OrderByDescending(c => c.TotalInvoiceValue)
						  .Take(10)
						  .ToList();

						allTopCustomers.AddRange(topCustomers);

					}

				}
				else if (item.Key == "customer data")
				{
					var CustomerData = await _context.GetFinCustomerDataReportProcedure(param);
					if (CustomerData != null)
					{
						var topCustomers = CustomerData.GroupBy(i => i.CustomerAcronym)
						  .Select(g => new TopCustomer
						  {
							  Customer = g.Key,
							  TotalInvoiceValue = (decimal)g.Sum(i => i.CollectionValueInr)
						  })
						  .OrderByDescending(c => c.TotalInvoiceValue)
						  .Take(10)
						  .ToList();

						allTopCustomers.AddRange(topCustomers);
					}
				}
			}

			var combinedTopCustomers = allTopCustomers
			  .GroupBy(c => c.Customer)
			  .Select(g => new TopCustomer
			  {
				  Customer = g.Key,
				  TotalInvoiceValue = g.Sum(c => c.TotalInvoiceValue)
			  })
			  .OrderByDescending(c => c.TotalInvoiceValue)
			  .Take(10)
			  .ToList();

			decimal totalInvoiceValueSum = combinedTopCustomers.Sum(c => c.TotalInvoiceValue);

			foreach (var customer in combinedTopCustomers)
			{
				biBiDashboardChart.Add(new BiFinDashReportResponse
				{
					ServiceLine = customer.Customer,
					PercentageCostCtc = (long)(Math.Round((customer.TotalInvoiceValue / totalInvoiceValueSum) * 100))
				});
			}

			return biBiDashboardChart;
		}

		public async Task<List<BiFinDashReportBarChartDataResponse>> GetFinDashReportExistingAndNewData(DirectCostParam param)
		{
			List<BiFinDashReportBarChartDataResponse> biBiDashboardChart = new List<BiFinDashReportBarChartDataResponse>();
			BarChartData aggregatedData = new BarChartData();
			var headerKey = new Dictionary<string,
			  bool>();

			string[] headerArray = { "direct cost", "indirect cost", "other cost", "customer data" };
			if (param.heading == null || param.heading.ToLower() == "all")
			{
				headerKey.Add("direct cost", true);
				headerKey.Add("indirect cost", true);
				headerKey.Add("other cost", true);
				headerKey.Add("customer data", true);
			}
			else
			{
				if (param.heading.Split(',').Count() > 0)
				{
					foreach (var item in param.heading.Split(','))
					{
						headerKey.Add(item.ToLower(), true);
					}
				}
			}

			foreach (var item in headerKey)
			{
				if (item.Key == "direct cost")
				{
					var DirectCostData = await _context.GetFinDashDirectCostReportProcedure(param);
					if (DirectCostData != null)
					{
						var existingAndNewData = CalculateExistingAndNewDirectCostData(DirectCostData);

						aggregatedData.Existing += existingAndNewData.Existing;
						aggregatedData.New += existingAndNewData.New;
					}
				}
				else if (item.Key == "indirect cost") { }
				else if (item.Key == "other cost")
				{
					var OtherCostData = await _context.GetFinOtherCostReportProcedure(param);
					if (OtherCostData != null)
					{
						var existingAndNewData = CalculateExistingAndNewData(OtherCostData);
						aggregatedData.Existing += existingAndNewData.Existing;
						aggregatedData.New += existingAndNewData.New;
					}

				}
				else if (item.Key == "customer data")
				{
					var CustomerData = await _context.GetFinCustomerDataReportProcedure(param);
					if (CustomerData != null)
					{
						var existingAndNewData = CalculateExistingAndNewCustomerData(CustomerData);
						aggregatedData.Existing += existingAndNewData.Existing;
						aggregatedData.New += existingAndNewData.New;
					}
				}
			}

			biBiDashboardChart.Add(new BiFinDashReportBarChartDataResponse
			{
				existingData = (long)aggregatedData.Existing,
				newData = (long)aggregatedData.New
			});


			return biBiDashboardChart;
		}

		private BarChartData CalculateExistingAndNewCustomerData(List<BiCustomerDatum> customerData)
		{
			var summarizedData = new BarChartData();

			foreach (var data in customerData)
			{
				decimal existingValue = 0;
				decimal newValue = 0;

				// Assuming 'NewBusiness' field determines if it's new business or not
				if (data.NewBusiness == true)
				{
					newValue = (decimal)data.CollectionValueInr;
				}
				else
				{
					existingValue = (decimal)data.CollectionValueInr;
				}

				summarizedData.Existing += existingValue;
				summarizedData.New += newValue;
			}

			return summarizedData;
		}

		private BarChartData CalculateExistingAndNewData(List<BiOtherCost> otherCostData)
		{
			var summarizedData = new BarChartData();

			foreach (var data in otherCostData)
			{
				decimal existingValue = 0;
				decimal newValue = 0;

				if (data.BudgetedAmount > 0)
				{
					if (data.TotalInvoiceValueInr <= data.BudgetedAmount)
					{
						existingValue = data.TotalInvoiceValueInr;
					}
					else
					{
						existingValue = data.BudgetedAmount;
						newValue = data.TotalInvoiceValueInr - data.BudgetedAmount;
					}
				}
				else
				{
					newValue = data.TotalInvoiceValueInr;
				}

				summarizedData.Existing += existingValue;
				summarizedData.New += newValue;
			}

			return summarizedData;
		}

		private BarChartData CalculateExistingAndNewDirectCostData(List<BiDirectCost> directCostData)
		{
			var summarizedData = new BarChartData();

			foreach (var data in directCostData)
			{
				decimal existingValue = 0;
				decimal newValue = 0;

				// Assuming we have budgeted amount information
				decimal budgetedAmount = GetBudgetedAmountForDirectCost(data);

				if (budgetedAmount > 0)
				{
					if (data.CostCtc <= budgetedAmount)
					{
						existingValue = data.CostCtc;
					}
					else
					{
						existingValue = budgetedAmount;
						newValue = data.CostCtc - budgetedAmount;
					}
				}
				else
				{
					newValue = data.CostCtc;
				}

				summarizedData.Existing += existingValue;
				summarizedData.New += newValue;
			}

			return summarizedData;
		}


		public async Task<List<BiDashboardChart>> GetMatrixwisePublishers(string Metrics, string Top, string sMonthYear, string eMonthYear, string publisher)
		{
			List<BiDashboardChart> biBiDashboardChart = new List<BiDashboardChart>();
			var MetricChart = await _context.GetUnderperformingPublishers("", Metrics, Top, sMonthYear, eMonthYear, publisher);
			if (MetricChart != null)
			{
				foreach (var item in MetricChart)
				{
					var userDto = new BiDashboardChart
					{
						PublisherName = item.PublisherName,
						Count = item.Count,
						logoPath = item.logoPath
					};
					biBiDashboardChart.Add(userDto);
				}

			}
			return biBiDashboardChart;
		}

		private decimal GetBudgetedAmountForDirectCost(BiDirectCost data)
		{
			// You would replace this with actual logic to get the budgeted amount
			return 500000; // Example budgeted amount
		}

		//public async Task<BiOtherCost> GetById(long id)
		//{
		//	return await _context.BiOtherCosts.FindAsync(id);
		//}
	}

	

	public class BarChartData
	{
		public decimal Existing { get; set; }
		public decimal New { get; set; }
	}

}
