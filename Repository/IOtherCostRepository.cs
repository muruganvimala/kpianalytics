using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
	public interface IOtherCostRepository
	{
		Task<List<BiOtherCost>> GetAll();
		Task<BiOtherCost> GetById(long id);
		Task Create(BiOtherCost item);
		Task Update(long id, BiOtherCost item);
		Task Delete(long id);
		//filter
		Task<IEnumerable<BiOtherCost>> Filter(OtherCostFilter filter);
	}
}
