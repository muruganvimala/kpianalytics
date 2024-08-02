using BusinessIntelligence_API.Models;

namespace BusinessIntelligence_API.Repository
{
	public interface ILogServiceRepository
	{
		void InsertLog(string strValue, string strColor = "");
		Task InsertApiLog(BiApiCallLog biApiCallLog,int code);
	}
}
