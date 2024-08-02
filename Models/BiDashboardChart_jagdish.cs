using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BusinessIntelligence_API.Models
{
    [Keyless]
    public partial class BiDashboardChart
    {
        public string? PublisherName{ get; set; }
        public int? Count { get; set; }
    }

    [Keyless]
    public partial class BiDashboardMetricChart
    {
        public string? Metrics{ get; set; }
        public int? Count { get; set; }
    }

    [Keyless]
    public partial class BiDashboardMonthyearChart
    {
        public string PublisherName { get; set; }
        public string Metrics { get; set; }
        public string MonthYear { get; set; } 
        public int Target { get; set; }
        public int Actual {  get; set; }  

    }
}
