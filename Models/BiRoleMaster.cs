using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiRoleMaster
    {
		public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public string? ControlCenter { get; set; }
        public bool? CanView { get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanUpdate { get; set; }
        public bool? CanDelete { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
