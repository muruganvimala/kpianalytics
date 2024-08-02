namespace BusinessIntelligence_API.Models
{
	public class RoleMenuRequest
	{
		public BiRoleMaster RoleMaster { get; set; }
		public List<BiMenuInventory> MenuInventories { get; set; }
	}

	public class RoleMenuRequestupdate
	{
		public BiRoleMaster RoleMaster { get; set; }
		public List<BiMenuInventory_Mapper> MenuInventories { get; set; }
	}

	public class UserRoleMenuRequestupdate
	{
		public RoleMasterRequest RoleMaster { get; set; }
		public List<MenuInventory> MenuInventories { get; set; }
	}

	public class UserRoleMenuRequest
	{
		public RoleMasterRequest RoleMaster { get; set; }
		public List<MenuRequestModel1> MenuInventories { get; set; }
	}
	

	public class RoleMasterRequest
	{
		public int roleId { get; set; }
		public string roleName { get; set; }
		public string description { get; set; }
	}

	public class MenuRequestModel1
	{
		public int ParentMenuId { get; set; }
		public int ChildMenuId { get; set; }
		public bool? canview { get; set; }
		public bool? caninsert { get; set; }
		public bool? canupdate { get; set; }
		public bool? candelete { get; set; }
	}

	public class MenuRequest
	{
		public string? ParentMenuName { get; set; }
		public int ParentMenuId { get; set; }
		public string? childMenuName { get; set; }
		public int childMenuId { get; set; }
		//public Boolean haschild { get; set; }
		public Boolean canview { get; set; }
		public Boolean caninsert { get; set; }
		public Boolean canupdate { get; set; }
		public Boolean candelete { get; set; }
	}

	public class MenuInventoriesCustom
	{
		public string parentMenuName { get; set; }
		public int parentMenuId { get; set; }
		public string childMenuName { get; set; }
		public int childMenuId { get; set; }
		public Boolean haschild { get; set; }
		public Boolean canview { get; set; }
		public Boolean canupdate { get; set; }
		public Boolean candelete { get; set; }
	}

	public class MenuItem
	{
		public int Id { get; set; }
		public string Label { get; set; }
		public string Icon { get; set; }
		public string Link { get; set; }
		public int ParentId { get; set; }
		public List<MenuItem> SubItems { get; set; }
	}

	public class MenuInventory
	{		
		public int parentMenuId { get; set; }
		public int childMenuId { get; set; }
		public bool canview { get; set; }
		public bool caninsert { get; set; }
		public bool canupdate { get; set; }
		public bool candelete { get; set; }
	}

	public class MenuIdClass
	{
		public int parentMenuId { get; set; }
		public int childMenuId { get; set; }
	}

	public class MenuInventoryList
	{
		public List<MenuInventory> MenuInventories { get; set; }
	}


}
