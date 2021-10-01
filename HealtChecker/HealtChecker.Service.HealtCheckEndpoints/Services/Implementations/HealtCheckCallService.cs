using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Shared.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Implementations
{
    public class HealtCheckCallService : IHealtCheckCallService
    {
        public async Task<MetricItem> GetMetric(HealtCheckEndpointModel healtCheckEndpoint)
        {
            MetricItem metric = new MetricItem();
            using (HttpClient client = new HttpClient())
            {
                DateTime startTime = DateTime.UtcNow;

                HttpResponseMessage getHttpResponse = null;
                try
                {
                    getHttpResponse = await client.GetAsync(healtCheckEndpoint.HealtCheckUrl);
                    getHttpResponse.EnsureSuccessStatusCode();
                    metric.Description = getHttpResponse.ReasonPhrase;
                }
                catch (Exception ex)
                {
                    metric.Description = ex.Message;
                }

                TimeSpan timeSpan = DateTime.UtcNow - startTime;

                metric.ExecutionSeconds = timeSpan.TotalSeconds;
                metric.HealtCheckEndpointId = healtCheckEndpoint.Id;
                metric.HttpStatusCode = getHttpResponse == null ?
                    HttpStatusCode.NoContent : getHttpResponse.StatusCode;
                metric.ConnectedUserId = healtCheckEndpoint.ConnectedUserId;
                metric.HealtCheckUrl = healtCheckEndpoint.HealtCheckUrl;

            }

            return metric;

        }
    }
}
