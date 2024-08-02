using BusinessIntelligence_API.Models;
namespace BusinessIntelligence_API.Repository
{
	public interface ISettingRepository
	{
		Task<BiPublisherConfig> GetByIdAsync(int id);
		Task<List<BiPublisherConfig>> GetAllAsync();
		Task InsertAsync(BiPublisherConfig settingRepository);
		Task DeleteAsync(int id);
		Task UpdateAsync(BiPublisherConfig settingRepository);
	}
}
