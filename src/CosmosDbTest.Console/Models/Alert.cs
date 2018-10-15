using System;
using System.Collections.Generic;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class Alert
    {
        public Guid AlertId { get; set; }
        public Guid BatchId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string StatusCode { get; set; }
        public string StatusCodeDescription { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public bool Active { get; set; }
        public List<Triage> Triages { get; set; }
        public List<AlertComment> Comments { get; set; }
    }
}
