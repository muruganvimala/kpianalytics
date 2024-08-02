using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
    public interface IProcessMasterRepository
    {

        Task<IEnumerable<BiProcessMst>> GetAll();
        Task<BiProcessMst> GetById(long id);
        Task Create(BiProcessMst item);
        Task Update(BiProcessMst item);
        Task Delete(long id);
    }
}
