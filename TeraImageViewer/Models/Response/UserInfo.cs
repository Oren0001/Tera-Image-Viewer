using Newtonsoft.Json;

namespace TeraImageViewer.Models {
    public class UserInfo {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }
}
