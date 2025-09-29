using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ImgGen.Application.Infrastructure;

public static class ApplicationJsonSerializer
{

    public static readonly JsonSerializerOptions Default = new JsonSerializerOptions().ApplyDomainOptions();


    public static JsonSerializerOptions ApplyDomainOptions(
        this JsonSerializerOptions options,
        Action<JsonSerializerOptions>? configure = null)
    {
        options.ConfigureSerializer(
            x =>
            {
                x.TypeInfoResolver = JsonTypeInfoResolver.Combine(
                    new DefaultJsonTypeInfoResolver()
                );
            }
        );

        configure?.Invoke(options);
        return options;
    }


}