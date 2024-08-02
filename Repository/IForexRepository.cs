using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
    public interface IForexRepository
    {
        Task<BiForex> GetByIdAsync(int id);
        Task<List<BiForex>> GetAllAsync();
        Task InsertAsync(BiForex forex);
        Task ImportAsync(IEnumerable<BiRawForex> forexCollection);
        Task DeleteAsync(int id);
        Task UpdateAsync(BiForex forex);
        Task<List<BiForex>> GetDateRanged(DateRange range);

    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
