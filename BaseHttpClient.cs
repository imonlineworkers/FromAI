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
        private readonly CookieService _cookieService = cookieService;

        public async Task<T?> GetDataByIdAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }

        public async Task<List<T>?> GetDataAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<List<T>>(cancellationToken);
        }

        public async Task<BaseResponse<T>?> PostAsync<T>(string endpoint, object? data, CancellationToken cancellationToken)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<BaseResponse<T>>(cancellationToken);
        }
        public async Task<BaseResponse<T>?> PostAsync<T>(string endpoint, CancellationToken cancellationToken)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<BaseResponse<T>>(cancellationToken);
        }

        private void SetAuthorizationHeader()
        {
            var token = _cookieService.GetToken();
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
