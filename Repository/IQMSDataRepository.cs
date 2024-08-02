using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
    public interface IQMSDataRepository
    {
        Task<IEnumerable<BiQmsDatum>> GetAll();
        Task<BiQmsDatum> GetById(long id);
        Task Create(BiQmsDatum item);
        Task Update(BiQmsDatum item);
        Task Delete(long id,int loginId);
        Task ImportAsync(IEnumerable<BiRawQmsData> qmsDataCollection);
        Task<IEnumerable<BidQmsDataDashboardReport>> QMSDataDashboardReport(BiQmsFilter fParam);
        Task<IEnumerable<BIQMSFeedbackDashboardReport>> QMSFeedbackDashboardReport(BiQmsFilter fParam);

        //Task<IEnumerable<BiQmsDatum>> GetDataByFilter(CustomerDataFParam filterParam);
        //Task ImportAsync(IEnumerable<BiQmsDatum> customerDataCollection);
    }

    public class BIQMSFeedbackDashboardReport
    {
        public long? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public decimal? SumOfPE { get; set; }
        public decimal? SumOfCE { get; set; }
        public decimal? SumOfMC { get; set; }
        public decimal? SumOfTYP { get; set; }
        public decimal? SumOfPM { get; set; }
        public decimal? SumOfXML { get; set; }
        public decimal? SumOfNotNTerror { get; set; }
        public decimal? SumOfTechnical { get; set; }
        public decimal? SumOfPositive { get; set; }
        public decimal? SumOfTotal { get; set; }
    }


}