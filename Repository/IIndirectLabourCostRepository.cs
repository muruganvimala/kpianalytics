using BusinessIntelligence_API.Models;
namespace BusinessIntelligence_API.Repository
{
	public interface IIndirectLabourCostRepository
	{
		Task<IEnumerable<BiIndirectLabourCost>> GetAll();
		Task<BiIndirectLabourCost> GetById(long id);
		Task Create(BiIndirectLabourCost item);
		Task Update(long id, BiIndirectLabourCost item);
		Task Delete(long id);

		Task<IEnumerable<BiIndirectLabourCost>> GetDataByFilter(InDirectCostFilterParam filterParam);
	}
	   
}
