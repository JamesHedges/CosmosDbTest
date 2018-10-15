using System;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public bool Resonse { get; set; }
    }
}
