using HealtChecker.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealtChecker.UI.Services.Implementations
{
    public class MetricService
    {
        private HttpClient _httpClient { get; init; }
        public MetricService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResult<List<MetricItem>>> GetByHealtCheckEndPointIdOperation(Guid healtCheckEndpointId)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/Metrics/GetByHealtCheckEndpointId/{healtCheckEndpointId}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<List<MetricItem>> getResult = JsonConvert
                .DeserializeObject<ServiceResult<List<MetricItem>>>(getResponseJson);

            return getResult;
        }
    }
}
