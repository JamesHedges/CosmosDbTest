using System;
using System.Collections.Generic;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class AmlAnalyst
    {
        public Guid AnalystId { get; set; }
        public List<Guid> AlertIds { get; set; }
    }
}
