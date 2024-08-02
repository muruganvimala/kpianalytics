using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiForex
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal UsdInr { get; set; }
        public decimal GbpInr { get; set; }
        public decimal PhpInr { get; set; }
        public decimal UsdGbp { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

	public partial class BiForex_mapper
	{
		public string Date { get; set; }
		public string UsdInr { get; set; }
		public string GbpInr { get; set; }
		public string PhpInr { get; set; }
		public string UsdGbp { get; set; }
		public Boolean action { get; set; }
	}

	public class BiRawForex
    {
        public int Id { get; set; } = 0;
        public string Date { get; set; }
        public decimal UsdInr { get; set; }
        public decimal GbpInr { get; set; }
        public decimal PhpInr { get; set; }
        public decimal UsdGbp { get; set; }
    }
}
