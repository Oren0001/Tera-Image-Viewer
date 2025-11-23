using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TeraImageViewer.Models;

namespace TeraImageViewer.Services {
    public class TeraApiService {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationService _authService;
        private readonly ILogger<TeraApiService> _logger;

        public TeraApiService(HttpClient httpClient, 
            AuthenticationService authService, 
            ILogger<TeraApiService> logger) {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        public async Task<ImageData> GetLatestImageAsync() {
            return await SendAuthenticatedRequestAsync<ImageData>("/api/image");
        }

        public async Task<InferenceResults> GetInferenceResultsAsync() {
            return await SendAuthenticatedRequestAsync<InferenceResults>("/api/results");
        }

        public async Task<UserInfo> GetCurrentUserAsync() {
            return await SendAuthenticatedRequestAsync<UserInfo>("/api/auth/me");
        }

        private async Task<T> SendAuthenticatedRequestAsync<T>(string endpoint) where T : class {
            try {
                if (!await _authService.EnsureValidTokenAsync()) {
                    return null;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    _authService.GetAccessToken()
                );

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode) {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug("Successfully retrieved data from {Endpoint}", endpoint);
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }

                _logger.LogWarning("Request to {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);
                return null;
            } catch (JsonException jsonEx) {
                _logger.LogError(jsonEx, "JSON parsing error for {Endpoint}", endpoint);
                return null;
            } catch (HttpRequestException httpEx) {
                _logger.LogError(httpEx, "HTTP request error for {Endpoint}", endpoint);
                return null;
            } catch (TaskCanceledException taskEx) {
                _logger.LogError(taskEx, "Request timeout for {Endpoint}", endpoint);
                return null;
            } catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error for {Endpoint}", endpoint);
                return null;
            }
        }
    }
}
