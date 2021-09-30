using FluentAssertions;
using HealtChecker.Service.HealtCheckEndpoints;
using HealtChecker.Service.HealtCheckEndpoints.Models;
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
    public class HealtCheckEndpointControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _webApplicationFactory { get; init; }
        private HttpClient _httpClient { get; init; }
        public HealtCheckEndpointControllerTests(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _webApplicationFactory.Server.BaseAddress = new Uri("http://localhost/");

            _httpClient = _webApplicationFactory.CreateClient();
        }

        private HealtCheckEndpointModel GetNewModel()
        {
            HealtCheckEndpointModel testModel = new HealtCheckEndpointModel()
            {
                ConnectedUserId = Guid.NewGuid(),
                HealtCheckUrl = "http://www.google.com",
                Name = "test endpoint",
                OperatedUserId = Guid.NewGuid(),
                IntervalSeconds = 10
            };
            return testModel;
        }

        private async Task<ServiceResult<Guid>> DoAddOperation(HealtCheckEndpointModel testModel)
        {
            HttpResponseMessage addHttpResponse = await _httpClient.PostAsJsonAsync("/api/HealtCheckEndpoint", testModel);

            addHttpResponse.EnsureSuccessStatusCode();

            string addresponseJson = await addHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<Guid> addResult = JsonConvert.DeserializeObject<ServiceResult<Guid>>(addresponseJson);

            addResult.Should().NotBeNull();
            addResult.Data.Should().NotBeEmpty();

            return addResult;
        }

        private async Task<ServiceResult<bool>> DoUpdateOperation(HealtCheckEndpointModel testModel)
        {
            HttpResponseMessage updateHttpResponse = await _httpClient.PutAsJsonAsync("/api/HealtCheckEndpoint", testModel);

            updateHttpResponse.EnsureSuccessStatusCode();

            string responseJson = await updateHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<bool> updateResult = JsonConvert.DeserializeObject<ServiceResult<bool>>(responseJson);

            updateResult.Should().NotBeNull();
            updateResult.Data.Should().BeTrue();

            return updateResult;
        }

        private async Task<ServiceResult<bool>> DoDeleteOperation(Guid id)
        {
            HttpResponseMessage deleteHttpResponse = await _httpClient.DeleteAsync($"/api/HealtCheckEndpoint/{id}");

            deleteHttpResponse.EnsureSuccessStatusCode();

            string responseJson = await deleteHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<bool> deleteResult = JsonConvert.DeserializeObject<ServiceResult<bool>>(responseJson);

            deleteResult.Should().NotBeNull();
            deleteResult.Data.Should().BeTrue();

            return deleteResult;
        }



        private async Task<HealtCheckEndpointModel> DoGetOperation(HealtCheckEndpointModel testModel)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/HealtCheckEndpoint/GetById/{testModel.Id}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<HealtCheckEndpointModel> getResult = JsonConvert
                .DeserializeObject<ServiceResult<HealtCheckEndpointModel>>(getResponseJson);

            return getResult.Data;
        }

        private async Task<List<HealtCheckEndpointModel>> DoGetByUserIdOperation(Guid userId)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/HealtCheckEndpoint/GetByUserId/{userId}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<List<HealtCheckEndpointModel>> getResult = JsonConvert
                .DeserializeObject<ServiceResult<List<HealtCheckEndpointModel>>>(getResponseJson);

            return getResult.Data;
        }


        [Fact]
        public async Task CreateAppTest()
        {
            HealtCheckEndpointModel testModel = GetNewModel();

            ServiceResult<Guid> addResult = await DoAddOperation(testModel);

            testModel.Id = addResult.Data;

            HealtCheckEndpointModel actualValue = await DoGetOperation(testModel);

            testModel.Should().BeEquivalentTo(actualValue);
        }

        [Fact]
        public async Task UpdateAppTest()
        {
            HealtCheckEndpointModel testModel = GetNewModel();
            Guid firstConnectedUserId = testModel.ConnectedUserId;

            ServiceResult<Guid> addResult = await DoAddOperation(testModel);
            testModel.Id = addResult.Data;


            testModel.Name = "test 2";
            testModel.HealtCheckUrl = "www.google.com";

            await DoUpdateOperation(testModel);

            HealtCheckEndpointModel actualValue = await DoGetOperation(testModel);
            testModel.Should().BeEquivalentTo(actualValue);

            testModel.ConnectedUserId = Guid.NewGuid();
            testModel.OperatedUserId = Guid.NewGuid();
            testModel.Name = "test 3";
            await DoUpdateOperation(testModel);

            actualValue = await DoGetOperation(testModel);

            actualValue.OperatedUserId.Should().Be(testModel.OperatedUserId);
            actualValue.ConnectedUserId.Should().Be(firstConnectedUserId);
        }

        [Fact]
        public async Task GetByUserIdTest()
        {
            Guid connectedUserId = Guid.NewGuid();

            HealtCheckEndpointModel testModel1 = GetNewModel();
            testModel1.ConnectedUserId = connectedUserId;
            ServiceResult<Guid> addResult1 = await DoAddOperation(testModel1);
            testModel1.Id = addResult1.Data;

            HealtCheckEndpointModel testModel2 = GetNewModel();
            testModel2.ConnectedUserId = connectedUserId;
            ServiceResult<Guid> addResult2 = await DoAddOperation(testModel2);
            testModel2.Id = addResult2.Data;

            HealtCheckEndpointModel testModel3 = GetNewModel();
            testModel3.ConnectedUserId = connectedUserId;
            ServiceResult<Guid> addResult3 = await DoAddOperation(testModel3);
            testModel3.Id = addResult3.Data;

            List<HealtCheckEndpointModel> list = await DoGetByUserIdOperation(connectedUserId);

            list.Should().HaveCount(3);

            list[0].Should().BeEquivalentTo(testModel1);
            list[1].Should().BeEquivalentTo(testModel2);
            list[2].Should().BeEquivalentTo(testModel3);
        }


        [Fact]
        public async Task DeleteAppTest()
        {
            Guid connectedUserId = Guid.NewGuid();

            HealtCheckEndpointModel testModel1 = GetNewModel();
            testModel1.ConnectedUserId = connectedUserId;
            ServiceResult<Guid> addResult1 = await DoAddOperation(testModel1);
            testModel1.Id = addResult1.Data;

            HealtCheckEndpointModel testModel2 = GetNewModel();
            testModel2.ConnectedUserId = connectedUserId;
            ServiceResult<Guid> addResult2 = await DoAddOperation(testModel2);
            testModel2.Id = addResult2.Data;

            HealtCheckEndpointModel testModel3 = GetNewModel();
            testModel3.ConnectedUserId = connectedUserId;
            ServiceResult<Guid> addResult3 = await DoAddOperation(testModel3);
            testModel3.Id = addResult3.Data;

            await DoDeleteOperation(testModel1.Id);

            List<HealtCheckEndpointModel> list = await DoGetByUserIdOperation(connectedUserId);

            list.Should().HaveCount(2);

            list[0].Should().BeEquivalentTo(testModel2);
            list[1].Should().BeEquivalentTo(testModel3);
        }
    }
}
