using System;

namespace HealtChecker.Service.Metrics.Models
{
    public class HealtCheckEndpointModel : BaseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ConnectedUserId { get; set; }
        public string HealtCheckUrl { get; set; }
        public int IntervalSeconds { get; set; }
    }
}
