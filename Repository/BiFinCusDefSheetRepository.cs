using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessIntelligence_API.Repository
{
    public class BiFinCusDefSheetRepository : IBiFinCusDefSheetRepository
    {
        private readonly JTSContext _context;

        public BiFinCusDefSheetRepository(JTSContext context)
        {
            _context = context;
        }
        public async Task Create(BiFinCusDefSheet item)
        {
            item.CreatedOn = DateTime.Now;
            _context.BiFinCusDefSheets.Add(item);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(long id)
        {
           var existingItem = await _context.BiFinCusDefSheets.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<BiFinCusDefSheet>> GetAll()
        {
            return await _context.BiFinCusDefSheets.ToListAsync();
        }

        public async Task<BiFinCusDefSheet> GetById(long id)
        {
            return await _context.BiFinCusDefSheets.Where(i => i.Id == id).FirstOrDefaultAsync();
        }

      

        public async Task Update(BiFinCusDefSheet item)
        {
            item.UpdatedOn = DateTime.Now;
            var existingItem = await _context.BiFinCusDefSheets.Where(i => i.Id == item.Id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }
            existingItem.Acronym = item.Acronym;
            existingItem.CustomerName = item.CustomerName;
            existingItem.Ccy = item.Ccy;
            existingItem.Vat = item.Vat;
            existingItem.Vatpercent = item.Vatpercent;
            await _context.SaveChangesAsync();
        }

    }
}
