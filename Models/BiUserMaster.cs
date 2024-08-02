using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiUserMaster
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Emailid { get; set; }
        public string? Displayname { get; set; }
        public string? Signature { get; set; }
        public string? Userrole { get; set; }
        public bool? Activestatus { get; set; }
        public DateTime? Createdtime { get; set; }
        public DateTime? Updatedtime { get; set; }
    }
}
