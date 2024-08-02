using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiFinCusDefSheet
    {
        public int Id { get; set; }
        public string? Acronym { get; set; }
        public string? CustomerName { get; set; }
        public string? Ccy { get; set; }
        public bool? Vat { get; set; }
        public decimal? Vatpercent { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
