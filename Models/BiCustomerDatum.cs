using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiCustomerDatum
    {
        public long Id { get; set; }
        public string? UA { get; set; }
        public string? Year { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAcronym { get; set; }
        public decimal? Tspage { get; set; }
        public string? Ccytype { get; set; }
        public string? MajorHeadServiceLine { get; set; }
        public string? MinorHead { get; set; }
        public decimal? Quantity { get; set; }
        public string? Uom { get; set; }
        public decimal? Rate { get; set; }
        public decimal? GrossValueFc { get; set; }
        public bool? Vat { get; set; }
        public decimal? Vatvalue { get; set; }
        public decimal? NetValue { get; set; }
        public decimal? InvoiceFxrate { get; set; }
        public decimal? GrossValueInr { get; set; }
        public DateTime? CollectionDate { get; set; }
        public decimal? CollectionValueFc { get; set; }
        public decimal? CollectionFxrate { get; set; }
        public decimal? CollectionValueInr { get; set; }
        public decimal? ForexGainLoss { get; set; }
        public string? Irm { get; set; }
        public DateTime? StpiSubmissionDate { get; set; }
        public string? SoftexNo { get; set; }
        public DateTime? EdpmsUploadDate { get; set; }
        public string? EdpmsRefNo { get; set; }
        public string? EdpmsClosureYnp { get; set; }
        public string? EbrcNo { get; set; }
        public DateTime? EbrcDate { get; set; }
        public string? AdBank { get; set; }
        public bool? NewBusiness { get; set; }
        public decimal? AgedDays { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
