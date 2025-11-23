using Newtonsoft.Json;

namespace TeraImageViewer.Models {
    public class ImageData {
        [JsonProperty("image_id")]
        public string ImageId { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("image_data_base64")]
        public string ImageDataBase64 { get; set; }
    }
}
