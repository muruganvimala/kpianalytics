using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BusinessIntelligence_API.Repository
{
	public class CustomerDatumRepository:ICustomerDatumRepository
	{
		private readonly JTSContext _context;

		public CustomerDatumRepository(JTSContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<BiCustomerDatum>> GetAll()
		{
			return await _context.BiCustomerData.ToListAsync();
		}

		public async Task<BiCustomerDatum> GetById(long id)
		{
			return await _context.BiCustomerData.FindAsync(id);
		}

		//public async Task Create(BiCustomerDatum item)
		//{
		//	item.CreatedTime = DateTime.Now;
		//	_context.BiCustomerData.Add(item);
		//	await _context.SaveChangesAsync();
		//}
		public async Task Create(BiCustomerDatum item)
		{
			// Set the Year
			item.InvoiceDate = item.InvoiceDate?.AddDays(1);
			item.Year = item.InvoiceDate?.ToString("MMM yyyy", CultureInfo.InvariantCulture);//

			// Get customer details
			var customerDetails = _context.BiFinCusDefSheets
				.Where(i => i.Acronym == item.CustomerAcronym)
				.Select(i => new { i.CustomerName, i.Ccy, i.Vat, i.Vatpercent })
				.FirstOrDefault();

			//if (customerDetails == null)
			//{
			//	// Handle the case where customer details are not found
			//	throw new Exception("Customer details not found.");
			//}

			item.CustomerName = customerDetails.CustomerName;
			item.Ccytype = customerDetails.Ccy;
			if(item.Vat == null)
			{
                item.Vat = customerDetails.Vat;
            }

			decimal vatPercent = (decimal)customerDetails.Vatpercent;

			// Get exchange rates
			switch (item.Ccytype)
			{
				case "USD":
					item.InvoiceFxrate = _context.BiForices
						.Where(i => i.Date == item.InvoiceDate)
						.Select(i => i.UsdInr)
						.FirstOrDefault();
					break;

				case "GBP":
					item.InvoiceFxrate = _context.BiForices
						.Where(i => i.Date == item.InvoiceDate)
						.Select(i => i.GbpInr)
						.FirstOrDefault();
					break;

				// Add more cases for other currencies if needed
				default:
					// Handle the default case if necessary
					item.InvoiceFxrate = 1; // Default to 1 or another default value
					break;
			}

			item.CollectionFxrate = item.InvoiceFxrate;

			// Calculate values
			item.GrossValueFc = item.Quantity * item.Rate;

			item.Vatvalue = (vatPercent / 100) * item.GrossValueFc;
			
			item.NetValue = item.GrossValueFc + item.Vatvalue;
			item.GrossValueInr = item.NetValue * item.InvoiceFxrate;
			item.CollectionValueInr = item.CollectionValueFc * item.CollectionFxrate;
			item.ForexGainLoss = item.GrossValueInr - item.CollectionValueInr;
			item.AgedDays = (decimal)( (int) (DateTime.Now - item.InvoiceDate)?.TotalDays + 1);
			item.CreatedTime = DateTime.Now;

			// Save to the database
			_context.BiCustomerData.Add(item);
			await _context.SaveChangesAsync();
		}

		public async Task Update(long id, BiCustomerDatum item)
		{
			var existingItem = await _context.BiCustomerData.FindAsync(id);
			if (existingItem == null)
			{
				throw new Exception("Item not found");
			}

            existingItem.UA = item.UA;
			existingItem.InvoiceNo = item.InvoiceNo;
			existingItem.InvoiceDate = item.InvoiceDate?.AddDays(1);
            existingItem.CustomerAcronym = item.CustomerAcronym;
			existingItem.Tspage = item.Tspage;
			existingItem.MajorHeadServiceLine = item.MajorHeadServiceLine;
			existingItem.MinorHead = item.MinorHead;
			existingItem.Quantity = item.Quantity;
			existingItem.Uom = item.Uom;
			existingItem.Rate = item.Rate;
			existingItem.CollectionDate = item.CollectionDate;
			existingItem.CollectionValueFc = item.CollectionValueFc;
			existingItem.Irm = item.Irm;
			existingItem.StpiSubmissionDate = item.StpiSubmissionDate;
			existingItem.SoftexNo = item.SoftexNo;
			existingItem.EdpmsUploadDate = item.EdpmsUploadDate;
			existingItem.EdpmsRefNo = item.EdpmsRefNo;
			existingItem.EdpmsClosureYnp = item.EdpmsClosureYnp;
			existingItem.EbrcNo = item.EbrcNo;
			existingItem.EbrcDate = item.EbrcDate;
			existingItem.AdBank = item.AdBank;
			existingItem.NewBusiness = item.NewBusiness;

			existingItem.Year = item.InvoiceDate?.ToString("MMM yyyy", CultureInfo.InvariantCulture);

			var customerDetails = _context.BiFinCusDefSheets
				.Where(i => i.Acronym == item.CustomerAcronym)
				.Select(i => new { i.CustomerName, i.Ccy, i.Vat, i.Vatpercent })
				.FirstOrDefault();
			decimal vatPercent = (decimal)customerDetails.Vatpercent;

            

            if (customerDetails == null)
			{
				throw new Exception("Customer details not found.");
			}

			existingItem.CustomerName = customerDetails.CustomerName;
			existingItem.Ccytype = customerDetails.Ccy;
            if (item.Vat == null)
            {
                existingItem.Vat = customerDetails.Vat;
            }
			else {
                existingItem.Vat = item.Vat;
            }
            

			switch (existingItem.Ccytype)
			{
				case "USD":
					existingItem.InvoiceFxrate = _context.BiForices
						.Where(i => i.Date == item.InvoiceDate)
						.Select(i => i.UsdInr)
						.FirstOrDefault();
					break;

				case "GBP":
					existingItem.InvoiceFxrate = _context.BiForices
						.Where(i => i.Date == item.InvoiceDate)
						.Select(i => i.GbpInr)
						.FirstOrDefault();
					break;

				// Add more cases for other currencies if needed
				default:
					existingItem.InvoiceFxrate = 1; // Default to 1 or another default value
					break;
			}

			existingItem.CollectionFxrate = existingItem.InvoiceFxrate;

			existingItem.GrossValueFc = item.Quantity * item.Rate;

			existingItem.Vatvalue = (vatPercent / 100) * existingItem.GrossValueFc;

			existingItem.NetValue = existingItem.GrossValueFc + existingItem.Vatvalue;
			existingItem.GrossValueInr = existingItem.NetValue * existingItem.InvoiceFxrate;
			existingItem.CollectionValueInr = item.CollectionValueFc * existingItem.CollectionFxrate;
			existingItem.ForexGainLoss = existingItem.GrossValueInr - existingItem.CollectionValueInr;
			existingItem.AgedDays = (decimal)(DateTime.Now - item.InvoiceDate)?.TotalDays + 1;

			existingItem.UpdatedTime = DateTime.Now;
			existingItem.UpdatedBy = item.UpdatedBy;

			await _context.SaveChangesAsync();
		}
		
		public async Task Delete(long id)
		{
			var item = await _context.BiCustomerData.FindAsync(id);
			if (item == null)
			{
				throw new Exception("Item not found");
			}

			_context.BiCustomerData.Remove(item);
			await _context.SaveChangesAsync();
		}

        public async Task<IEnumerable<BiCustomerDatum>> GetDataByFilter(CustomerDataFilterParam filterParam)
        {
            try
            {
                return await _context.BiCustomerData.FromSqlInterpolated($"EXEC sp_bi_customerDataFilter {filterParam.InvoiceNo}, {filterParam.InvoiceDate}, {filterParam.CustomerName}, {filterParam.CustomerAcronym}, {filterParam.CCYType}, {filterParam.MajorHeadServiceLine}").ToListAsync();
            }
            catch (Exception ex)
            {
                // Properly handle the exception (e.g., logging)
                throw; // rethrowing the exception after logging it
            }
        }
    }
}
