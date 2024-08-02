using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiPerformanceMetric
    {
        public int Id { get; set; }
        public string? PublisherId { get; set; }
        public string? Acronym { get; set; }
        public string? Publisher { get; set; }
        public string? MonthYear { get; set; }
        public decimal? OverallPerformance { get; set; }
        public decimal? Schedule { get; set; }
        public decimal? Quality { get; set; }
        public decimal? Communication { get; set; }
        public decimal? CustomerSatisfaction { get; set; }
        public decimal? AccountManagement { get; set; }
        public decimal? Rft { get; set; }
        public decimal? PublicationSpeed { get; set; }
        public decimal? Feedback { get; set; }
        public decimal? AuthorSatisfaction { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
