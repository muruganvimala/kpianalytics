using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiApiCallLog
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Method { get; set; }
        public string? Endpoint { get; set; }
        public string? RequestData { get; set; }
        public string? ResponseData { get; set; }
        public int? StatusCode { get; set; }
        public string? Exception { get; set; }
        public string? Environment { get; set; }
        public string? ServerName { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
