using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
	public interface ICustomerDatumRepository
	{
		Task<IEnumerable<BiCustomerDatum>> GetAll();
		Task<BiCustomerDatum> GetById(long id);
		Task Create(BiCustomerDatum item);
		Task Update(long id, BiCustomerDatum item);
		Task Delete(long id);
        Task<IEnumerable<BiCustomerDatum>> GetDataByFilter(CustomerDataFilterParam filterParam);
    }

	public class CustomerDataFilterParam
	{
		public string? InvoiceNo { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public string? CustomerName { get; set; } 
		public string? CustomerAcronym { get; set; }
		public string? CCYType { get; set; } = null;
		public string? MajorHeadServiceLine { get; set; }

    }
}
