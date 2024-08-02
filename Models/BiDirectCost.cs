using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiDirectCost
    {
        public long Id { get; set; }
        public string Year { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string ServiceLine { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string Fc { get; set; } = null!;
        public decimal? FxRate { get; set; }
        public decimal CostCtc { get; set; }
        public string Branch { get; set; } = null!;
        public decimal? NoOfManDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

}
