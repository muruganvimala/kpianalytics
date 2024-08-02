using BusinessIntelligence_API.Models;
namespace BusinessIntelligence_API.Repository
{
	public interface IUserMasterRepository
	{
		Task<BiUserMaster_mapper> GetByIdAsync(int id);
		Task<BiUserMasterDto> GetByIdAsync(string userName);
		Task<List<BiUserMaster>> GetAllAsync();
		Task InsertAsync(BiUserMaster biUserMaster);
		Task DeleteAsync(int id);
		Task UpdateAsync(BiUserMaster biUserMaster);
	}
}
