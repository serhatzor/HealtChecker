using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealtChecker.UI.Services.Implementations
{
    public class HealtCheckService
    {
        private HttpClient _httpClient { get; init; }
        private IHttpContextAccessor _httpContextAccessor { get; init; }
        public HealtCheckService(
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

        public async Task<ServiceResult<List<HealtCheckEndpointModel>>> GetHealtCheckEndpoints()
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync($"/api/HealtCheckEndpoint/GetByUserId/{GetUserId()}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<List<HealtCheckEndpointModel>> getResult = JsonConvert
                .DeserializeObject<ServiceResult<List<HealtCheckEndpointModel>>>(getResponseJson);

            return getResult;
        }

        public async Task<ServiceResult<Guid>> InsertHealtCheckEndPoint(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            healtCheckEndpointModel.OperatedUserId = Guid.Parse(GetUserId());
            healtCheckEndpointModel.ConnectedUserId = healtCheckEndpointModel.OperatedUserId;

            HttpResponseMessage addHttpResponse = await _httpClient.PostAsJsonAsync("/api/HealtCheckEndpoint", healtCheckEndpointModel);

            addHttpResponse.EnsureSuccessStatusCode();

            string addresponseJson = await addHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<Guid> addResult = JsonConvert.DeserializeObject<ServiceResult<Guid>>(addresponseJson);

            return addResult;

        }

        public async Task<ServiceResult<bool>> UpdateHealtCheckEndPoint(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            healtCheckEndpointModel.OperatedUserId = Guid.Parse(GetUserId());
            healtCheckEndpointModel.ConnectedUserId = healtCheckEndpointModel.OperatedUserId;

            HttpResponseMessage httpResponse = await _httpClient.PutAsJsonAsync("/api/HealtCheckEndpoint", healtCheckEndpointModel);

            httpResponse.EnsureSuccessStatusCode();

            string responseJSon = await httpResponse.Content.ReadAsStringAsync();

            ServiceResult<bool> result = JsonConvert.DeserializeObject<ServiceResult<bool>>(responseJSon);

            return result;

        }

        public async Task<ServiceResult<HealtCheckEndpointModel>> GetHealtCheckById(Guid id)
        {
            HttpResponseMessage getHttpResponse = await _httpClient.GetAsync(
                $"/api/HealtCheckEndpoint/GetById/{id}/{GetUserId()}");

            getHttpResponse.EnsureSuccessStatusCode();

            string getResponseJson = await getHttpResponse.Content.ReadAsStringAsync();


            ServiceResult<HealtCheckEndpointModel> getResult = JsonConvert
                .DeserializeObject<ServiceResult<HealtCheckEndpointModel>>(getResponseJson);

            return getResult;
        }

        public async Task<ServiceResult<bool>> Delete(Guid id)
        {
            HttpResponseMessage deleteHttpResponse = await _httpClient.DeleteAsync($"/api/HealtCheckEndpoint/{id}/{GetUserId()}");

            deleteHttpResponse.EnsureSuccessStatusCode();

            string responseJson = await deleteHttpResponse.Content.ReadAsStringAsync();

            ServiceResult<bool> deleteResult = JsonConvert.DeserializeObject<ServiceResult<bool>>(responseJson);

            return deleteResult;
        }

    }
}
