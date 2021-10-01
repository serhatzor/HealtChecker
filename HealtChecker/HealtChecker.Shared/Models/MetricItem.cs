using System;
using System.Net;

namespace HealtChecker.Shared.Models
{
    public class MetricItem
    {
        public double ExecutionSeconds { get; set; }
        public Guid HealtCheckEndpointId { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public Guid ConnectedUserId { get; set; }
        public string HealtCheckUrl { get; set; }
        public Guid Id { get; set; }
        public string NotificationEmailAddress { get; set; }
        public string Name { get; set; }
        public int DownTimeAlertInterval { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
