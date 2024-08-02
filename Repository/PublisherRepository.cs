using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessIntelligence_API.Repository
{
	public class PublisherRepository : IPublisherRepository
	{
		private readonly JTSContext _jTSContext;
		public PublisherRepository( JTSContext jTSContext)
        {
			_jTSContext = jTSContext;
		}

		public async Task InsertAsync(BiPublisher biPublisher)
		{
			biPublisher.InsertedDate = DateTime.Now;
			await _jTSContext.BiPublishers.AddAsync(biPublisher);
			await _jTSContext.SaveChangesAsync();
		}

		public async Task<List<BiPublisher>> GetAllAsync()
		{
			//return await _jTSContext.BiPublishers.ToListAsync();
			return await _jTSContext.BiPublishers.OrderBy(p=>p.Acronym).ToListAsync();
		}

		public async Task<BiPublisher> GetByIdAsync(int id)
		{
			return await _jTSContext.BiPublishers.FirstAsync(x => x.Id == id);
		}

		public async Task UpdateAsync(BiPublisher biPublisher)
		{
			var existingKpi = await _jTSContext.BiPublishers.FirstAsync(x => x.Id == biPublisher.Id);
			if (existingKpi != null)
			{
				existingKpi.PublisherName = biPublisher.PublisherName;
				existingKpi.Acronym = biPublisher.Acronym;
				existingKpi.UpdatedDate = DateTime.Now;
				await _jTSContext.SaveChangesAsync();
			}
			else
			{
				// Handle the case where the entity with the given ID is not found
				throw new Exception($"Record with PublisherName {biPublisher.PublisherName} not found.");
			}
		}

		public async Task DeleteAsync(int Id)
		{
			var kpiToDelete = await _jTSContext.BiPublishers.FirstAsync(x => x.Id == Id);

			if (kpiToDelete != null)
			{
				_jTSContext.BiPublishers.Remove(kpiToDelete);
				await _jTSContext.SaveChangesAsync();
			}
		}

	}
}
