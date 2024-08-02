using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessIntelligence_API.Repository
{

	public class SettingRepository : ISettingRepository
	{
		private readonly JTSContext _context;
        public SettingRepository(JTSContext context)
        {
			_context = context;
		}

		public async Task<BiPublisherConfig> GetByIdAsync(int publisherId)
		{
			return await _context.BiPublisherConfigs.FirstAsync(x => x.PublisherId == publisherId);
			//return await _context.BiPublisherConfigs.FirstAsync(x => x.Id == Id);
		}

		public async Task DeleteAsync(int PublisherId)
		{
			var kpiToDelete = await _context.BiPublisherConfigs.FirstAsync(x => x.Id == PublisherId);

			if (kpiToDelete != null)
			{
				_context.BiPublisherConfigs.Remove(kpiToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<BiPublisherConfig>> GetAllAsync()
		{
			//return await _context.BiPublisherConfigs.ToListAsync();
			return await _context.BiPublisherConfigs.OrderBy(p=>p.PublisherName).ToListAsync();
		}
		

		public async Task InsertAsync(BiPublisherConfig settingRepository)
		{
			settingRepository.InsertedDate = DateTime.Now;
			await _context.BiPublisherConfigs.AddAsync(settingRepository);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(BiPublisherConfig settingRepository)
		{
			//var existingKpi = await _context.BiPublisherConfigs.FirstAsync(x => x.PublisherId == settingRepository.PublisherId);
			var existingKpi = await _context.BiPublisherConfigs.FirstAsync(x => x.Id == settingRepository.Id);
			if (existingKpi != null)
			{				
				existingKpi.PublisherName = settingRepository.PublisherName;

				existingKpi.OverallPerfomanceRequired = settingRepository.OverallPerfomanceRequired;
				existingKpi.ScheduleRequired = settingRepository.ScheduleRequired;
				existingKpi.QualityRequired = settingRepository.QualityRequired;
				existingKpi.CommunicationRequired = settingRepository.CommunicationRequired;
				existingKpi.CustomerSatisfactionRequired = settingRepository.CustomerSatisfactionRequired;
				existingKpi.AccountManagementRequired = settingRepository.AccountManagementRequired;
				existingKpi.RftRequired = settingRepository.RftRequired;
				existingKpi.PublicationSpeedRequired = settingRepository.PublicationSpeedRequired;
				existingKpi.FeedbackRequired = settingRepository.FeedbackRequired;
				existingKpi.AuthorsatisficationRequired = settingRepository.AuthorsatisficationRequired;

				// Update metrics properties
				existingKpi.OverallPerfomanceMetrics = settingRepository.OverallPerfomanceMetrics;
				existingKpi.ScheduleMetrics = settingRepository.ScheduleMetrics;
				existingKpi.QualityMetrics = settingRepository.QualityMetrics;
				existingKpi.CommunicationMetrics = settingRepository.CommunicationMetrics;
				existingKpi.CustomerSatisfactionMetrics = settingRepository.CustomerSatisfactionMetrics;
				existingKpi.AccountManagementMetrics = settingRepository.AccountManagementMetrics;
				existingKpi.RftMetrics = settingRepository.RftMetrics;
				existingKpi.PublicationSpeedMetrics = settingRepository.PublicationSpeedMetrics;
				existingKpi.FeedbackMetrics = settingRepository.FeedbackMetrics;
				existingKpi.AuthorsatisficationMetrics = settingRepository.AuthorsatisficationMetrics;
				;

				// Update action properties
				existingKpi.OverallPerfomanceAction = settingRepository.OverallPerfomanceAction;
				existingKpi.ScheduleAction = settingRepository.ScheduleAction;
				existingKpi.QualityAction = settingRepository.QualityAction;
				existingKpi.CommunicationAction = settingRepository.CommunicationAction;
				existingKpi.CustomerSatisfactionAction = settingRepository.CustomerSatisfactionAction;
				existingKpi.AccountManagementAction = settingRepository.AccountManagementAction;
				existingKpi.RftAction = settingRepository.RftAction;
				existingKpi.PublicationSpeedAction = settingRepository.PublicationSpeedAction;
				existingKpi.FeedbackAction = settingRepository.FeedbackAction;
				existingKpi.AuthorsatisficationAction = settingRepository.AuthorsatisficationAction;

				existingKpi.UpdatedDate = DateTime.Now;
				await _context.SaveChangesAsync();
			}
			else
			{
				// Handle the case where the entity with the given ID is not found
				throw new Exception($"Record with PublisherName {settingRepository.PublisherName} not found.");
			}
		}
	}
}
