using AS400WebInterface.Domain.Common;

namespace AS400WebInterface.Application.Interfaces
{
    public interface IBaseHttpClient
    {
        Task<T?> GetDataByIdAsync<T>(string endpoint, CancellationToken cancellationToken);
        Task<List<T>?> GetDataAsync<T>(string endpoint, CancellationToken cancellationToken);
        Task<BaseResponse<T>?> PostAsync<T>(string endpoint, object? data, CancellationToken cancellationToken);
        Task<BaseResponse<T>?> PutAsync<T>(string endpoint, object? data, CancellationToken cancellationToken);
        Task<BaseResponse<T>?> DeleteAsync<T>(string endpoint, object? data, CancellationToken cancellationToken);
    }
}


using AS400WebInterface.Application.Interfaces;
using AS400WebInterface.Domain.Common;
using AS400WebInterface.Infrastructure.Services.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AS400WebInterface.Infrastructure.HttpClients
{
    public class BaseHttpClient(IHttpClientFactory httpClientFactory, CookieService cookieService) : IBaseHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

        public BaseHttpClient(IHttpClientFactory httpClientFactory, CookieService cookieService)
            : this(httpClientFactory, cookieService)
        {
            var token = cookieService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T?> GetDataByIdAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode) return default;

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }

        public async Task<List<T>?> GetDataAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode) return default;

            return await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken);
        }

        public async Task<BaseResponse<T>?> PostAsync<T>(string endpoint, object? data, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, cancellationToken);
            if (!response.IsSuccessStatusCode) return default;

            return await response.Content.ReadFromJsonAsync<BaseResponse<T>>(cancellationToken);
        }

        public async Task<BaseResponse<T>?> PutAsync<T>(string endpoint, object? data, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, cancellationToken);
            if (!response.IsSuccessStatusCode) return default;

            return await response.Content.ReadFromJsonAsync<BaseResponse<T>>(cancellationToken);
        }

        public async Task<BaseResponse<T>?> DeleteAsync<T>(string endpoint, object? data, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = JsonContent.Create(data)
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode) return default;

            return await response.Content.ReadFromJsonAsync<BaseResponse<T>>(cancellationToken);
        }
    }
}
