using System;
using Newtonsoft.Json;

namespace TeraImageViewer.Converters {
    public class SafeDoubleConverter : JsonConverter<double?> {
        public override double? ReadJson(JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            if (reader.TokenType == JsonToken.String) {
                var stringValue = reader.Value.ToString();
                if (double.TryParse(stringValue, out double result)) {
                    return result;
                }
                return null;
            }

            if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer) {
                return Convert.ToDouble(reader.Value);
            }

            reader.Skip();
            return null;
        }

        public override void WriteJson(JsonWriter writer, double? value, JsonSerializer serializer) {
            writer.WriteValue(value);
        }
    }

    public class SafeStringConverter : JsonConverter<string> {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined) {
                return null;
            }
            var stringValue = reader.Value?.ToString();
            return string.IsNullOrWhiteSpace(stringValue) ? null : stringValue;
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer) {
            writer.WriteValue(value);
        }
    }

    public class SafeIntArrayConverter : JsonConverter<int[]> {
        public override int[] ReadJson(JsonReader reader, Type objectType, int[] existingValue, bool hasExistingValue, JsonSerializer serializer) {
            try {
                return serializer.Deserialize<int[]>(reader);
            } catch {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, int[] value, JsonSerializer serializer) {
            serializer.Serialize(writer, value);
        }
    }
}
