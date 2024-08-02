using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiMenuInventory_Mapper
	{
        public string? parentMenuId { get; set; }
		public string? parentMenuName { get; set; }
		public string? childMenuId { get; set; }
		public string? childMenuName { get; set; }
		public bool? canview { get; set; }
		public bool? caninsert { get; set; }
		public bool? canupdate { get; set; }
		public bool? candelete { get; set; }
    }
}
