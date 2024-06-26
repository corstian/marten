﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Marten.Services.Json;

/// <summary>
///     Taken from:
///     https://github.com/dotnet/runtime/blob/3e4a06c0e90e65c0ad514d8e2a9f93cb584d775a/src/libraries/System.Text.Json/tests/Serialization/CustomConverterTests.Object.cs#L267
///     A converter that converts System.Object similar to Newtonsoft's JSON.Net.
///     Only primitives are the same; arrays and objects do not result in the same types.
/// </summary>
internal class SystemObjectNewtonsoftCompatibleConverter: JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True)
        {
            return true;
        }

        if (reader.TokenType == JsonTokenType.False)
        {
            return false;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var l))
            {
                return l;
            }

            return reader.GetDouble();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            if (reader.TryGetDateTime(out var datetime))
            {
                return datetime;
            }

            return reader.GetString();
        }

        // Use JsonElement as fallback.
        // Newtonsoft uses JArray or JObject.
        using (var document = JsonDocument.ParseValue(ref reader))
        {
            return document.RootElement.Clone();
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        throw new InvalidOperationException("Should not get here.");
    }
}
