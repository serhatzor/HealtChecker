using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Implementations
{
    public class DetectDownTimeService : IDetectDownTimeService
    {
        private IEmailNotificationService _emailNotificationService { get; init; }
        private IMetricService _metricService { get; init; }
        public DetectDownTimeService(
            IEmailNotificationService emailNotificationService
            ,IMetricService metricService)
        {
            _emailNotificationService = emailNotificationService;
            _metricService = metricService;
        }

        public async Task DetectDownTime(MetricItem metric)
        {
            if ((int)metric.HttpStatusCode >= 200 && (int)metric.HttpStatusCode <= 299)
            {
                return;
            }

            ServiceResult<MetricItem> lastSuccessItem = await _metricService.GetLastSuccessMetric
                                                        (metric.HealtCheckEndpointId);

            if (lastSuccessItem != null && lastSuccessItem.Data != null)
            {
                if (lastSuccessItem.Data.CreatedAt.AddSeconds(metric.DownTimeAlertInterval) >= DateTime.UtcNow)
                {
                    return;
                }
            }

            _emailNotificationService.SendEmail(metric);
        }
    }
}
