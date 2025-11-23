using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TeraImageViewer.Models;

namespace TeraImageViewer.Services {
    public class AuthenticationService {
        private readonly HttpClient _httpClient;
        private readonly TokenStorage _tokenStorage;
        private readonly ILogger<AuthenticationService> _logger;

        public event EventHandler<AuthenticationStatus> AuthenticationStatusChanged;

        public bool IsAuthenticated => _tokenStorage.IsAccessTokenValid() || _tokenStorage.HasRefreshToken();

        public AuthenticationService(HttpClient httpClient, 
            TokenStorage tokenStorage, 
            ILogger<AuthenticationService> logger) {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string username, string password) {
            try {
                var loginRequest = new LoginRequest {
                    Username = username,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);

                if (response.IsSuccessStatusCode) {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseJson);

                    _tokenStorage.SetTokens(
                        tokenResponse.AccessToken,
                        tokenResponse.RefreshToken,
                        tokenResponse.ExpiresIn
                    );

                    _logger.LogInformation("User logged in successfully");
                    AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.Authenticated);
                    return true;
                }

                _logger.LogWarning("Login failed: Invalid credentials or server error");
                return false;
            } catch (Exception ex) {
                _logger.LogError(ex, "Login error occurred");
                return false;
            }
        }

        public async Task<bool> RefreshTokenAsync() {
            if (!_tokenStorage.HasRefreshToken()) {
                return false;
            }

            AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.Authenticating);
            const int maxRetries = 3;
            const int retryDelayMs = 2000;

            try {
                for (int attempt = 1; attempt <= maxRetries; attempt++) {
                    try {
                        var refreshRequest = new RefreshTokenRequest {
                            RefreshToken = _tokenStorage.RefreshToken
                        };

                        var json = JsonConvert.SerializeObject(refreshRequest);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync("/api/auth/refresh", content);

                        if (response.IsSuccessStatusCode) {
                            var responseJson = await response.Content.ReadAsStringAsync();
                            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseJson);

                            _tokenStorage.SetTokens(
                                tokenResponse.AccessToken,
                                tokenResponse.RefreshToken,
                                tokenResponse.ExpiresIn
                            );

                            _logger.LogInformation("Token refreshed successfully");
                            AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.Authenticated);
                            return true;
                        }

                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                            response.StatusCode == System.Net.HttpStatusCode.Forbidden) {
                            _logger.LogWarning("Token refresh failed: Session expired (401/403)");
                            _tokenStorage.Clear();
                            AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.NotAuthenticated);
                            return false;
                        }

                        _logger.LogWarning("Token refresh attempt {Attempt}/{MaxRetries} failed: {StatusCode}", attempt, maxRetries, response.StatusCode);

                        if (attempt < maxRetries) {
                            await Task.Delay(retryDelayMs);
                        }
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Token refresh attempt {Attempt}/{MaxRetries} error", attempt, maxRetries);
                        if (attempt < maxRetries) {
                            await Task.Delay(retryDelayMs);
                        } else {
                            throw;
                        }
                    }
                }

                _logger.LogError("Token refresh failed after all retries");
                _tokenStorage.Clear();
                AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.NotAuthenticated);
                return false;
            } catch (Exception ex) {
                _logger.LogError(ex, "Token refresh fatal error occurred");
                _tokenStorage.Clear();
                AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.NotAuthenticated);
                return false;
            }
        }

        public async Task<bool> EnsureValidTokenAsync() {
            if (_tokenStorage.IsAccessTokenValid()) {
                return true;
            }

            if (_tokenStorage.HasRefreshToken()) {
                return await RefreshTokenAsync();
            }

            return false;
        }

        public string GetAccessToken() {
            return _tokenStorage.AccessToken;
        }

        public void Logout() {
            _tokenStorage.Clear();
            AuthenticationStatusChanged?.Invoke(this, AuthenticationStatus.NotAuthenticated);
        }
    }
}
