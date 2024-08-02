using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessIntelligence_API.Repository
{
    public class ForexRepository : IForexRepository
    {
        private readonly JTSContext _jTSContext;
        public ForexRepository(JTSContext jTSContext)
        {
            _jTSContext = jTSContext;
        }

        public async Task<BiForex> GetByIdAsync(int id)
        {
            return await _jTSContext.BiForices.FirstAsync(x => x.Id == id);
        }

        public async Task<List<BiForex>> GetAllAsync()
        {
            return await _jTSContext.BiForices.OrderBy(p => p.Date).ToListAsync();
        }

        public async Task InsertAsync(BiForex biForex)
        {
            biForex.CreatedTime = DateTime.Now;
            await _jTSContext.BiForices.AddAsync(biForex);
            await _jTSContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BiForex biForex)
        {
            var existingForex = await _jTSContext.BiForices.FirstAsync(x => x.Id == biForex.Id);
            if (existingForex != null)
            {
                existingForex.Date = biForex.Date;
                existingForex.UsdInr = biForex.UsdInr;
                existingForex.GbpInr = biForex.GbpInr;
                existingForex.PhpInr = biForex.PhpInr;
                existingForex.UsdGbp = biForex.UsdGbp;
                existingForex.UpdatedTime = DateTime.Now;
                await _jTSContext.SaveChangesAsync();
            }
            else
            {
                // Handle the case where the entity with the given ID is not found
                throw new Exception($"Record with Date {biForex.Date} not found.");
            }
        }

        public async Task DeleteAsync(int Id)
        {
            var forexToDelete = await _jTSContext.BiForices.FirstAsync(x => x.Id == Id);

            if (forexToDelete == null)
            {
				throw new Exception("Item not found");
				
            }
			_jTSContext.BiForices.Remove(forexToDelete);
			await _jTSContext.SaveChangesAsync();

		}

        public async Task<List<BiForex>> GetDateRanged(DateRange range)
        {
            return await _jTSContext.BiForices.
                Where(d => d.Date >= range.StartDate && d.Date <= range.EndDate).ToListAsync();
        }

        public async Task ImportAsync(IEnumerable<BiRawForex> forexCollection)
        {
            var forexToImport = forexCollection.Select(rawForex => new BiForex
            {
                Date = DateTime.ParseExact(rawForex.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                UsdInr = rawForex.UsdInr,
                GbpInr = rawForex.GbpInr,
                PhpInr = rawForex.PhpInr,
                UsdGbp = rawForex.UsdGbp,
                CreatedTime = DateTime.UtcNow, // Assuming the created time is now
                //UpdatedTime = DateTime.UtcNow  // Assuming the updated time is also now if needed
            }).ToList();

            await _jTSContext.BiForices.AddRangeAsync(forexToImport);
            await _jTSContext.SaveChangesAsync();
        }


    }
}
