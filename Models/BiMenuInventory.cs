using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiMenuInventory
    {
        public long Id { get; set; }
        public string? UserroleId { get; set; }
        public string? ParentmenuId { get; set; }
		public string? ChildmenuId { get; set; }
		public bool? canview { get; set; }
		public bool? caninsert { get; set; }
		public bool? canupdate { get; set; }
		public bool? candelete { get; set; }
		public string? Action { get; set; }
        public DateTime? InsertedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
