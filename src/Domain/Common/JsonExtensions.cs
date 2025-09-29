using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Common;

public static class JsonExtensions
{

    public static readonly JsonSerializerOptions Default = new JsonSerializerOptions().ConfigureSerializer();

    public static JsonSerializerOptions ConfigureSerializer(
        this JsonSerializerOptions options,
        Action<JsonSerializerOptions>? configure = null)
    {
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        configure?.Invoke(options);

        return options;
    }


}