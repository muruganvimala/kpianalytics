using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
namespace BusinessIntelligence_API.Repository
{
	public class IndirectLabourCostRepository: IIndirectLabourCostRepository
	{
		private readonly JTSContext _context;

		public IndirectLabourCostRepository(JTSContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<BiIndirectLabourCost>> GetAll()
		{
			return await _context.BiIndirectLabourCosts.ToListAsync();
		}

		public async Task<BiIndirectLabourCost> GetById(long id)
		{
			return await _context.BiIndirectLabourCosts.FindAsync(id);
		}

		public async Task Create(BiIndirectLabourCost item)
		{
			item.CreatedTime = DateTime.Now;
			_context.BiIndirectLabourCosts.Add(item);
			await _context.SaveChangesAsync();
		}

		public async Task Update(long id, BiIndirectLabourCost item)
		{
			var existingItem = await _context.BiIndirectLabourCosts.FindAsync(id);
			if (existingItem == null)
			{
				throw new Exception("Item not found");
			}

			existingItem.Year = item.Year;
			existingItem.Department = item.Department;
			existingItem.Branch = item.Branch;
			existingItem.Fc = item.Fc;
			existingItem.FxRate = item.FxRate;
			existingItem.CostCtc = item.CostCtc;
			existingItem.NoOfManDate = item.NoOfManDate;
			existingItem.UpdatedTime = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task Delete(long id)
		{
			var item = await _context.BiIndirectLabourCosts.FindAsync(id);
			if (item == null)
			{
				throw new Exception("Item not found");
			}

			_context.BiIndirectLabourCosts.Remove(item);
			await _context.SaveChangesAsync();
		}

        //public async Task<IEnumerable<BiIndirectLabourCost>> GetDataByFilter(InDirectCostFilterParam filterParam)
        //{
        //    try
        //    {
        //        return await _context.BiIndirectLabourCosts.FromSqlInterpolated($"EXEC sp_bi_IndirectCostFilter  {filterParam.DepartmentFilter}, {filterParam.FCFilter}, {filterParam.BranchFilter}").ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Properly handle the exception (e.g., logging)
        //        throw; // rethrowing the exception after logging it
        //    }
        //}

		public async Task<IEnumerable<BiIndirectLabourCost>> GetDataByFilter(InDirectCostFilterParam filterParam)
		{
			try
			{
				return await _context.GetInDirectCostByFilterProcedure(filterParam);
			}
			catch (Exception ex)
			{
				// Properly handle the exception (e.g., logging)
				throw; // rethrowing the exception after logging it
			}
		}
	}
}
