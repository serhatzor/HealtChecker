using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealtChecker.UI.Services.Implementations
{
    public class MetricService
    {
        private HttpClient _httpClient { get; init; }
        private IHttpContextAccessor _httpContextAccessor { get; init; }

        public MetricService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

        }

        private string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }


        public async Task<ServiceResult<List<MetricItem>>> GetByHealtCheckEndPointIdOperation(Guid healtCheckEndpointId)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/Metrics/GetByHealtCheckEndpointId/{healtCheckEndpointId}/{GetUserId()}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<List<MetricItem>> getResult = JsonConvert
                .DeserializeObject<ServiceResult<List<MetricItem>>>(getResponseJson);

            return getResult;
        }
    }
}
