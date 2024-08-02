using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiOtherCostDisplay
	{
        public long Id { get; set; }
        public string Vch { get; set; } = null!;
        public string SupplierName { get; set; } = null!;
        public string TypeOfExpense { get; set; } = null!;
        public string? ServiceLine { get; set; }
        public string? Customer { get; set; }
        public string Description { get; set; } = null!;
        public string InvoiceNo { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public string? PoNo { get; set; }
        public DateTime? PoDate { get; set; }
        public string Rcm { get; set; }
        public decimal HsnSac { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public decimal Fxrate { get; set; }
        public decimal ValueInr { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Cgst { get; set; }
        public decimal? Sgst { get; set; }
        public decimal Igst { get; set; }
        public decimal TotalInvoiceValueInr { get; set; }
        public string TdsApplicable { get; set; }
        public string TdsDeclaration { get; set; }
        public string? TdsSection { get; set; }
        public decimal TdsRate { get; set; }
        public decimal TdsValue { get; set; }
        public string Budgeted { get; set; }
        public decimal BudgetedAmount { get; set; }
        public decimal? Variance { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
