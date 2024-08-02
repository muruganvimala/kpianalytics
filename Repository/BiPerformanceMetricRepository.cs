using BusinessIntelligence_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace BusinessIntelligence_API.Repository
{
	public class BiPerformanceMetricRepository:IBiPerformanceMetricRepository
	{
		private readonly JTSContext _context;
		private readonly IMapper _mapper;

		public BiPerformanceMetricRepository(JTSContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<BiPerformanceMetric> GetByIdAsync(int id)
		{
			return await _context.BiPerformanceMetrics.FindAsync(id);
		}

		public async Task<List<BiPerformanceMetric>> GetAllAsync()
		{
			return await _context.BiPerformanceMetrics.ToListAsync();
		}

		//GetAllConfigAsync
		public async Task<List<BiPerformanceMetricString>> GetAllConfigAsync()
		{
			//List<BiPerformanceMetric> biData = _context.BiPerformanceMetrics.ToList();
			List<BiPerformanceMetric> biData = _context.BiPerformanceMetrics.OrderBy(p=>p.Acronym).ToList();
			List<BiPerformanceMetricString> biDataString = _mapper.Map<List<BiPerformanceMetricString>>(biData);

			foreach (var bi in biDataString)
			{				
				var publisher = _context.BiPublisherConfigs.Where(item => item.PublisherName == bi.Acronym).FirstOrDefault();
				if (publisher != null)
				{
					bi.OverallPerformance = UpdateMetricIfFalse(publisher.OverallPerfomanceRequired, bi.OverallPerformance);
					bi.Schedule = UpdateMetricIfFalse(publisher.ScheduleRequired, bi.Schedule);
					bi.Quality = UpdateMetricIfFalse(publisher.QualityRequired, bi.Quality);
					bi.Communication = UpdateMetricIfFalse(publisher.CommunicationRequired, bi.Communication);
					bi.CustomerSatisfaction = UpdateMetricIfFalse(publisher.CustomerSatisfactionRequired, bi.CustomerSatisfaction);
					bi.AccountManagement = UpdateMetricIfFalse(publisher.AccountManagementRequired, bi.AccountManagement);
					bi.Rft = UpdateMetricIfFalse(publisher.RftRequired, bi.Rft);
					bi.PublicationSpeed = UpdateMetricIfFalse(publisher.PublicationSpeedRequired, bi.PublicationSpeed);
					bi.Feedback = UpdateMetricIfFalse(publisher.FeedbackRequired, bi.Feedback);
					bi.AuthorSatisfaction = UpdateMetricIfFalse(publisher.AccountManagementRequired, bi.AuthorSatisfaction);
				}
				if (bi.OverallPerformance != null && bi.OverallPerformance.EndsWith(".00")) bi.OverallPerformance = TrimValue(bi.OverallPerformance);				
				if (bi.OverallPerformance != null) bi.OverallPerformance = TrimeZeroend(bi.OverallPerformance);

				if (bi.Schedule != null && bi.Schedule.EndsWith(".00")) bi.Schedule = TrimValue(bi.Schedule);
				if (bi.Schedule != null) bi.Schedule = TrimeZeroend(bi.Schedule);

				if (bi.Quality != null && bi.Quality.EndsWith(".00")) bi.Quality = TrimValue(bi.Quality);
				if (bi.Quality != null) bi.Quality = TrimeZeroend(bi.Quality);

				if (bi.Communication != null && bi.Communication.EndsWith(".00")) bi.Communication = TrimValue(bi.Communication);
				if (bi.Communication != null) bi.Communication = TrimeZeroend(bi.Communication);

				if (bi.CustomerSatisfaction != null && bi.CustomerSatisfaction.EndsWith(".00")) bi.CustomerSatisfaction = TrimValue(bi.CustomerSatisfaction);
				if (bi.CustomerSatisfaction != null) bi.CustomerSatisfaction = TrimeZeroend(bi.CustomerSatisfaction);

				if (bi.AccountManagement != null && bi.AccountManagement.EndsWith(".00")) bi.AccountManagement = TrimValue(bi.AccountManagement);
				if (bi.AccountManagement != null) bi.AccountManagement = TrimeZeroend(bi.AccountManagement);

				if (bi.Rft != null && bi.Rft.EndsWith(".00")) bi.Rft = TrimValue(bi.Rft);
				if (bi.Rft != null) bi.Rft = TrimeZeroend(bi.Rft);

				if (bi.PublicationSpeed != null && bi.PublicationSpeed.EndsWith(".00")) bi.PublicationSpeed = TrimValue(bi.PublicationSpeed);
				if (bi.PublicationSpeed != null) bi.PublicationSpeed = TrimeZeroend(bi.PublicationSpeed);

				if (bi.Feedback != null && bi.Feedback.EndsWith(".00")) bi.Feedback = TrimValue(bi.Feedback);
				if (bi.Feedback != null) bi.Feedback = TrimeZeroend(bi.Feedback);

				if (bi.AuthorSatisfaction != null && bi.AuthorSatisfaction.EndsWith(".00")) bi.AuthorSatisfaction = TrimValue(bi.AuthorSatisfaction);
				if (bi.AuthorSatisfaction != null) bi.AuthorSatisfaction = TrimeZeroend(bi.AuthorSatisfaction);

				DateTime date = DateTime.ParseExact(bi.MonthYear, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
				bi.monthyearinDate = date.ToString("dd-MM-yyyy");

			}
			return biDataString;
		}

		private string TrimeZeroend(string overallPerformance)
		{
			if (Regex.IsMatch(overallPerformance, "\\.[1-9]+(0)+"))
			{
				overallPerformance = Regex.Replace(overallPerformance, "(\\.[1-9]+)(0)+", "$1");
			}
			return overallPerformance;
		}

		string UpdateMetricIfFalse(bool? requirement, string metric)
		{
			return (bool)!requirement ? "NA" : metric;
		}

		static string TrimValue(string stringValue)
		{			

			// Check if the string representation ends with ".00", if yes, remove it
			if (stringValue.EndsWith(".00"))
			{
				return stringValue.Substring(0, stringValue.Length - 3); // Trim ".00"
			}
			else
			{
				return stringValue; // No need to trim
			}
		}

		public async Task InsertAsync(BiPerformanceMetric biPerformanceMetric)
		{
			biPerformanceMetric.InsertedDate =DateTime.Now;
			var publisher =  _context.BiPublishers.Where(item => item.Id == Convert.ToInt32(biPerformanceMetric.PublisherId)).FirstOrDefault();
			biPerformanceMetric.Publisher = publisher.PublisherName;
			biPerformanceMetric.Acronym = publisher.Acronym;

			await _context.BiPerformanceMetrics.AddAsync(biPerformanceMetric);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var kpiToDelete = await _context.BiPerformanceMetrics.FindAsync(id);

			if (kpiToDelete != null)
			{
				_context.BiPerformanceMetrics.Remove(kpiToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task UpdateAsync(BiPerformanceMetric biPerformanceMetric)
		{
			var existingKpi = await _context.BiPerformanceMetrics.FindAsync(biPerformanceMetric.Id);

			if (existingKpi != null)
			{
				if (existingKpi.MonthYear != biPerformanceMetric.MonthYear)
					existingKpi.MonthYear = biPerformanceMetric.MonthYear;
				if (existingKpi.OverallPerformance != biPerformanceMetric.OverallPerformance)
					existingKpi.OverallPerformance = biPerformanceMetric.OverallPerformance;
				if (existingKpi.Schedule != biPerformanceMetric.Schedule)
					existingKpi.Schedule = biPerformanceMetric.Schedule;
				if (existingKpi.Quality != biPerformanceMetric.Quality)
					existingKpi.Quality = biPerformanceMetric.Quality;
				if (existingKpi.Communication != biPerformanceMetric.Communication)
					existingKpi.Communication = biPerformanceMetric.Communication;
				if (existingKpi.CustomerSatisfaction != biPerformanceMetric.CustomerSatisfaction)
					existingKpi.CustomerSatisfaction = biPerformanceMetric.CustomerSatisfaction;
				if (existingKpi.AccountManagement != biPerformanceMetric.AccountManagement)
					existingKpi.AccountManagement = biPerformanceMetric.AccountManagement;
				if (existingKpi.Rft != biPerformanceMetric.Rft)
					existingKpi.Rft = biPerformanceMetric.Rft;
				if (existingKpi.PublicationSpeed != biPerformanceMetric.PublicationSpeed)
					existingKpi.PublicationSpeed = biPerformanceMetric.PublicationSpeed;
				if (existingKpi.Feedback != biPerformanceMetric.Feedback)
					existingKpi.Feedback = biPerformanceMetric.Feedback;
				if (existingKpi.AuthorSatisfaction != biPerformanceMetric.AuthorSatisfaction)
					existingKpi.AuthorSatisfaction = biPerformanceMetric.AuthorSatisfaction;

				await _context.SaveChangesAsync();
			}
			else
			{
				// Handle the case where the entity with the given ID is not found
				throw new Exception($"Record with ID {biPerformanceMetric.Id} not found.");
			}
		}
	}
}
