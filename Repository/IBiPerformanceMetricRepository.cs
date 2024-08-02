using BusinessIntelligence_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessIntelligence_API.Repository
{
	public interface IBiPerformanceMetricRepository
	{
		Task<BiPerformanceMetric> GetByIdAsync(int id);
		Task<List<BiPerformanceMetric>> GetAllAsync();
		Task<List<BiPerformanceMetricString>> GetAllConfigAsync();
		Task InsertAsync(BiPerformanceMetric biPerformanceMetric);
		Task DeleteAsync(int id);
		Task UpdateAsync(BiPerformanceMetric biPerformanceMetric);
	}
}
