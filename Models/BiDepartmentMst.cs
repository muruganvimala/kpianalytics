using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiDepartmentMst
    {
        public long Id { get; set; }
        public string? Acronym { get; set; }
        public string? Title { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
