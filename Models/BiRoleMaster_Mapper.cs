using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiRoleMaster_Mapper
	{
		public int Id { get; set; }
		public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }       
    }
}
