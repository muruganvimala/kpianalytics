using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiUserMasterDto
	{
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Emailid { get; set; }
        public string? Displayname { get; set; }
        public string? Signature { get; set; }
    }
}
