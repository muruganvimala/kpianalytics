using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessIntelligence_API.Repository
{
	public class DirectCostRepository: IDirectCostRepository
	{
		private readonly JTSContext _context;

		public DirectCostRepository(JTSContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<BiDirectCost>> GetAll()
		{			
			return await _context.BiDirectCosts.ToListAsync(); ;
		}

		public async Task<BiDirectCost> GetById(long id)
		{
			return await _context.BiDirectCosts.FindAsync(id);
		}

		public async Task Create(BiDirectCost item)
		{
			item.CreatedTime = DateTime.Now;
			_context.BiDirectCosts.Add(item);
			await _context.SaveChangesAsync();
		}

		public async Task Update(long id, BiDirectCost item)
		{
			var existingItem = await _context.BiDirectCosts.FindAsync(id);
			if (existingItem == null)
			{
				throw new Exception("Item not found");
			}

			existingItem.Year = item.Year;
			existingItem.Type = item.Type;
			existingItem.Department = item.Department;
			existingItem.ServiceLine = item.ServiceLine;
			existingItem.Customer = item.Customer;
			existingItem.Fc = item.Fc;
			existingItem.FxRate = item.FxRate;
			existingItem.CostCtc = item.CostCtc;
			existingItem.Branch = item.Branch;
			existingItem.NoOfManDate = item.NoOfManDate;
			existingItem.UpdatedTime = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task Delete(long id)
		{
			var item = await _context.BiDirectCosts.FindAsync(id);
			if (item == null)
			{
				throw new Exception("Item not found");
			}

			_context.BiDirectCosts.Remove(item);
			await _context.SaveChangesAsync();
		}

        public async Task<IEnumerable<BiDirectCost>> GetDataByFilter(DirectCostFilterParam filterParam)
        {
            try
            {
                return await _context.BiDirectCosts.FromSqlInterpolated($"EXEC sp_bi_directCostFilter {filterParam.TypeFilter}, {filterParam.DepartmentFilter}, {filterParam.ServiceLineFilter}, {filterParam.CustomerFilter}, {filterParam.FCFilter}, {filterParam.BranchFilter}").ToListAsync();
            }
            catch (Exception ex)
            {
                // Properly handle the exception (e.g., logging)
                throw; // rethrowing the exception after logging it
            }
        }
    }
}
