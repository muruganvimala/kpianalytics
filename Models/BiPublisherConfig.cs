using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiPublisherConfig
    {
        public int Id { get; set; }
        public long? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public bool? OverallPerfomanceRequired { get; set; }
        public bool? ScheduleRequired { get; set; }
        public bool? QualityRequired { get; set; }
        public bool? CommunicationRequired { get; set; }
        public bool? CustomerSatisfactionRequired { get; set; }
        public bool? AccountManagementRequired { get; set; }
        public bool? RftRequired { get; set; }
        public bool? PublicationSpeedRequired { get; set; }
        public bool? FeedbackRequired { get; set; }
        public bool? AuthorsatisficationRequired { get; set; }
        public double? OverallPerfomanceMetrics { get; set; }
        public double? ScheduleMetrics { get; set; }
        public double? QualityMetrics { get; set; }
        public double? CommunicationMetrics { get; set; }
        public double? CustomerSatisfactionMetrics { get; set; }
        public double? AccountManagementMetrics { get; set; }
        public double? RftMetrics { get; set; }
        public double? PublicationSpeedMetrics { get; set; }
        public double? FeedbackMetrics { get; set; }
        public double? AuthorsatisficationMetrics { get; set; }
        public string? OverallPerfomanceAction { get; set; }
        public string? ScheduleAction { get; set; }
        public string? QualityAction { get; set; }
        public string? CommunicationAction { get; set; }
        public string? CustomerSatisfactionAction { get; set; }
        public string? AccountManagementAction { get; set; }
        public string? RftAction { get; set; }
        public string? PublicationSpeedAction { get; set; }
        public string? FeedbackAction { get; set; }
        public string? AuthorsatisficationAction { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
