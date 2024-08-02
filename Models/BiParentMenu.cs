using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiParentMenu
    {
        public long Id { get; set; }
        public string? MenuName { get; set; }
        public int MenuId { get; set; }
        public string? Label { get; set; }
        public string? Icon { get; set; }
        public string? Link { get; set; }
        public bool? Iactive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
