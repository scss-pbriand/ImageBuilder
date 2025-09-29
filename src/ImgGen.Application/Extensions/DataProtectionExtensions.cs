using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ImgGen.Application.Extensions;

public static class DataProtectionExtensions
{

    public static IDataProtectionBuilder PersistKeysToXmlRepository(this IDataProtectionBuilder builder)
    {
        builder.Services.AddSingleton(
            (Func<IServiceProvider, IConfigureOptions<KeyManagementOptions>>)
            (services =>
            {
                var xmlRepository = services.GetRequiredService<IXmlRepository>();
                return new ConfigureOptions<KeyManagementOptions>(delegate (KeyManagementOptions options)
                {
                    options.AutoGenerateKeys = true;
                    options.XmlRepository = xmlRepository;
                });
            }));
        return builder;
    }
}