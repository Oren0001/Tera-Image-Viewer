using System;

namespace TeraImageViewer.Services {
    public class TokenStorage {
        private string _accessToken;
        private string _refreshToken;
        private DateTime _tokenExpiresAt;

        public string AccessToken {
            get => _accessToken;
            private set => _accessToken = value;
        }

        public string RefreshToken {
            get => _refreshToken;
            private set => _refreshToken = value;
        }

        public bool IsAccessTokenValid() {
            return !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiresAt;
        }

        public bool HasRefreshToken() {
            return !string.IsNullOrEmpty(_refreshToken);
        }

        public void SetTokens(string accessToken, string refreshToken, int expiresInSeconds) {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _tokenExpiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds - 60);
        }

        public void Clear() {
            _accessToken = null;
            _refreshToken = null;
            _tokenExpiresAt = DateTime.MinValue;
        }
    }
}
