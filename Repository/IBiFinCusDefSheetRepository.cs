using BusinessIntelligence_API.Models;

using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
    public interface IBiFinCusDefSheetRepository
    {
        Task<IEnumerable<BiFinCusDefSheet>> GetAll();
        Task<BiFinCusDefSheet> GetById(long id);
        Task Create(BiFinCusDefSheet item);
        Task Update(BiFinCusDefSheet item);
        Task Delete(long id);
    }
}
