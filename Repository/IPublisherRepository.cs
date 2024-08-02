using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{	
	public interface IPublisherRepository
	{
		Task<BiPublisher> GetByIdAsync(int id);
		Task<List<BiPublisher>> GetAllAsync();
		Task InsertAsync(BiPublisher BiRoleMaster);
		Task DeleteAsync(int id);
		Task UpdateAsync(BiPublisher BiRoleMaster);
	}
}
