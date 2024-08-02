using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiIndirectLabourCost
    {
        public long Id { get; set; }
        public string Year { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Branch { get; set; } = null!;
        public string Fc { get; set; } = null!;
        public decimal? FxRate { get; set; }
        public decimal CostCtc { get; set; }
        public decimal NoOfManDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
    
	public class InDirectCostFilterParam
	{
		public string? DepartmentFilter { get; set; }
		public string? BranchFilter { get; set; }
		public string? FCFilter { get; set; }
	}
}
