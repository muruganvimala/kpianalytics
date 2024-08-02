using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Transactions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BusinessIntelligence_API.Repository
{
	public class RoleRepository : IRoleRepository
	{
		private readonly JTSContext _context;
	
		public RoleRepository()
        {
			_context = new JTSContext();
        }

		public async Task InsertAsync(BiRoleMaster biRoleMaster)
		{
			biRoleMaster.CreatedTime = DateTime.Now;
			await _context.BiRoleMasters.AddAsync(biRoleMaster);
			await _context.SaveChangesAsync();
		}
		

		public async Task<List<BiRoleMaster_Mapper>> GetAllAsync()
		{
			//BiRoleMaster biRoleMaster = await _context.BiRoleMasters.ToListAsync();
			//return await _context.BiRoleMasters.ToListAsync();
			List<BiRoleMaster> biRoleMasters = await _context.BiRoleMasters.ToListAsync();
			List<BiRoleMaster_Mapper> biRoleMaster_Mappers = new List<BiRoleMaster_Mapper>();
			int count = 0;
			foreach (var biRoleMaster in biRoleMasters)
			{
				count = +1;
				biRoleMaster_Mappers.Add(new BiRoleMaster_Mapper() { Id = count, RoleId = biRoleMaster.RoleId, RoleName = biRoleMaster.RoleName, Description = biRoleMaster.Description }); // Populate Id from RoleId
			}
			return biRoleMaster_Mappers;
		}

		

		public async Task<BiRoleMaster> GetByIdAsync(int id)
		{
			return await _context.BiRoleMasters.FindAsync(id);
		}

		public async Task UpdateAsync(BiRoleMaster biRoleMaster)
		{
			var existingKpi = await _context.BiRoleMasters.FindAsync(biRoleMaster.RoleId);

			if (existingKpi != null)
			{

				existingKpi.RoleName = biRoleMaster.RoleName;
				existingKpi.Description = biRoleMaster.Description;
				//existingKpi.CanView = biRoleMaster.CanView;
				//existingKpi.CanAdd = biRoleMaster.CanAdd;
				//existingKpi.CanUpdate = biRoleMaster.CanUpdate;
				//existingKpi.CanDelete = biRoleMaster.CanDelete;
				existingKpi.UpdatedTime = DateTime.Now;
				await _context.SaveChangesAsync();
			}
			else
			{
				// Handle the case where the entity with the given ID is not found
				throw new Exception($"Record with ID {biRoleMaster.RoleId} not found.");
			}
		}

		public async Task UpdateAsync(RoleMenuRequest biRequest)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					BiRoleMaster biRoleMaster = biRequest.RoleMaster;
					var existingKpi = await _context.BiRoleMasters.FindAsync(biRoleMaster.RoleId);

					if (existingKpi != null)
					{
						existingKpi.RoleName = biRoleMaster.RoleName;
						existingKpi.Description = biRoleMaster.Description;
						existingKpi.UpdatedTime = DateTime.Now;
						await _context.SaveChangesAsync();

						foreach (var menuInventory in biRequest.MenuInventories)
						{
							menuInventory.UserroleId = biRoleMaster.RoleName;
							await _context.BiMenuInventories.AddAsync(menuInventory);
						}
						await _context.SaveChangesAsync();

						transaction.Commit(); // Commit the transaction if everything is successful
					}
					else
					{
						throw new InvalidOperationException($"Record with ID {biRoleMaster.RoleId} not found.");
					}
				}
				catch (Exception ex)
				{
					// Rollback the transaction if an exception occurs
					transaction.Rollback();
					throw new Exception("Transaction failed. Changes reverted.", ex);
				}
			}
		}


		public async Task DeleteAsync(int RoleId)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var kpiToDelete = await _context.BiRoleMasters.FirstOrDefaultAsync(x => x.RoleId == RoleId);

					if (kpiToDelete != null)
					{
						_context.BiRoleMasters.Remove(kpiToDelete);
						await _context.SaveChangesAsync();

						transaction.Commit(); // Commit the transaction if everything is successful
					}
					else
					{
						throw new InvalidOperationException($"Role with ID {RoleId} not found.");
					}
				}
				catch (Exception ex)
				{
					// Rollback the transaction if an exception occurs
					transaction.Rollback();
					throw new Exception("Transaction failed. Changes reverted.", ex);
				}
			}
		}


		////new
		public async Task InsertAsync(RoleMenuRequest biRequest)
		{
			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				try
				{
					BiRoleMaster biRoleMaster = biRequest.RoleMaster;
					biRoleMaster.ControlCenter = biRoleMaster.RoleName;
					biRoleMaster.CreatedTime = DateTime.Now;
					await _context.BiRoleMasters.AddAsync(biRoleMaster);
					await _context.SaveChangesAsync();

					foreach (var menuInventory in biRequest.MenuInventories)
					{
						menuInventory.UserroleId = biRoleMaster.RoleName;
						menuInventory.InsertedTime = DateTime.Now;
						await _context.BiMenuInventories.AddAsync(menuInventory);
					}
					await _context.SaveChangesAsync();

					// Complete the transaction
					scope.Complete();
				}
				catch (Exception ex)
				{
					// Revert changes if an exception occurs
					scope.Dispose();
					throw new Exception("Failed to insert role menu request. Transaction rolled back.", ex);
				}
			}
		}

		public async Task<List<RoleMenuRequest>> GetAllRoleMenuRequestsAsync()
		{
			List<BiRoleMaster> biRoleMasters = await _context.BiRoleMasters.ToListAsync();
			List<BiMenuInventory> biMenuInventories = await _context.BiMenuInventories.ToListAsync();

			List<RoleMenuRequest> roleMenuRequests = new List<RoleMenuRequest>();

			foreach (var biRoleMaster in biRoleMasters)
			{
				RoleMenuRequest roleMenuRequest = new RoleMenuRequest();
				roleMenuRequest.RoleMaster = biRoleMaster;
				roleMenuRequest.MenuInventories = biMenuInventories.Where(m => m.UserroleId == biRoleMaster.RoleName).ToList();
				roleMenuRequests.Add(roleMenuRequest);
			}
			return roleMenuRequests;
		}

		public async Task<RoleMenuRequestupdate> GetRoleMenuByIdRequestsAsync(string roleName)
		{
			try
			{
				BiRoleMaster biRoleMaster = await _context.BiRoleMasters.FirstOrDefaultAsync(item => item.RoleName == roleName);
				if (biRoleMaster == null)
				{
					throw new InvalidOperationException($"Role {roleName} not found in BiRoleMasters table.");
				}
				else
				{
					List<BiMenuInventory> biMenuInventories = await _context.BiMenuInventories
						.Where(item => item.UserroleId == biRoleMaster.RoleId.ToString())
						.OrderBy(item => item.ParentmenuId)
						.ThenBy(item => item.ChildmenuId)
						.ToListAsync();
					List<BiMenuInventory_Mapper> biMenuInventoriesmapper = new List<BiMenuInventory_Mapper>();
					foreach (var biMenu in biMenuInventories)
					{
						int ParentmenuId = Convert.ToInt32(biMenu.ParentmenuId);
						string ParentMenuName = _context.BiParentMenus.Where(x => x.MenuId == ParentmenuId).Select(x=>x.MenuName).FirstOrDefault();
						string ChildMenuName = null;

						if (biMenu.ChildmenuId!=null)
						{
							int childmenuId = Convert.ToInt32(biMenu.ChildmenuId);
							ChildMenuName = _context.BiChildmenus.Where(x => x.MenuId == childmenuId).Select(x => x.MenuName).FirstOrDefault();
						}
						
						var bidata = new BiMenuInventory_Mapper
						{
							parentMenuId = biMenu.ParentmenuId,
							parentMenuName = ParentMenuName,
							childMenuId = biMenu.ChildmenuId,
							childMenuName = ChildMenuName,
							canview = biMenu.canview,
							caninsert = biMenu.caninsert ?? false,
							canupdate = biMenu.canupdate ?? false,
							candelete = biMenu.candelete ?? false
						};


						biMenuInventoriesmapper.Add(bidata);
					}
					RoleMenuRequestupdate roleMenuRequest = new RoleMenuRequestupdate
					{
						RoleMaster = biRoleMaster,
						MenuInventories = biMenuInventoriesmapper
					};
					return roleMenuRequest;
				}
			}
			catch (Exception ex)
			{
				if (ex is InvalidOperationException)
				{
					throw ex;
				}
				else
				{
					throw new Exception("Failed to retrieve role menu request.", ex);
				}
			}

		}

		//public async Task UpdateByRoleName(RoleMenuRequest request)
		//{
		//	BiRoleMaster biRoleMaster = request.RoleMaster;
		//	var existingKpi = await _context.BiRoleMasters.FirstOrDefaultAsync(x => x.RoleName == biRoleMaster.RoleName);

		//	if (existingKpi != null)
		//	{
		//		using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
		//		{
		//			try
		//			{
		//				//BiRoleMasters update
		//				existingKpi.RoleName = biRoleMaster.RoleName;
		//				existingKpi.Description = biRoleMaster.Description;
		//				existingKpi.ControlCenter = biRoleMaster.RoleName;
		//				existingKpi.UpdatedTime = DateTime.Now;

		//				//MenuInventories update
		//				foreach (var menuInventory in request.MenuInventories)
		//				{
		//					var existingBiMenuInventories = await _context.BiMenuInventories.FirstOrDefaultAsync(x => x.MenuId == menuInventory.MenuId && x.MenuName == menuInventory.MenuName);
		//					if (existingBiMenuInventories != null)
		//					{
		//						existingBiMenuInventories.Action = menuInventory.Action;
		//						existingBiMenuInventories.UpdatedTime = DateTime.Now;
		//					}
		//					else
		//					{
		//						// Create a new BiMenuInventory if it doesn't exist
		//						var newMenuInventory = new BiMenuInventory
		//						{
		//							MenuId = menuInventory.MenuId,
		//							MenuName = menuInventory.MenuName,
		//							Action = menuInventory.Action,
		//							InsertedTime = DateTime.Now
		//						};
		//						_context.BiMenuInventories.Add(newMenuInventory);
		//					}
		//				}

		//				await _context.SaveChangesAsync();

		//				//remove unused data
		//				List<string> requestedMenuName = request.MenuInventories.Select(item => item.MenuName).ToList();

		//				List<BiMenuInventory> biMenuInventories = await _context.BiMenuInventories.Where(x => x.MenuId == biRoleMaster.RoleName).ToListAsync();
		//				if (biMenuInventories.Any())
		//				{
		//					// Get the MenuNames from biMenuInventories
		//					List<string> existingMenuNames = biMenuInventories.Select(x => x.MenuName).ToList();
		//					// Remove entries from database if they are not in requestedMenuName
		//					foreach (var menuName in existingMenuNames.Except(requestedMenuName).ToList())
		//					{
		//						var menuToRemove = await _context.BiMenuInventories
		//							.FirstOrDefaultAsync(x => x.MenuId == biRoleMaster.RoleName && x.MenuName == menuName);
		//						if (menuToRemove != null)
		//						{
		//							_context.BiMenuInventories.Remove(menuToRemove);
		//						}
		//					}
		//				}

		//				await _context.SaveChangesAsync();

		//				// Complete the transaction
		//				scope.Complete();
		//			}
		//			catch (Exception ex)
		//			{
		//				// Revert changes if an exception occurs
		//				Transaction.Current.Rollback();
		//				throw new Exception("Transaction failed. Changes reverted.", ex);
		//			}
		//		}
		//	}
		//	else
		//	{
		//		// Handle the case where the entity with the given ID is not found
		//		throw new Exception($"Record with ID {biRoleMaster.RoleId} not found.");
		//	}

		//}

		public async Task DeleteRoleMenuById(string roleName)
		{
			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				try
				{
					var BiRoleMastersToDelete = await _context.BiRoleMasters.FirstOrDefaultAsync(x => x.RoleName == roleName);

					if (BiRoleMastersToDelete != null)
					{
						_context.BiRoleMasters.Remove(BiRoleMastersToDelete);
						await _context.SaveChangesAsync();
					}

					var BiMenuInventories = await _context.BiMenuInventories.Where(x => x.UserroleId == roleName).ToListAsync();
					if (BiMenuInventories.Count > 0)
					{
						foreach (var item in BiMenuInventories)
						{
							_context.BiMenuInventories.Remove(item);
							await _context.SaveChangesAsync();
						}
					}

					// Complete the transaction
					scope.Complete();
				}
				catch (Exception ex)
				{
                    // Revert changes if an exception occurs
                    System.Transactions.Transaction.Current.Rollback();
					throw new Exception("Transaction failed. Changes reverted.", ex);
				}
			}
		}

		public async Task<string> GetMenuByRoleRequestsAsync(string userRole) //done
		{
			string jsonresult = "";
			BiRoleMaster biRoleMaster = await _context.BiRoleMasters.Where(x => x.RoleName == userRole).FirstOrDefaultAsync();
			if (biRoleMaster != null)
			{
				//List<BiMenuInventory> biMenuInventories = await _context.BiMenuInventories
				//.Where(x => x.UserroleId == biRoleMaster.RoleId.ToString())
				//.OrderBy(x => x.ParentmenuId)
				//.ToListAsync();

				List<BiMenuInventory> biMenuInventories = await _context.BiMenuInventories
				.Where(x => x.UserroleId == biRoleMaster.RoleId.ToString() && x.canview == true)
				.OrderBy(x => x.ParentmenuId)
				.ToListAsync();

				if (biMenuInventories.Any())
				{
					List<BiParentMenu> parentMenuslst = new List<BiParentMenu>();
					List<BiChildmenu> childMenus = new List<BiChildmenu>();
					foreach (var biMenuItem in biMenuInventories)
					{
						int ParentmenuId = int.Parse(biMenuItem.ParentmenuId.Trim());
						var parentmns = await _context.BiParentMenus
							.Where(x => x.MenuId == ParentmenuId)
							.FirstOrDefaultAsync();
						if (parentmns != null && !parentMenuslst.Any(x => x.MenuId == parentmns.MenuId))
						{
							parentMenuslst.Add(parentmns);
						}

						int childmenuId = int.Parse(biMenuItem.ChildmenuId.Trim());
						var childmns = await _context.BiChildmenus
							.Where(x => x.MenuId == childmenuId && x.ParentId == ParentmenuId)
							.FirstOrDefaultAsync();
						if (childmns != null)
						{
							childMenus.Add(childmns);
						}
					}

					var parentMenusResult = parentMenuslst.Select(parent => new
					{
						id = parent.MenuId,
						label = parent.Label,
						icon = parent.Icon,
						link = parent.Link != null ? parent.Link : null,
						parentId = parent.MenuId,
						subItems = getsubmenu(childMenus, parent.MenuId)
					})
					.ToList();
					var json = JsonConvert.SerializeObject(parentMenusResult, Formatting.Indented);
					JArray array = JArray.Parse(json);
					foreach (JObject item in array)
					{
						if (item["link"].Type == JTokenType.Null)
						{
							item.Remove("link");
						}

						if (item["subItems"].Type == JTokenType.Null)
						{
							item.Remove("subItems");
							item.Remove("parentId");
						}
					}

					//JObject menuTextItem = new JObject(
					//	new JProperty("id", 1),
					//	new JProperty("label", "MENUITEMS.MENU.TEXT"),
					//	new JProperty("isTitle", true)
					//);

					//array.Insert(0, menuTextItem);
					jsonresult = JsonConvert.SerializeObject(array, Formatting.Indented);
				}
				else
				{
					JObject menuTextItem = new JObject(
						new JProperty("id", 1),
						new JProperty("label", "MENUITEMS.MENU.TEXT"),
						new JProperty("isTitle", true)
					);

					jsonresult = JsonConvert.SerializeObject(menuTextItem, Formatting.Indented);
				}
			}
			else
			{
				JObject menuTextItem = new JObject(
						new JProperty("id", 1),
						new JProperty("label", "MENUITEMS.MENU.TEXT"),
						new JProperty("isTitle", true)
					);

				jsonresult = JsonConvert.SerializeObject(menuTextItem, Formatting.Indented);
			}
			

			return jsonresult;
		}

		private List<object> getsubmenu(List<BiChildmenu> childMenus, int menuId)
		{
			var subItems = childMenus.Where(child => child.ParentId == menuId)
									 .Select(child => new
									 {
										 id = child.Id,
										 label = child.Label,
										 link = child.Link,
										 parentId = child.ParentId
									 })
									 .ToList<object>();
			return subItems.Any() ? subItems : null;
		}

		public async Task<List<MenuRequest>> GetAllMenuRequestsAsync() //done
		{
			List<BiParentMenu> biParentMenus = await _context.BiParentMenus.OrderBy(b => b.MenuId).ToListAsync();
			List<MenuRequest> menuRequests = new List<MenuRequest>();
			foreach (var parentMenu in biParentMenus)
			{
				var childMenus = await _context.BiChildmenus.Where(x => x.ParentId == parentMenu.MenuId).ToListAsync();
				if (childMenus.Count > 0)
				{
					foreach (var childMenu in childMenus)
					{
						MenuRequest mRequest = new MenuRequest
						{
							ParentMenuName = parentMenu.MenuName,
							ParentMenuId = parentMenu.MenuId,
							childMenuName = childMenu.MenuName,
							childMenuId = childMenu.MenuId,
							canview = false,
							caninsert = false,
							canupdate = false,
							candelete = false
						};
						menuRequests.Add(mRequest);
					}
				}
				else
				{
					MenuRequest mRequest = new MenuRequest
					{
						ParentMenuName = parentMenu.MenuName,
						ParentMenuId = parentMenu.MenuId,
						childMenuName = null,
						childMenuId = 0,
						canview = false,
						caninsert = false,
						canupdate = false,
						candelete = false
					};
					menuRequests.Add(mRequest);
				}
				
			}			
			return menuRequests;
		}

		public async Task<List<MenuInventoriesCustom>> GetUserMenuInventoriesRequestsAsync(string userRole) //check
		{
			List<MenuInventoriesCustom> menuInventoriesCustom = new List<MenuInventoriesCustom>();
			List<MenuRequest> menuRequests = new List<MenuRequest>();

			List<BiParentMenu> biParentMenus = await _context.BiParentMenus.OrderBy(b => b.MenuId).ToListAsync();
			
			foreach (var parenmenu in biParentMenus)
			{
				MenuRequest mRequest = new MenuRequest();
				List<BiChildmenu> bichild = await _context.BiChildmenus.Where(x => x.ParentId == parenmenu.MenuId).ToListAsync();
				if (bichild.Count() > 0)
				{					
					foreach (var biChildMenu in bichild)
					{
						MenuInventoriesCustom menuInventories = new MenuInventoriesCustom();
						var biData =_context.BiMenuInventories.Where(x=>x.ParentmenuId == parenmenu.MenuId.ToString() && x.ChildmenuId == biChildMenu.MenuId.ToString() && x.UserroleId == userRole).FirstOrDefault();

						if (biData != null)
						{	
							menuInventories.parentMenuName = parenmenu.MenuName;							
							menuInventories.parentMenuId = parenmenu.MenuId;
							menuInventories.childMenuName = biChildMenu.MenuName;
							menuInventories.childMenuId = biChildMenu.MenuId;
							menuInventories.haschild = true;
							menuInventories.canview = Regex.IsMatch(biData.Action, "view", RegexOptions.IgnoreCase);
							menuInventories.canupdate = Regex.IsMatch(biData.Action, "edit", RegexOptions.IgnoreCase);
							menuInventories.candelete = Regex.IsMatch(biData.Action, "delete", RegexOptions.IgnoreCase);
							menuInventoriesCustom.Add(menuInventories);
						}
						else
						{
							menuInventories.parentMenuName = parenmenu.MenuName;							
							menuInventories.parentMenuId = parenmenu.MenuId;
							menuInventories.childMenuName = biChildMenu.MenuName;
							menuInventories.childMenuId = biChildMenu.MenuId;
							menuInventories.haschild = true;
							menuInventories.canview = false;
							menuInventories.canupdate = false;
							menuInventories.candelete = false;
							menuInventoriesCustom.Add(menuInventories);
						}
						
					}
				}
				else
				{
					MenuInventoriesCustom menuInventories = new MenuInventoriesCustom();
					var biData = _context.BiMenuInventories.Where(x=>x.ParentmenuId == parenmenu.MenuId.ToString() && x.UserroleId == userRole).FirstOrDefault<BiMenuInventory>();
					if (biData != null)
					{
						menuInventories.parentMenuName = parenmenu.MenuName;
						menuInventories.parentMenuId = parenmenu.MenuId;
						menuInventories.childMenuName = null;
						menuInventories.childMenuId = 0;
						menuInventories.haschild = false;
						menuInventories.canview = Regex.IsMatch(biData.Action, "view", RegexOptions.IgnoreCase);
						menuInventories.canupdate = Regex.IsMatch(biData.Action, "edit", RegexOptions.IgnoreCase);
						menuInventories.candelete = Regex.IsMatch(biData.Action, "delete", RegexOptions.IgnoreCase);
						menuInventoriesCustom.Add(menuInventories);
					}
					else
					{
						menuInventories.parentMenuName =parenmenu.MenuName;
						menuInventories.parentMenuId = parenmenu.MenuId;
						menuInventories.childMenuName = null;
						menuInventories.childMenuId = 0;
						menuInventories.haschild = false;
						menuInventories.canview = false;
						menuInventories.canupdate = false;
						menuInventories.candelete = false;
						menuInventoriesCustom.Add(menuInventories);
					}
					
				}
				
			}
		
			return menuInventoriesCustom;
		}



		public async Task<bool> RolemenuInsertAsync(UserRoleMenuRequest userRoleMenuRequest)//done
		{
			RoleMasterRequest roleMaster = userRoleMenuRequest.RoleMaster;
			List<BiMenuInventory> inventory = new List<BiMenuInventory>();
			Boolean isfound = _context.BiMenuInventories.Any(x => x.UserroleId == roleMaster.roleName);
			if (isfound== false)
			{				
				using (var transaction = _context.Database.BeginTransaction())
				{
					try
					{
						BiRoleMaster biRoleMaster = new BiRoleMaster();
						biRoleMaster.RoleName = roleMaster.roleName;
						biRoleMaster.Description = roleMaster.description;
						biRoleMaster.CreatedTime = DateTime.Now;
						_context.BiRoleMasters.Add(biRoleMaster);
						await _context.SaveChangesAsync();

						foreach (var item in userRoleMenuRequest.MenuInventories)
						{
							BiMenuInventory biMenuInventory = new BiMenuInventory();
							biMenuInventory.UserroleId = biRoleMaster.RoleId.ToString();
							biMenuInventory.ParentmenuId = item.ParentMenuId.ToString();
							biMenuInventory.ChildmenuId = item.ChildMenuId.ToString();
							biMenuInventory.canview = item.canview;
							biMenuInventory.caninsert = item.caninsert;
							biMenuInventory.canupdate = item.canupdate;
							biMenuInventory.candelete = item.candelete;
							biMenuInventory.InsertedTime = DateTime.Now;
							await _context.BiMenuInventories.AddAsync(biMenuInventory);
						}
						await _context.SaveChangesAsync();
						transaction.Commit();
						return true;
					}
					catch (Exception ex)
					{
						// Handle exception
						transaction.Rollback();
						throw ex;
					}
				}
			}
			else
			{
				return false;
			}
			
			
		}

		public async Task UpdateUserMenuInventories(UserRoleMenuRequestupdate userRoleMenuRequestupdate) //done
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				RoleMasterRequest biRoleMaster = userRoleMenuRequestupdate.RoleMaster;
				try
				{
					var data = await _context.BiRoleMasters.Where(x => x.RoleId == biRoleMaster.roleId).FirstOrDefaultAsync();
					if (data != null)
					{
						data.RoleName = biRoleMaster.roleName;
						data.Description = biRoleMaster.description;
						await _context.SaveChangesAsync();
					}

					List<MenuInventory> menuInventory = userRoleMenuRequestupdate.MenuInventories;
					var MenuInventoryResult = await _context.BiMenuInventories.Where(x => x.UserroleId == biRoleMaster.roleId.ToString()).ToListAsync();

					if (MenuInventoryResult.Count > 0)
					{
						foreach (var menuInventoryItem in menuInventory)
						{
							string parentmenuID = menuInventoryItem.parentMenuId.ToString();
							string childmenuID = menuInventoryItem.childMenuId.ToString();
							var existingMenuInventory = await _context.BiMenuInventories.Where(x => x.UserroleId == biRoleMaster.roleId.ToString() && x.ParentmenuId == parentmenuID && x.ChildmenuId == childmenuID).FirstOrDefaultAsync();
							if (existingMenuInventory != null)
							{
								existingMenuInventory.canview = menuInventoryItem.canview;
								existingMenuInventory.caninsert = menuInventoryItem.caninsert;
								existingMenuInventory.canupdate = menuInventoryItem.canupdate;
								existingMenuInventory.candelete = menuInventoryItem.candelete;
								existingMenuInventory.UpdatedTime = DateTime.Now;
								await _context.SaveChangesAsync();
							}
							else
							{
								//not found
								BiMenuInventory biMenuInventory = new BiMenuInventory
								{
									UserroleId = biRoleMaster.roleId.ToString(),
									ParentmenuId = parentmenuID,
									ChildmenuId = childmenuID,
									canview = false,
									caninsert=false,
									canupdate=false,
									candelete=false,
									InsertedTime = DateTime.Now,
									UpdatedTime = DateTime.Now
								};
								await _context.BiMenuInventories.AddAsync(biMenuInventory);
								await _context.SaveChangesAsync();
							}							
						}
						transaction.Commit();
					}
					
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw new Exception("Transaction failed. Changes reverted.", ex);
				}
			}
		}

		public async Task DeleteUserMenuInventories(string userRole)//done
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					int roleid = Convert.ToInt32(userRole.Trim());
					var biRoleMasterdata = await _context.BiRoleMasters.Where(x => x.RoleId == roleid).FirstOrDefaultAsync();

					if (biRoleMasterdata != null)
					{
						List<BiMenuInventory> biMenuInventories = await _context.BiMenuInventories.Where(x => x.UserroleId == biRoleMasterdata.RoleId.ToString()).ToListAsync();
						if (biMenuInventories.Count > 0)
						{
							_context.BiMenuInventories.RemoveRange(biMenuInventories);
						}
						_context.BiRoleMasters.Remove(biRoleMasterdata);
					}				
					
					await _context.SaveChangesAsync();
					transaction.Commit();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw new Exception("Transaction failed. Changes reverted.", ex);
				}
			}
		}


		public bool isRoleExist(string userRole) //done
		{
			try
			{
				return _context.BiRoleMasters.Any(x => x.RoleName == userRole);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<BiMenuInventory> GetUserAccessRequestsAsync(MenuDetails menuDetails)
		{
			try
			{
				BiRoleMaster biRoleMaster = await _context.BiRoleMasters.FirstOrDefaultAsync(item => item.RoleName == menuDetails.userrole);
				if (biRoleMaster == null)
				{
					throw new InvalidOperationException($"Role {menuDetails.userrole} not found in BiRoleMasters table.");
				}
				else
				{
					UserAccess userAccess = new UserAccess();
					BiMenuInventory biMenuInventories = await _context.BiMenuInventories
						.Where(item => item.UserroleId == biRoleMaster.RoleId.ToString() && item.ParentmenuId == menuDetails.parentmenuid && item.ChildmenuId == menuDetails.childmenuid).FirstOrDefaultAsync();
					return biMenuInventories;
				}
			}
			catch (Exception ex)
			{
				if (ex is InvalidOperationException)
				{
					throw ex;
				}
				else
				{
					throw new Exception("Failed to retrieve role menu request.", ex);
				}
			}
		}

		public async Task<MenuIdClass> GetMenuData(string url)
		{
			try
			{
				MenuIdClass menuIdClass = new MenuIdClass();

				var menu = await _context.BiParentMenus
					.Where(item => item.Link.ToLower() == url.ToLower())
					.Select(item => new { ParentMenuId = item.MenuId, ChildMenuId = 0 })
					.Union(
						_context.BiChildmenus
							.Where(item => item.Link.ToLower() == url.ToLower())
							.Select(item => new { ParentMenuId = item.ParentId ?? 0, ChildMenuId = item.MenuId })
					)
					.FirstOrDefaultAsync();

				if (menu == null)
				{
					throw new InvalidOperationException($"Invalid {url}, data not found in menu table.");
				}

				menuIdClass.parentMenuId = menu.ParentMenuId;
				menuIdClass.childMenuId = menu.ChildMenuId;

				return menuIdClass;
			}
			catch (InvalidOperationException ex)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to retrieve role menu request.", ex);
			}
		}

	}
}
