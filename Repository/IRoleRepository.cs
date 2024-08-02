using BusinessIntelligence_API.Models;
namespace BusinessIntelligence_API.Repository
{
	public interface IRoleRepository
	{
		Task<BiRoleMaster> GetByIdAsync(int id);		
		Task<List<BiRoleMaster_Mapper>> GetAllAsync();		
		Task InsertAsync(BiRoleMaster BiRoleMaster);
		Task DeleteAsync(int id);
		Task UpdateAsync(BiRoleMaster BiRoleMaster);

		Task<RoleMenuRequestupdate> GetRoleMenuByIdRequestsAsync(string username);
		Task<List<RoleMenuRequest>> GetAllRoleMenuRequestsAsync();
		//Task InsertAsync(RoleMenuRequest menuRequest);
		//Task DeleteRoleMenuById(string username);//UpdateByRoleName
		//Task UpdateByRoleName(RoleMenuRequest menuRequest);

		Task<List<MenuRequest>> GetAllMenuRequestsAsync();

		Task<List<MenuInventoriesCustom>> GetUserMenuInventoriesRequestsAsync(string userRole);
		Task<BiMenuInventory> GetUserAccessRequestsAsync(MenuDetails menuDetails);

		Task<string> GetMenuByRoleRequestsAsync(string userRole);

		Task<bool> RolemenuInsertAsync(UserRoleMenuRequest userRoleMenuRequest);

		Task UpdateUserMenuInventories(
		  UserRoleMenuRequestupdate userRoleMenuRequestupdate);

		Task DeleteUserMenuInventories(string userRole);
		Task<MenuIdClass> GetMenuData(string url);

		bool isRoleExist(string userRole);
	}
}
