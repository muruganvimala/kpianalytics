using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiQmsFeedback
    {
        public long Id { get; set; }
        public long? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public decimal? Pe { get; set; }
        public decimal? Ce { get; set; }
        public decimal? Mc { get; set; }
        public decimal? Typ { get; set; }
        public decimal? Pm { get; set; }
        public decimal? Xml { get; set; }
        public decimal? NotNterror { get; set; }
        public decimal? Technical { get; set; }
        public decimal? Positive { get; set; }
        public decimal? Total { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? MonthYear { get; set; }
	}
}
