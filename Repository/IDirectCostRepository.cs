using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
	public interface IDirectCostRepository
	{
		Task<IEnumerable<BiDirectCost>> GetAll();
		Task<BiDirectCost> GetById(long id);
		Task Create(BiDirectCost item);
		Task Update(long id, BiDirectCost item);
		Task Delete(long id);

        Task<IEnumerable<BiDirectCost>> GetDataByFilter(DirectCostFilterParam filterParam);
    }
    public class DirectCostFilterParam
    {
        public string? TypeFilter { get; set; }
        public string? DepartmentFilter { get; set; }
        public string? ServiceLineFilter { get; set; }
        public string? CustomerFilter { get; set; }
        public string? FCFilter { get; set; }
        public string? BranchFilter { get; set; }

    }
}
