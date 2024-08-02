namespace BusinessIntelligence_API.Models
{
	public class UserAccess
	{
		public string userrole { get; set; }
		public string parentmenu { get; set; }
		public string childmenu { get; set; }
		public bool? canview { get; set; }
		public bool? caninsert { get; set; }
		public bool? canupdate { get; set; }
		public bool? candelete { get; set; }
	}
	public class MenuDetails
	{
		public string userrole { get; set; }
		public string parentmenuid { get; set; }
		public string childmenuid { get; set; }
	}
}
