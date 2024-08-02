using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;


namespace BusinessIntelligence_API.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly JTSContext _context;

        public DepartmentRepository(JTSContext context)
        {
            _context = context;
        }
        public async Task Create(BiDepartmentMst item)
        {
            item.CreatedOn = DateTime.Now;
            _context.BiDepartmentMsts.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            //var item = await _context.BiQmsData.FindAsync(id);
            //if (item == null)
            //{
            //    throw new Exception("Item not found");
            //}

            //_context.BiQmsData.Remove(item);
            //await _context.SaveChangesAsync();

            var existingItem = await _context.BiDepartmentMsts.Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }
            existingItem.IsDeleted = true;
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<BiDepartmentMst>> GetAll()
        {
            return await _context.BiDepartmentMsts.Where(i => i.IsDeleted == false).ToListAsync();
        }

        public async Task<BiDepartmentMst> GetById(long id)
        {
            return await _context.BiDepartmentMsts.Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(BiDepartmentMst item)
        {
            item.UpdatedOn = DateTime.Now;
            var existingItem = await _context.BiDepartmentMsts.Where(i => i.IsDeleted == false && i.Id == item.Id).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                throw new Exception("Item not found");
            }

            existingItem.Acronym = item.Acronym;
            existingItem.Title = item.Title;
            await _context.SaveChangesAsync();
        }
    }
}

