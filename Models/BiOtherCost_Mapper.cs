using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
	public class BiOtherCost_Mapper
	{
		public static BiOtherCost_Mapper Map(BiOtherCost biOtherCost)
		{
			return new BiOtherCost_Mapper
			{
				Id = biOtherCost.Id,
				Vch = biOtherCost.Vch,
				SupplierName = biOtherCost.SupplierName,
				TypeOfExpense = biOtherCost.TypeOfExpense,
				ServiceLine = biOtherCost.ServiceLine ?? "",
				Customer = biOtherCost.Customer ?? "",
				Description = biOtherCost.Description,
				InvoiceNo = biOtherCost.InvoiceNo,
				InvoiceDate = biOtherCost.InvoiceDate != null ? DateOnly.Parse(biOtherCost.InvoiceDate.ToString("yyyy-MM-dd")) : new DateOnly(),
				PoNo = biOtherCost.PoNo ?? "",
				PoDate = biOtherCost.PoDate != null ? DateOnly.Parse(biOtherCost.PoDate.Value.ToString("yyyy-MM-dd")) : new DateOnly(),
				Rcm = biOtherCost.Rcm ? "Yes" : "No",
				HsnSac = biOtherCost.HsnSac,
				Qty = biOtherCost.Qty,
				Rate = biOtherCost.Rate,
				Value = biOtherCost.Value,
				Fxrate = biOtherCost.Fxrate,
				ValueInr = biOtherCost.ValueInr,
				Vat = biOtherCost.Vat ?? null,
				Cgst = biOtherCost.Cgst ?? null,
				Sgst = biOtherCost.Sgst ?? null,
				Igst = biOtherCost.Igst,
				TotalInvoiceValueInr = biOtherCost.TotalInvoiceValueInr,
				TdsApplicable = biOtherCost.TdsApplicable ? "Yes" : "No",
				TdsDeclaration = biOtherCost.TdsDeclaration ? "Yes" : "No",
				TdsSection = biOtherCost.TdsSection ?? "", // Default value if null
				TdsRate = biOtherCost.TdsRate,
				TdsValue = biOtherCost.TdsValue,
				Budgeted = biOtherCost.Budgeted ? "Yes" : "No",
				BudgetedAmount = biOtherCost.BudgetedAmount,
				Variance = biOtherCost.Variance,
				CreatedTime = biOtherCost.CreatedTime,
				UpdatedTime = biOtherCost.UpdatedTime
			};
		}


		public long Id { get; set; }
		public string Vch { get; set; } = null!;
		public string SupplierName { get; set; } = null!;
		public string TypeOfExpense { get; set; } = null!;
		public string? ServiceLine { get; set; }
		public string? Customer { get; set; }
		public string Description { get; set; } = null!;
		public string InvoiceNo { get; set; } = null!;
		public DateOnly InvoiceDate { get; set; }
		public string? PoNo { get; set; }
		public DateOnly? PoDate { get; set; }
		public string Rcm { get; set; }
		public decimal HsnSac { get; set; }
		public decimal Qty { get; set; }
		public decimal Rate { get; set; }
		public decimal Value { get; set; }
		public decimal? Fxrate { get; set; }
		public decimal? ValueInr { get; set; }
		public decimal? Vat { get; set; }
		public decimal? Cgst { get; set; }
		public decimal? Sgst { get; set; }
		public decimal Igst { get; set; } 
		public decimal TotalInvoiceValueInr { get; set; }
		public string TdsApplicable { get; set; }
		public string TdsDeclaration { get; set; }
		public string? TdsSection { get; set; } = null!;
		public decimal TdsRate { get; set; }
		public decimal? TdsValue { get; set; }
		public string Budgeted { get; set; }
		public decimal BudgetedAmount { get; set; }
		public decimal? Variance { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? UpdatedTime { get; set; }
	}

	public class BiOtherCost_Mapper1
	{
		public static BiOtherCost_Mapper1 MapWithoutYesNo(BiOtherCost biOtherCost)
		{
			return new BiOtherCost_Mapper1
			{
				Id = biOtherCost.Id,
				Vch = biOtherCost.Vch,
				SupplierName = biOtherCost.SupplierName,
				TypeOfExpense = biOtherCost.TypeOfExpense,
				ServiceLine = biOtherCost.ServiceLine ?? "",
				Customer = biOtherCost.Customer ?? "",
				Description = biOtherCost.Description,
				InvoiceNo = biOtherCost.InvoiceNo,
				InvoiceDate = biOtherCost.InvoiceDate != null ? DateOnly.Parse(biOtherCost.InvoiceDate.ToString("yyyy-MM-dd")) : new DateOnly(),
				PoNo = biOtherCost.PoNo ?? "",
				PoDate = biOtherCost.PoDate != null ? DateOnly.Parse(biOtherCost.PoDate.Value.ToString("yyyy-MM-dd")) : new DateOnly(),
				Rcm = biOtherCost.Rcm, // Keeping this line for consistency with the existing method
				HsnSac = biOtherCost.HsnSac,
				Qty = biOtherCost.Qty,
				Rate = biOtherCost.Rate,
				Value = biOtherCost.Value,
				Fxrate = biOtherCost.Fxrate,
				ValueInr = biOtherCost.ValueInr,
				Vat = biOtherCost.Vat ?? null,
				Cgst = biOtherCost.Cgst ?? null,
				Sgst = biOtherCost.Sgst ?? null,
				Igst = biOtherCost.Igst,
				TotalInvoiceValueInr = biOtherCost.TotalInvoiceValueInr,
				TdsApplicable = biOtherCost.TdsApplicable,
				TdsDeclaration = biOtherCost.TdsDeclaration,
				TdsSection = biOtherCost.TdsSection ?? "", // Default value if null
				TdsRate = biOtherCost.TdsRate,
				TdsValue = biOtherCost.TdsValue,
				Budgeted = biOtherCost.Budgeted,
				BudgetedAmount = biOtherCost.BudgetedAmount,
				Variance = biOtherCost.Variance,
				CreatedTime = biOtherCost.CreatedTime,
				UpdatedTime = biOtherCost.UpdatedTime
			};
		}

		public long Id { get; set; }
		public string Vch { get; set; } = null!;
		public string SupplierName { get; set; } = null!;
		public string TypeOfExpense { get; set; } = null!;
		public string? ServiceLine { get; set; }
		public string? Customer { get; set; }
		public string Description { get; set; } = null!;
		public string InvoiceNo { get; set; } = null!;
		public DateOnly InvoiceDate { get; set; }
		public string? PoNo { get; set; }
		public DateOnly? PoDate { get; set; }
		public bool Rcm { get; set; }
		public decimal HsnSac { get; set; }
		public decimal Qty { get; set; }
		public decimal Rate { get; set; }
		public decimal Value { get; set; }
		public decimal? Fxrate { get; set; }
		public decimal? ValueInr { get; set; }
		public decimal? Vat { get; set; }
		public decimal? Cgst { get; set; }
		public decimal? Sgst { get; set; }
		public decimal Igst { get; set; }
		public decimal TotalInvoiceValueInr { get; set; }
		public bool TdsApplicable { get; set; }
		public bool TdsDeclaration { get; set; }
		public string? TdsSection { get; set; } = null!;
		public decimal TdsRate { get; set; }
		public decimal? TdsValue { get; set; }
		public bool Budgeted { get; set; }
		public decimal BudgetedAmount { get; set; }
		public decimal? Variance { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? UpdatedTime { get; set; }
	}
}
