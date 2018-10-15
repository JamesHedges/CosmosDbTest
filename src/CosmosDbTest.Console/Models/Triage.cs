using System;
using System.Collections.Generic;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class Triage
    {
        public Guid AnalystId { get; set; }
        public string TriageType { get; set; }
        public List<QuestionResponse> QuestionResponses { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
    }
}
