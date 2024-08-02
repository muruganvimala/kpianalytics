using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using BusinessIntelligence_API.Service;
using System.Security.Cryptography.Xml;
using static BusinessIntelligence_API.Mapping.MappingProfile;

namespace BusinessIntelligence_API.Repository
{
	public class UserMasterRepository : IUserMasterRepository
	{
		private readonly JTSContext _context;
        public UserMasterRepository()
        {
			_context = new JTSContext();
        }

		public async Task InsertAsync(BiUserMaster biUserMaster)
		{
			biUserMaster.Createdtime = DateTime.Now;
			biUserMaster.Password = EncodeandDecode.EncodePassword(biUserMaster.Password);
			await _context.BiUserMasters.AddAsync(biUserMaster);
			await _context.SaveChangesAsync();
		}

		public async Task<List<BiUserMaster>> GetAllAsync()
		{
			var users = await _context.BiUserMasters.ToListAsync();
			foreach (var user in users)
			{
				user.Password = EncodeandDecode.DecodePassword(user.Password);
			}
			return users;
		}

		public async Task<BiUserMaster_mapper> GetByIdAsync(int id)
		{
			//BiUserMaster_mapper biUserMaster_Mapper = new BiUserMaster_mapper();
			//var user = await _context.BiUserMasters.FindAsync(id);
			//if (user == null)
			//{
			//	return null;
			//}
			//biUserMaster_Mapper.Id = user.Id;
			//biUserMaster_Mapper.Firstname = user.Firstname;
			//biUserMaster_Mapper.Lastname = user.Lastname;
			//biUserMaster_Mapper.Username = user.Username;
			//biUserMaster_Mapper.Password = EncodeandDecode.DecodePassword(user.Password);
			//biUserMaster_Mapper.Emailid = user.Emailid;
			//biUserMaster_Mapper.Displayname = user.Displayname;
			//biUserMaster_Mapper.Signature = user.Signature;
			//biUserMaster_Mapper.Userrole = user.Userrole;
			//biUserMaster_Mapper.Activestatus = user.Activestatus;
			//biUserMaster_Mapper.Createdtime = user.Createdtime;
			//biUserMaster_Mapper.Updatedtime = user.Updatedtime;
			//int roleid = Convert.ToInt32(user.Userrole);
			//var biRole = await _context.BiRoleMasters.FirstOrDefaultAsync(item => item.RoleId == roleid);
			//if (biRole != null)
			//{
			//	biUserMaster_Mapper.Rolename = biRole.RoleName;
			//}			
			//return biUserMaster_Mapper;

			var user = await _context.BiUserMasters.FindAsync(id);
			if (user == null)
			{
				return null;
			}
			int roleid = Convert.ToInt32(user.Userrole);
			var biRole = await _context.BiRoleMasters.FirstOrDefaultAsync(item => item.RoleId == roleid);

			var mapper = new BiUserMasterMapper();
			return mapper.Map(user, biRole);
		}

		public async Task<BiUserMasterDto> GetByIdAsync(string userName)
		{
			var user = await _context.BiUserMasters.FirstOrDefaultAsync(item => item.Username == userName);
			if (user != null)
			{
				var userDto = new BiUserMasterDto
				{
					Firstname = user.Firstname,
					Lastname= user.Lastname,
					Username = user.Username,
					Emailid = user.Emailid,
					Displayname = user.Displayname,
					Signature = user.Signature	
				};
				return userDto;
			}

			return null;
		}

		public async Task UpdateAsync(BiUserMaster biUserMaster)
		{
			var existingKpi = await _context.BiUserMasters.FindAsync(biUserMaster.Id);

			if (existingKpi != null)
			{				
				existingKpi.Firstname = biUserMaster.Firstname;
				existingKpi.Lastname = biUserMaster.Lastname;
				existingKpi.Username = biUserMaster.Username;
				existingKpi.Password = EncodeandDecode.EncodePassword(biUserMaster.Password);
				existingKpi.Emailid = biUserMaster.Emailid;
				existingKpi.Displayname = biUserMaster.Displayname;
				existingKpi.Signature = biUserMaster.Signature;
				existingKpi.Userrole = biUserMaster.Userrole;
				existingKpi.Activestatus = biUserMaster.Activestatus;
				existingKpi.Updatedtime = DateTime.Now;
				await _context.SaveChangesAsync();
			}
			else
			{
				// Handle the case where the entity with the given ID is not found
				throw new Exception($"Record with ID {biUserMaster.Id} not found.");
			}
		}

		public async Task DeleteAsync(int id)
		{
			var kpiToDelete = await _context.BiUserMasters.FirstAsync(x => x.Id == id);
			if (kpiToDelete != null)
			{
				_context.BiUserMasters.Remove(kpiToDelete);
				await _context.SaveChangesAsync();
			}
		}

		
	}
}
