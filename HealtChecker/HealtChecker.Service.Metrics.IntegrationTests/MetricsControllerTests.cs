using FluentAssertions;
using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace HealtChecker.Service.Metrics.IntegrationTests
{
    public class MetricsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _webApplicationFactory { get; init; }
        private HttpClient _httpClient { get; init; }
        public MetricsControllerTests(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _webApplicationFactory.Server.BaseAddress = new Uri("http://localhost/");

            _httpClient = _webApplicationFactory.CreateClient();
        }

        private MetricItem GetNewModel()
        {
            MetricItem testModel = new MetricItem()
            {
                ConnectedUserId = Guid.NewGuid(),
                Content = "OK",
                Description = "200",
                ExecutionSeconds = 120,
                HealtCheckEndpointId = Guid.NewGuid(),
                HealtCheckUrl = "http://www.google.com",
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };

            return testModel;

        }

        private async Task<ServiceResult<Guid>> DoAddOperation(MetricItem testModel)
        {
            HttpResponseMessage addHttpResponse = await _httpClient.PostAsJsonAsync("/api/Metrics", testModel);

            addHttpResponse.EnsureSuccessStatusCode();

            string addresponseJson = await addHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<Guid> addResult = JsonConvert.DeserializeObject<ServiceResult<Guid>>(addresponseJson);

            addResult.Should().NotBeNull();
            addResult.Data.Should().NotBeEmpty();

            return addResult;
        }

        private async Task<List<MetricItem>> DoGetByUserIdOperation(Guid userId)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/Metrics/GetByConnectedUserId/{userId}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<List<MetricItem>> getResult = JsonConvert
                .DeserializeObject<ServiceResult<List<MetricItem>>>(getResponseJson);

            return getResult.Data;
        }

        private async Task<List<MetricItem>> DoGetByHealtCheckEndPointIdOperation(Guid healtCheckEndpointId)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/Metrics/GetByHealtCheckEndpointId/{healtCheckEndpointId}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<List<MetricItem>> getResult = JsonConvert
                .DeserializeObject<ServiceResult<List<MetricItem>>>(getResponseJson);

            return getResult.Data;
        }

        [Fact]
        public async Task GetByHealtCheckEndPointIdTest()
        {
            Guid hcEndpointId = Guid.NewGuid();
            ServiceResult<Guid> addResult = null;

            MetricItem testModel1 = GetNewModel();
            testModel1.HealtCheckEndpointId = hcEndpointId;
            addResult = await DoAddOperation(testModel1);
            testModel1.Id = addResult.Data;

            MetricItem testModel2 = GetNewModel();
            testModel2.HealtCheckEndpointId = hcEndpointId;
            addResult = await DoAddOperation(testModel2);
            testModel2.Id = addResult.Data;

            List<MetricItem> list = await DoGetByHealtCheckEndPointIdOperation(hcEndpointId);

            list.Should().HaveCount(2);

            list[0].CreatedAt = testModel1.CreatedAt;
            list[1].CreatedAt = testModel2.CreatedAt;
            list[0].Should().BeEquivalentTo(testModel1);
            list[1].Should().BeEquivalentTo(testModel2);
        }


        [Fact]
        public async Task GetByUserIdTest()
        {
            Guid connectedUserId = Guid.NewGuid();
            ServiceResult<Guid> addResult = null;

            MetricItem testModel1 = GetNewModel();
            testModel1.ConnectedUserId = connectedUserId;
            addResult = await DoAddOperation(testModel1);
            testModel1.Id = addResult.Data;

            MetricItem testModel2 = GetNewModel();
            testModel2.ConnectedUserId = connectedUserId;
            addResult = await DoAddOperation(testModel2);
            testModel2.Id = addResult.Data;

            List<MetricItem> list = await DoGetByUserIdOperation(connectedUserId);

            list.Should().HaveCount(2);

            list[0].CreatedAt = testModel1.CreatedAt;
            list[1].CreatedAt = testModel2.CreatedAt;
            list[0].Should().BeEquivalentTo(testModel1);
            list[1].Should().BeEquivalentTo(testModel2);
        }

    }
}
