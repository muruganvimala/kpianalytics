using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiQmsDatum
    {
        public long Id { get; set; }
        public long? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public decimal? EppFp { get; set; }
        public decimal? EppRev { get; set; }
        public decimal? Feedback { get; set; }
        public decimal? Epp { get; set; }
        public decimal? Rft { get; set; }
        public decimal? Positive { get; set; }
        public decimal? PeEpp { get; set; }
        public decimal? CeEpp { get; set; }
        public decimal? TypEpp { get; set; }
        public decimal? McEpp { get; set; }
        public decimal? Escalations { get; set; }
        public decimal? Ttp { get; set; }
        public decimal? ZeroError { get; set; }
        public decimal? AuthorSurvey { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? MonthYear { get; set; }
    }

    public class BiQmsFilter
    {
        public string? PublisherName { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
    }

    public class BiRawQmsData
    {
        public string? PublisherName { get; set; }
        public string? MonthYear { get; set; }
        public decimal? EppFp { get; set; }
        public decimal? EppRev { get; set; }
        public decimal? Feedback { get; set; }
        public decimal? Epp { get; set; }
        public decimal? Rft { get; set; }
        public decimal? Positive { get; set; }
        public decimal? PeEpp { get; set; }
        public decimal? CeEpp { get; set; }
        public decimal? TypEpp { get; set; }
        public decimal? McEpp { get; set; }
        public decimal? Escalations { get; set; }
        public decimal? Ttp { get; set; }
        public decimal? ZeroError { get; set; }
        public decimal? AuthorSurvey { get; set; }
    }

    public class BidQmsDataDashboardReport
    {
        public long? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public decimal? SumOfEPP_FP { get; set; }
        public decimal? SumOfEPP_Rev { get; set; }
        public decimal? SumOfFeedback { get; set; }
        public decimal? SumOfEPP { get; set; }
        public decimal? SumOfRFT { get; set; }
        public decimal? SumOfPositive { get; set; }
        public decimal? SumOfPE_EPP { get; set; }
        public decimal? SumOfCE_EPP { get; set; }
        public decimal? SumOfTYP_EPP { get; set; }
        public decimal? SumOfMC_EPP { get; set; }
        public decimal? SumOfEscalations { get; set; }
        public decimal? SumOfTTP { get; set; }
        public decimal? SumOfZeroError { get; set; }
        public decimal? SumOfAuthor_Survey { get; set; }

    }



}