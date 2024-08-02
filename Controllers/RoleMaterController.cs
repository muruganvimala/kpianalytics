using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.Text.RegularExpressions;

namespace BusinessIntelligence_API.Controllers
{
	[ApiController]
	public class RoleMaterController : BaseController
	{
		private readonly IRoleRepository _roleRepository;
		private readonly JTSContext _jtscontext;
		private readonly ILogger<RoleMaterController> _logger;

		public RoleMaterController(JTSContext jTSContext , IRoleRepository roleRepository, ILogger<RoleMaterController> logger, IHttpContextAccessor httpContextAccessor)
		{
			_roleRepository = roleRepository;
			_jtscontext = jTSContext;
			_logger = logger;
			InitializeApiCallLog(httpContextAccessor);
		}

		[HttpPost("Rolemaster/Insert")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Insert([FromBody] BiRoleMaster biRoleMaster)
		{
			var requestData = JsonConvert.SerializeObject(biRoleMaster);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{
				await _roleRepository.InsertAsync(biRoleMaster);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record Inserted Successfully";
				return Ok(new { Message = "Record Inserted Successfully", status = true });
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Error = "Error" });
			}
		}

		[HttpGet("Rolemaster/Display")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Display()
		{
			try
			{
				var result = await _roleRepository.GetAllAsync();
				if (result != null && result.Any())
				{
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = "Data retrieved successfully";
					return Ok(new { Status = true, Data = result });
				}
				else
				{
					_biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
					_biApiCallLog.ResponseData = "No data found";
					return NotFound(new { Message = "No data found", status = false });
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}


		[HttpGet("Rolemaster/GetRoleNameCollection")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetRoleNameCollection()
		{
			try
			{
				var roleNames = await _roleRepository.GetAllAsync();
				if (roleNames != null && roleNames.Any())
				{
					var roleNameCollection = roleNames.Select(role => role.RoleName).ToList();
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = "Data retrieved successfully";
					return Ok(new { Status = true, Data = roleNameCollection });
				}
				else
				{
					_biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
					_biApiCallLog.ResponseData = "No data found";
					return NotFound(new { Message = "No data found", status = false });
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}


		//[HttpGet("Rolemaster/DisplayById/{id}")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> DisplayById(int id)
		//{
		//	try
		//	{
		//		var result = await _roleRepository.GetByIdAsync(id);
		//		if (result != null)
		//		{
		//			_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//			_biApiCallLog.ResponseData = "Record fetched";
		//			return Ok(new { Status = true, Data = result });
		//		}
		//		else
		//		{
		//			_biApiCallLog.StatusCode = StatusCodes.Status404NotFound;
		//			_biApiCallLog.ResponseData = "No record found";
		//			return NotFound(new { Message = "No record found", status = false });
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception as needed
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = "Unable to get data";
		//		_biApiCallLog.Exception = ex.Message + ex.StackTrace;
		//		return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
		//	}
		//}


		//[HttpPut("Rolemaster/Update")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> Update([FromBody] BiRoleMaster	 biRoleMaster)
		//{			
		//	try
		//	{
		//		var requestData = JsonConvert.SerializeObject(biRoleMaster);
		//		_biApiCallLog.RequestData = requestData;

		//		await _roleRepository.UpdateAsync(biRoleMaster);

		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = $"Record with RoleId {biRoleMaster.RoleId} updated successfully.";

		//		return Ok(new { Message = $"Record with RoleId {biRoleMaster.RoleId} updated successfully.", status = true });
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception as needed
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = $"Unable to update record with ID {biRoleMaster.RoleId}";
		//		_biApiCallLog.Exception = ex.Message + ex.StackTrace;
		//		return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
		//	}
		//}

		//[HttpDelete("Rolemaster/DeleteById/{id}")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> DeleteById(int id)
		//{
		//	try
		//	{
		//		_biApiCallLog.RequestData = id.ToString();
		//		await _roleRepository.DeleteAsync(id);

		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = $"Record with ID {id} deleted successfully.";

		//		return Ok(new { Message = $"Record with ID {id} deleted successfully.", status = true });
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception as needed
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = $"Unable to delete record ID {id}";
		//		_biApiCallLog.Exception = ex.Message + ex.StackTrace;

		//		return StatusCode(500, new { Message = "Unable to delete record", status = false, Error = ex.Message });
		//	}
		//}

		////menu
		//[HttpPost("Rolemaster/MenuInsert")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> MenuInsert([FromBody] RoleMenuRequest request)
		//{
		//	var requestData = JsonConvert.SerializeObject(request);
		//	_biApiCallLog.RequestData = requestData;
		//	if (ModelState.IsValid)
		//	{
		//		await _roleRepository.InsertAsync(request);
		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = "Record Inserted Successfully";

		//		return Ok(new { Message = "Record Inserted Successfully", status = true });
		//	}
		//	else
		//	{
		//		_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
		//		_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
		//		return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Error = "Error" });
		//	}
		//}

		[HttpGet("Rolemaster/AllRoleMenu")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> AllRoleMenu()
		{
			try
			{
				var result = await _roleRepository.GetAllRoleMenuRequestsAsync();
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = result });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpGet("Rolemaster/AllRoleMenuById/{rolename}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> AllRoleMenuById(string rolename)
		{
			try
			{
				_biApiCallLog.RequestData = rolename;

				var result = await _roleRepository.GetRoleMenuByIdRequestsAsync(rolename);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Record fetched";

				return Ok(new { Status = true, Data = result });
			}
			catch (Exception ex)
			{
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = "Unable to get data";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				Log.Error(ex, "An error occurred while processing the request for role {RoleName}.", rolename);
				return StatusCode(500, new { Message = "Unable to get data for role " + rolename, Status = false, Error = ex.Message });
			}
		}

		//[HttpPut("Rolemaster/UpdateByRoleName")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> UpdateByRoleName([FromBody] RoleMenuRequest request)
		//{
		//	try
		//	{
		//		var requestData = JsonConvert.SerializeObject(request);
		//		_biApiCallLog.RequestData = requestData;

		//		await _roleRepository.UpdateByRoleName(request);

		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = $"Record with RoleName updated successfully.";

		//		return Ok(new { Message = $"Record with RoleName updated successfully.", status = true });
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception as needed
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = $"Unable to update record";
		//		_biApiCallLog.Exception = ex.Message + ex.StackTrace;
		//		return StatusCode(500, new { Message = "Unable to update record", status = false, Error = ex.Message });
		//	}
		//}

		//[HttpDelete("Rolemaster/DeleteRoleMenuById/{rolename}")]
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> DeleteRoleMenuById(string rolename)
		//{
		//	try
		//	{
		//		_biApiCallLog.RequestData = rolename;

		//		await _roleRepository.DeleteRoleMenuById(rolename);

		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = $"Record with ID {rolename} deleted successfully.";

		//		return Ok(new { Message = $"Record with ID {rolename} deleted successfully.", status = true });
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception as needed
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = $"Unable to delete record ID {rolename}";
		//		_biApiCallLog.Exception = ex.Message + ex.StackTrace;

		//		return StatusCode(500, new { Message = "Unable to delete record", status = false, Error = ex.Message });
		//	}
		//}

		//menu structure


		[HttpGet("Rolemaster/getMenuByRole/{userRole}")]//for menu diplay
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getMenuByRole(string userRole)
		{
			try
			{
				var jsonResult = await _roleRepository.GetMenuByRoleRequestsAsync(userRole);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(jsonResult);
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpGet("Rolemaster/getAllMenu")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getAllMenu() //add form
		{
			try
			{
				var result = await _roleRepository.GetAllMenuRequestsAsync();
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(new { Status = true, Data = result });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpPost("Rolemaster/roleMenuInsert")] //insert form
		[Authorize(Roles = "User")]
		public async Task<IActionResult> roleMenuInsert([FromBody] UserRoleMenuRequest request)
		{
			var requestData = JsonConvert.SerializeObject(request);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{
				bool result=  await _roleRepository.RolemenuInsertAsync(request);
				if (result)
				{
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = "Record Inserted Successfully";

					return Ok(new { Message = "Record Inserted Successfully", status = true });
				}
				else
				{
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = "Record already Exist";

					return BadRequest(new { Message = "Record already Exist.", status = false, Error = "Error" });
				}
				
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Insertion failure, Invalid Model";
				return BadRequest(new { Message = "Insertion failure, Invalid Model", status = false, Error = "Error" });
			}
		}


		[HttpGet("Rolemaster/getUserMenuInventories/{userRole}")]//display for update form
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getUserMenuInventories(string userRole)
		{
			try
			{
				var jsonResult = await _roleRepository.GetUserMenuInventoriesRequestsAsync(userRole);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(jsonResult);
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		[HttpGet("Rolemaster/getUserAccess")]//get access details
		[Authorize(Roles = "User")]
		public async Task<IActionResult> getUserAccess(string userrole, string url)
		{
			try
			{
				url = Regex.Replace(url,"app/","",RegexOptions.IgnoreCase);
				var data = await _roleRepository.GetMenuData(url);
				MenuDetails menuDetails = new MenuDetails
				{
					userrole = userrole,
					parentmenuid = data.parentMenuId.ToString(),
					childmenuid = data.childMenuId.ToString()
				};
				var jsonResult = await _roleRepository.GetUserAccessRequestsAsync(menuDetails);
				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = "Data retrieved successfully";
				return Ok(jsonResult);
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = ex.Message;
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;
				return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
			}
		}

		//[HttpGet("Rolemaster/getUserAccess")]//get access details
		//[Authorize(Roles = "User")]
		//public async Task<IActionResult> getUserAccess(string userrole, string parentmenuid, string childmenuid)
		//{
		//	try
		//	{
		//		MenuDetails menuDetails = new MenuDetails
		//		{
		//			userrole = userrole,
		//			parentmenuid = parentmenuid,
		//			childmenuid = childmenuid
		//		};
		//		var jsonResult = await _roleRepository.GetUserAccessRequestsAsync(menuDetails);
		//		_biApiCallLog.StatusCode = StatusCodes.Status200OK;
		//		_biApiCallLog.ResponseData = "Data retrieved successfully";
		//		return Ok(jsonResult);
		//	}
		//	catch (Exception ex)
		//	{
		//		// Log or handle the exception as needed
		//		_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
		//		_biApiCallLog.ResponseData = ex.Message;
		//		_biApiCallLog.Exception = ex.Message + ex.StackTrace;
		//		return StatusCode(500, new { Message = "Unable to get data", status = false, Error = ex.Message });
		//	}
		//}

		[HttpPut("Rolemaster/putUserMenuInventories")] //update form menu config
		[Authorize(Roles = "User")]
		public async Task<IActionResult> putUserMenuInventories([FromBody] UserRoleMenuRequestupdate request)
		{
			var requestData = JsonConvert.SerializeObject(request);
			_biApiCallLog.RequestData = requestData;
			if (ModelState.IsValid)
			{
				string rolename = request.RoleMaster.roleName;
				if (_roleRepository.isRoleExist(rolename))
				{
					await _roleRepository.UpdateUserMenuInventories(request);
					_biApiCallLog.StatusCode = StatusCodes.Status200OK;
					_biApiCallLog.ResponseData = $"Record with updated successfully.";
					return Ok(new { Message = "Record with updated successfully", status = true });
				}
				else
				{
					return BadRequest(new { Message = "Update failure, Role not found in menu config", status = false, Error = "Error" });
				}
				
			}
			else
			{
				_biApiCallLog.StatusCode = StatusCodes.Status400BadRequest;
				_biApiCallLog.ResponseData = "Update failure, Invalid Model";
				return BadRequest(new { Message = "Update failure, Invalid Model", status = false, Error = "Error" });
			}
		}


		[HttpDelete("Rolemaster/DeleteUserMenuInventories/{rolename}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteUserMenuInventories(string rolename) // delete from menu config
		{
			try
			{
				_biApiCallLog.RequestData = rolename;

				await _roleRepository.DeleteUserMenuInventories(rolename);

				_biApiCallLog.StatusCode = StatusCodes.Status200OK;
				_biApiCallLog.ResponseData = $"Record with ID {rolename} deleted successfully.";

				return Ok(new { Message = $"Record with ID {rolename} deleted successfully.", status = true });
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				_biApiCallLog.StatusCode = StatusCodes.Status500InternalServerError;
				_biApiCallLog.ResponseData = $"Unable to delete record ID {rolename}";
				_biApiCallLog.Exception = ex.Message + ex.StackTrace;

				return StatusCode(500, new { Message = "Unable to delete record", status = false, Error = ex.Message });
			}
		}


	}
}
