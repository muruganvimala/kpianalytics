using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
    public interface IQMSFeedbackRepository
    {
        Task<IEnumerable<BiQmsFeedback>> GetAll();
        Task<BiQmsFeedback> GetById(long id);
        Task Create(BiQmsFeedback item);
        Task Update(BiQmsFeedback item);
        Task Delete(long id);
    }
}
