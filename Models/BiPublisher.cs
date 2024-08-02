using System;
using System.Collections.Generic;

namespace BusinessIntelligence_API.Models
{
    public partial class BiPublisher
    {
        public int Id { get; set; }
        public string PublisherName { get; set; } = null!;
        public string? Acronym { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
