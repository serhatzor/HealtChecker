using System;
using System.Net;

namespace HealtChecker.Service.Metrics.Data.Entities
{
    public class Metric : BaseEntity
    {
        public double ExecutionSeconds { get; set; }
        public Guid HealtCheckEndpointId { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public Guid ConnectedUserId { get; set; }
        public string HealtCheckUrl { get; set; }
    }
} 
