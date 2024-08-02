namespace BusinessIntelligence_API.Models
{
	public class FinanceChartModelRequest
	{

	}

	public class FinanceChartRequest
	{
		public string Heading { get; set; }
		public string ServiceLine { get; set; }
		public string Customers { get; set; }
		public string ValueType { get; set; }
		public string StartMonth { get; set; }
		public string EndMonth { get; set; }
	}

	public partial class FinanceChartResponse
	{
		public string Year { get; set; }
		public string ServiceLine { get; set; }
		public string Customer { get; set; }
		public string Fc { get; set; }
		public decimal? FxRate { get; set; }
		public decimal CostCtc { get; set; }
	}

	public class ServiceData
	{
		public string Year { get; set; }
		public string ServiceLine { get; set; }
		public int Series { get; set; }
	}

	public class ServiceResult
	{
		public string Labels { get; set; }
		public int Series { get; set; }
	}

}
