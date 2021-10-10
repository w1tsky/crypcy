using System;
using System.Buffers;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace crypcy.shared
{
    public class IPEndPointConverter : JsonConverter<IPEndPoint>
    {
        public override IPEndPoint Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                IPEndPoint.Parse(reader.GetString());

        public override void Write(
            Utf8JsonWriter writer,
            IPEndPoint ipEndpoimtValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(ipEndpoimtValue.ToString());
    }
}