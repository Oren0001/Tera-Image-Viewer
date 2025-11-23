using Newtonsoft.Json;
using TeraImageViewer.Converters;

namespace TeraImageViewer.Models {
    public class InferenceResults {
        [JsonProperty("image_id")]
        public string ImageId { get; set; }

        [JsonProperty("intensity_average")]
        [JsonConverter(typeof(SafeDoubleConverter))]
        public double? IntensityAverage { get; set; }

        [JsonProperty("focus_score")]
        [JsonConverter(typeof(SafeDoubleConverter))]
        public double? FocusScore { get; set; }

        [JsonProperty("classification_label")]
        [JsonConverter(typeof(SafeStringConverter))]
        public string ClassificationLabel { get; set; }

        [JsonProperty("histogram")]
        [JsonConverter(typeof(SafeIntArrayConverter))]
        public int[] Histogram { get; set; }
    }
}
