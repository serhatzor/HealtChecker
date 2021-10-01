using System;

namespace HealtChecker.Service.HealtCheckEndpoints.Data.Entities
{
    public class HealtCheckEnpoint : BaseEntity
    {
        public string Name { get; set; }
        public Guid ConnectedUserId { get; set; }
        public string HealtCheckUrl { get; set; }
        public int IntervalSeconds { get; set; }
        public DateTime NextExecutionTime { get; set; }
        public string NotificationEmailAddress { get; set; }
    }
}
