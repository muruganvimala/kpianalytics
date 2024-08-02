using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BusinessIntelligence_API.Models
{
    [Keyless]
    public partial class BiDashboardChart
    {
        public string? PublisherName { get; set; }
        public int? Count { get; set; }
		public string? logoPath { get; set; }
	}

	[Keyless]
	public partial class BiDashboardMetricChart
	{
		public string? Metrics { get; set; }
		public int? Count { get; set; }
	}

	[Keyless]
    public partial class BiDashboardMonthyearChart
    {
        public string PublisherName { get; set; }
        public string Metrics { get; set; }
        public string MonthYear { get; set; } 
        public int Target { get; set; }
        public int Actual {  get; set; }
		public string? Percentage { get; set; }
	}


	[Keyless]
	public partial class BiUnderperforming
	{
		public string PublisherName { get; set; }
		public int Count { get; set; }
	}

	[Keyless]
	public partial class BiFinDashReportResponse
	{
		public string ServiceLine { get; set; }
		public long TotalCostCtc { get; set; }
		public long PercentageCostCtc { get; set; }
	}

	
	public partial class BiFinDashReportBarChartDataResponse
	{
		public string TypeOfExpense { get; set; }
		public long existingData { get; set; }
		public long newData { get; set; }
	}

	[Keyless]
	public partial class BiFinDashTop10CustomerReportResponse
	{
		public string Customer { get; set; }
		public decimal TotalInvoiceValue { get; set; }
	}

	public class TopCustomer
	{
		public string Customer { get; set; }
		public decimal TotalInvoiceValue { get; set; }
	}
}
