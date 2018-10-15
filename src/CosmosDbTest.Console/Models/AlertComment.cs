using System;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class AlertComment
    {
        public Guid AlertId { get; set; }
        public Guid AnalystId { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public DateTime UpdateDateUtc { get; set; }
        public string Note { get; set; }
    }
}
