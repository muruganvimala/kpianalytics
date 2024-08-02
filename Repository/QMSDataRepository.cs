using BusinessIntelligence_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessIntelligence_API.Repository
{
    public class QMSDataRepository : IQMSDataRepository
    {
        private readonly JTSContext _context;

        public QMSDataRepository(JTSContext context)
        {
            _context = context;
        }
        public async Task Create(BiQmsDatum item)
        {
            item.CreatedOn = DateTime.Now;
            item.PublisherName = _context.BiPublishers.Where(i => i.Id == item.PublisherId).First().Acronym;
            _context.BiQmsData.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id, int loginId)
        {
            //var item = await _context.BiQmsData.FindAsync(id);
            //if (item == null)
            //{
            //    throw new Exception("Item not found");
            //}

            //_context.BiQmsData.Remove(item);
            //await _context.SaveChangesAsync();

            var existingItem = await _context.BiQmsData.Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }
            existingItem.IsDeleted = true;
            existingItem.UpdatedBy = loginId;
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<BiQmsDatum>> GetAll()
        {
            return await _context.BiQmsData.Where(i => i.IsDeleted == false).ToListAsync();
        }

        public async Task<BiQmsDatum> GetById(long id)
        {
            return await _context.BiQmsData.Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(BiQmsDatum item)
        {
            item.UpdatedOn = DateTime.Now;
            var existingItem = await _context.BiQmsData.Where(i => i.IsDeleted == false && i.Id == item.Id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }
            existingItem.MonthYear = item.MonthYear;
            existingItem.PublisherId = item.PublisherId;
            existingItem.PublisherName = _context.BiPublishers.Where(i => i.Id == item.PublisherId).First().Acronym;
            existingItem.EppFp = item.EppFp;
            existingItem.EppRev = item.EppRev;
            existingItem.Feedback = item.Feedback;
            existingItem.Epp = item.Epp;
            existingItem.Rft = item.Rft;
            existingItem.Positive = item.Positive;
            existingItem.PeEpp = item.PeEpp;
            existingItem.CeEpp = item.CeEpp;
            existingItem.TypEpp = item.TypEpp;
            existingItem.McEpp = item.McEpp;
            existingItem.Escalations = item.Escalations;
            existingItem.Ttp = item.Ttp;
            existingItem.ZeroError = item.ZeroError;
            existingItem.AuthorSurvey = item.AuthorSurvey;
            existingItem.UpdatedBy = item.UpdatedBy;
            existingItem.UpdatedOn = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task ImportAsync(IEnumerable<BiRawQmsData> qmsDataCollection)
        {
            var qmsDataToImport = qmsDataCollection.Select(rawQmsData => new BiQmsDatum
            {
                PublisherName = rawQmsData.PublisherName,
                PublisherId = _context.BiPublishers.Where(i=>i.Acronym== rawQmsData.PublisherName).Select(i=>i.Id).First(),
                EppFp = rawQmsData.EppFp,
                EppRev = rawQmsData.EppRev,
                Feedback = rawQmsData.Feedback,
                Epp = rawQmsData.Epp,
                Rft = rawQmsData.Rft,
                Positive = rawQmsData.Positive,
                PeEpp = rawQmsData.PeEpp,
                CeEpp = rawQmsData.CeEpp,
                TypEpp = rawQmsData.TypEpp,
                McEpp = rawQmsData.McEpp,
                Escalations = rawQmsData.Escalations,
                Ttp = rawQmsData.Ttp,
                ZeroError = rawQmsData.ZeroError,
                AuthorSurvey = rawQmsData.AuthorSurvey,
                CreatedOn = DateTime.UtcNow,
            }).ToList();

            await _context.BiQmsData.AddRangeAsync(qmsDataToImport);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BidQmsDataDashboardReport>> QMSDataDashboardReport(BiQmsFilter fParam)
        {
			return await _context.GetQMSDataByFilterProcedure(fParam);
		}
        public async Task<IEnumerable<BIQMSFeedbackDashboardReport>> QMSFeedbackDashboardReport(BiQmsFilter fParam)
        {
			return await _context.GetQMSFeedByFilterProcedure(fParam);
		}

    }
}
