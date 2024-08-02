using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BusinessIntelligence_API.Models
{
	[Keyless]
	public partial class SSP_BIReport_DetailedResult
	{
		public string PublisherName { get; set; }
		public string Metrics { get; set; }
		public string MonthYear { get; set; }
		public decimal Target { get; set; }
		public decimal? Actual { get; set; }
	}

	[Keyless]
	public partial class SSP_BIReport_Shortfall
	{
		public string PublisherName { get; set; }
		public string Metrics { get; set; }
		public string MonthYear { get; set; }
		public Int32 Target { get; set; }  // Change from Int32 to decimal
		public Int32? Actual { get; set; }  // Change from Int32? to decimal?
	}

	public class SSP_BIReportFormattedData
	{
		public List<string> Metrics { get; set; }
		public List<decimal> Target { get; set; }
		public List<decimal?> Actual { get; set; }
		public List<BiGroup> BiGroups { get; set; }
	}

	public class BiGroup
	{
		public string Title { get; set; }
		public int Cols { get; set; }
	}

	public class MonthYear
	{
		private DateTime _value;

		public MonthYear()
		{
			_value = DateTime.MinValue;
		}

		public MonthYear(DateTime value)
		{
			_value = value;
		}

		public static implicit operator MonthYear(string value)
		{
			return new MonthYear(DateTime.ParseExact(value, "MMM yyyy", CultureInfo.InvariantCulture));
		}

		public static implicit operator DateTime(MonthYear monthYear)
		{
			return monthYear._value;
		}

		public override string ToString()
		{
			return _value.ToString("MMM yyyy");
		}
	}

}
