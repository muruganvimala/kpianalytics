using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<BiDepartmentMst>> GetAll();
        Task<BiDepartmentMst> GetById(long id);
        Task Create(BiDepartmentMst item);
        Task Update(BiDepartmentMst item);
        Task Delete(long id);
    }
}
