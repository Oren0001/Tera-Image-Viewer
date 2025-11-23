using Newtonsoft.Json;

namespace TeraImageViewer.Models {
    public class RefreshTokenRequest {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
