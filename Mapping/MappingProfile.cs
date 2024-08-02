using AutoMapper;
using BusinessIntelligence_API.Models;
using BusinessIntelligence_API.Service;

namespace BusinessIntelligence_API.Mapping
{
	public class MappingProfile:Profile
	{
        public MappingProfile()
        {
			CreateMap<BiPerformanceMetric, BiPerformanceMetricString>()
		   .ForMember(dest => dest.OverallPerformance,
					  opt => opt.MapFrom(src => src.OverallPerformance.ToString()));
			CreateMap<BiOtherCost, BiOtherCost_Mapper>()
				.ForMember(dest => dest.InvoiceDate, opt => opt.MapFrom(src => src.InvoiceDate.Date))
				.ForMember(dest => dest.PoDate, opt => opt.MapFrom(src => src.PoDate.HasValue ? src.PoDate.Value.Date : (DateTime?)null));
		}

		public class BiUserMasterMapper
		{
			public BiUserMaster_mapper Map(BiUserMaster user, BiRoleMaster biRole)
			{
				return new BiUserMaster_mapper
				{
					Id = user.Id,
					Firstname = user.Firstname,
					Lastname = user.Lastname,
					Username = user.Username,
					Password = EncodeandDecode.DecodePassword(user.Password),
					Emailid = user.Emailid,
					Displayname = user.Displayname,
					Signature = user.Signature,
					Userrole = user.Userrole,
					Activestatus = user.Activestatus,
					Createdtime = user.Createdtime,
					Updatedtime = user.Updatedtime,
					Rolename = biRole != null ? biRole.RoleName : null
				};
			}
		}

	}

}
