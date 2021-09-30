using System;

namespace HealtChecker.Service.Metrics.Data.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
