using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WasiApiGateway.JsonConverters
{
    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is JsonTokenType.Null) {
                return null;
            }

            if (reader.TokenType is not JsonTokenType.StartArray) {
                throw new Exception("unexcted input, this should be an array");
            }

            var byteList = new List<byte>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Number:
                        byteList.Add(reader.GetByte());
                        break;
                    case JsonTokenType.EndArray:
                        return byteList.ToArray();
                    case JsonTokenType.Comment:
                        // skip
                        break;
                    default:
                        throw new Exception(
                        string.Format(
                            "Unexpected token when reading bytes: {0}",
                            reader.TokenType));
                }
            }

            throw new Exception("Unexpected end when reading bytes.");
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();

            foreach (var b in value)
            {
                writer.WriteNumberValue(b);
            }

            writer.WriteEndArray();
        }
    }
}