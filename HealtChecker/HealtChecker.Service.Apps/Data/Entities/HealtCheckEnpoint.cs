using System;

namespace HealtChecker.Service.Metrics.Data.Entities
{
    public class HealtCheckEnpoint : BaseEntity
    {
        public string Name { get; set; }
        public Guid ConnectedUserId { get; set; }
        public string HealtCheckUrl { get; set; }
        public int IntervalSeconds { get; set; }
        public DateTime NextExecutionTime { get; set; }
    }
}
