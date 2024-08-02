using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
namespace BusinessIntelligence_API.Repository
{
	public class OtherCostRepository:IOtherCostRepository
	{
		private readonly JTSContext _context;

		public OtherCostRepository(JTSContext context)
		{
			_context = context;
		}

		public async Task<List<BiOtherCost>> GetAll()
		{
			return await _context.BiOtherCosts.ToListAsync();
		}

		public async Task<BiOtherCost> GetById(long id)
		{
			return await _context.BiOtherCosts.FindAsync(id);
		}

		public async Task Create(BiOtherCost item)
		{
			item.CreatedTime = DateTime.Now;
			_context.BiOtherCosts.Add(item);
			await _context.SaveChangesAsync();
		}

		public async Task Update(long id, BiOtherCost item)
		{
			var existingItem = await _context.BiOtherCosts.FindAsync(id);
			if (existingItem == null)
			{
				throw new Exception("Item not found");
			}

			existingItem.Vch = item.Vch;
			existingItem.SupplierName = item.SupplierName;
			existingItem.TypeOfExpense = item.TypeOfExpense;
			existingItem.ServiceLine = item.ServiceLine;
			existingItem.Customer = item.Customer;
			existingItem.Description = item.Description;
			existingItem.InvoiceNo = item.InvoiceNo;
			existingItem.InvoiceDate = item.InvoiceDate;
			existingItem.PoNo = item.PoNo;
			existingItem.PoDate = item.PoDate;
			existingItem.Rcm = item.Rcm;
			existingItem.HsnSac = item.HsnSac;
			existingItem.Qty = item.Qty;
			existingItem.Rate = item.Rate;
			existingItem.Value = item.Value;
			existingItem.Fxrate = item.Fxrate;
			existingItem.ValueInr = item.ValueInr;
			existingItem.Vat = item.Vat;
			existingItem.Cgst = item.Cgst;
			existingItem.Sgst = item.Sgst;
			existingItem.Igst = item.Igst;
			existingItem.TotalInvoiceValueInr = item.TotalInvoiceValueInr;
			existingItem.TdsApplicable = item.TdsApplicable;
			existingItem.TdsDeclaration = item.TdsDeclaration;
			existingItem.TdsSection = item.TdsSection;
			existingItem.TdsRate = item.TdsRate;
			existingItem.TdsValue = item.TdsValue;
			existingItem.Budgeted = item.Budgeted;
			existingItem.BudgetedAmount = item.BudgetedAmount;
			existingItem.Variance = item.Variance;
			existingItem.UpdatedTime = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task Delete(long id)
		{
			var item = await _context.BiOtherCosts.FindAsync(id);
			if (item == null)
			{
				throw new Exception("Item not found");
			}

			_context.BiOtherCosts.Remove(item);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<BiOtherCost>> Filter(OtherCostFilter filter)
		{
			var query = _context.BiOtherCosts.AsQueryable();

			if (!string.IsNullOrEmpty(filter.SupplierName))
				query = query.Where(x => x.SupplierName.Contains(filter.SupplierName));

			if (!string.IsNullOrEmpty(filter.TypeOfExpense))
				query = query.Where(x => x.TypeOfExpense.Contains(filter.TypeOfExpense));

			if (!string.IsNullOrEmpty(filter.ServiceLine))
				query = query.Where(x => x.ServiceLine.Contains(filter.ServiceLine));

			if (!string.IsNullOrEmpty(filter.Customer))
				query = query.Where(x => x.Customer.Contains(filter.Customer));

			if (!string.IsNullOrEmpty(filter.InvoiceNo))
				query = query.Where(x => x.InvoiceNo.Contains(filter.InvoiceNo));

			if (filter.InvoiceDate.HasValue)
				query = query.Where(x => x.InvoiceDate == filter.InvoiceDate);

			if (!string.IsNullOrEmpty(filter.PoNo))
				query = query.Where(x => x.PoNo.Contains(filter.PoNo));

			return await query.ToListAsync();
		}

	}
}
