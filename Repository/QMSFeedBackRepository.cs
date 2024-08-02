using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessIntelligence_API.Repository
{
    public class QMSFeedBackRepository : IQMSFeedbackRepository
    {
        private readonly JTSContext _context;

        public QMSFeedBackRepository(JTSContext context)
        {
            _context = context;
        }
        public async Task Create(BiQmsFeedback item)
        {
            item.CreatedOn = DateTime.Now;
            _context.BiQmsFeedbacks.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {

            var existingItem = await _context.BiQmsFeedbacks.Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }
            existingItem.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BiQmsFeedback>> GetAll()
        {
            return await _context.BiQmsFeedbacks.Where(i => i.IsDeleted == false).ToListAsync();
        }

        public async Task<BiQmsFeedback> GetById(long id)
        {
            return await _context.BiQmsFeedbacks.Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(BiQmsFeedback item)
        {
            item.UpdatedOn = DateTime.Now;
            var existingItem = await _context.BiQmsFeedbacks.Where(i => i.IsDeleted == false && i.Id == item.Id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }

            existingItem.PublisherId = item.PublisherId;
            existingItem.PublisherName = item.PublisherName;
            existingItem.Pe = item.Pe;
            existingItem.Ce = item.Ce;
            existingItem.Mc = item.Mc;
            existingItem.Typ = item.Typ;
            existingItem.Pm = item.Pm;
            existingItem.Positive = item.Positive;
            existingItem.Xml = item.Xml;
            existingItem.NotNterror = item.NotNterror;
            existingItem.Technical = item.Technical;
            existingItem.Total = item.Total;
            await _context.SaveChangesAsync();
        }
    }
    }

