using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeAutomation.Models.Mqtt.Serializers;

public sealed class DataPayloadSerializer : JsonConverter<DataPayload>
{
    // Our read case is unique since we have one value field that can be a string or float or int, so we should
    // always parse it as a string...but that isn't natively supported
    public override DataPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        // Read the properties of the object
        int? source = null;
        DateTimeOffset? time = null;
        string? contentType = null;
        string? dataValue = null;
        string? dataId = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString()!;
                reader.Read();
                string? val = null;

                switch (propertyName)
                {
                    case "source":
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            if (reader.TryGetInt32(out int intValue))
                            {
                                source = intValue;
                            }
                            else
                            {
                                throw new JsonException("Invalid board id format.");
                            }
                        }
                        else if (reader.TokenType == JsonTokenType.String)
                        {
                            val = reader.GetString();
                            if (int.TryParse(val, out int tmp))
                            {
                                source = tmp;
                            }
                            else
                            {
                                throw new JsonException($"Invalid board id format: {val}");
                            }
                        }
                        break;
                    case "time":
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            val = reader.GetString();
                            if (DateTimeOffset.TryParse(val, out DateTimeOffset parsedTime))
                            {
                                time = parsedTime;
                            }
                            else
                            {
                                throw new JsonException($"Invalid date format: {val}");
                            }
                        }
                        break;
                    case "content_type":
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            contentType = reader.GetString();
                        }
                        break;
                    case "data_id":
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            dataId = reader.GetString();
                        }
                        break;
                    case "data_value":
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            dataValue = reader.GetString();
                        }
                        else if (reader.TokenType == JsonTokenType.Number)
                        {
                            if (reader.TryGetInt32(out int intResult))
                            {
                                dataValue = intResult.ToString();
                            }
                            else if (reader.TryGetSingle(out float floatResult))
                            {
                                dataValue = floatResult.ToString();
                            }
                        }
                        break;
                }
            }
        }

        if (source is null || time is null || contentType is null || dataValue is null || dataId is null)
        {
            throw new JsonException("Missing required properties in DataPayload.");
        }

        return new DataPayload
        {
            BoardId = source.Value,
            ContentType = contentType,
            Timestamp = time.Value,
            SensorValue = dataValue,
            SensorId = dataId,
        };
    }

    // Nothing special here
    public override void Write(Utf8JsonWriter writer, DataPayload value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, options);
}