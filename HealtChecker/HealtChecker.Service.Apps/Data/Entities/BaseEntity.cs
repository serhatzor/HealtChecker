using System;

namespace HealtChecker.Service.Metrics.Data.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedUserId { get; internal set; }
        public DateTime? UpdatedAt { get; internal set; }
        public Guid? UpdatedUserId { get; internal set; }
    }
}
