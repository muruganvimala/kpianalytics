using BusinessIntelligence_API.Models;
namespace BusinessIntelligence_API.Repository
{
	public interface IDashboardRepository
	{
		//Task<BiUserMaster_mapper> GetByIdAsync(int id);
		//Task<BiUserMasterDto> GetByIdAsync(string userName);
		//Task<List<BiUserMaster>> GetAllAsync();
		//Task InsertAsync(BiUserMaster biUserMaster);
		//Task DeleteAsync(int id);
		//Task UpdateAsync(BiUserMaster biUserMaster);
		Task<List<SSP_BIReport_DetailedResult>> ExecuteSSPBIReportAsync(string publisherName, string metrics, string fromMonthYear, string toMonthYear);
		Task<List<SSP_BIReportFormattedData>> ExecuteSSPBIReportShortfallsAsync(string fromMonthYear, string toMonthYear);
		Task<List<BiDashboardChart>> GetUnderperformingPublishers(string sMonthYear, string eMonthYear, string Bottom, string Publisher);
		Task<List<BiDashboardChart>> GetLeadingReliableContributors(string sMonthYear, string eMonthYear, string top, string Publisher);
		Task<List<BiDashboardMetricChart>> GetConsistentlyPerformingServices(string sMonthYear, string eMonthYear, string top, string metric);
		Task<List<BiDashboardMonthyearChart>> GetDashboardMonthyear(string sMonthYear, string eMonthYear);
		//murugan
		Task<List<BiUnderperforming>> GetUnderperformingServices(string variable1, string variable2, string toporbottom,string sMonthYear, string eMonthYear, string metric);
		Task<List<BiDashboardMetricChart>> GetUnderPerformingServices(string sMonthYear, string eMonthYear, string Bottom, string Metric);

		//finance
		//Task<List<FinanceChartResponse>> GetDashboardFinance(string heading, string serviceline, string customers, string startMonth, string endMonth);
		//Task<List<ServiceResult>> GetFinanceServiceData1(string heading, string serviceline, string customers, string startMonth, string endMonth);
		//Task<List<ServiceResult>> GetFinanceServiceData2(string heading, string serviceline, string customers, string startMonth, string endMonth);
		Task<List<BiFinDashReportResponse>> GetFinDashReportProcedure1(DirectCostParam param);
		Task<List<BiFinDashReportResponse>> GetFinDashReportProcedure2(DirectCostParam param);
		Task<List<BiFinDashReportResponse>> GetFinDashReporttop10customer(DirectCostParam param);
		Task<List<BiFinDashReportBarChartDataResponse>> GetFinDashReportExistingAndNewData(DirectCostParam param);

		//jagdish
		Task<List<BiDashboardChart>> GetMatrixwisePublishers(string Metrics, string Top, string sMonthYear, string eMonthYear, string publisher);
	}

	public class DirectCostParam
	{
		public string heading { get; set; }
		public string serviceLine { get; set; }
		public string customerName { get; set; }
		public string valueType { get; set; }
		public string startMonth { get; set; }
		public string endMonth { get; set; }

	}
}
