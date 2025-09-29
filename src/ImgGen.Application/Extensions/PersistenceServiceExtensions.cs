using System.Xml.Linq; // Add this for XElement
using Microsoft.AspNetCore.DataProtection.Repositories; // Add this for IXmlRepository
using Domain.Images; // Import the domain models
using ImgGen.Application.Infrastructure;
using ImgGen.Application.Infrastructure.DataProtection;
using JasperFx;
using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace ImgGen.Application.Extensions;

public static class PersistenceServiceExtensions
{
    public static void AddDocumentStore(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ImgGenStoreOptions
        {
            AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate,
            Advanced =
            {
                Migrator = { TableCreation = CreationStyle.CreateIfNotExists }
            },
            OpenTelemetry = { TrackConnections = TrackLevel.Normal },
            DatabaseSchemaName = "imggen",
        };
        options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
        
        options.Connection(configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Missing 'database' connection string"));
        options.OpenTelemetry.TrackEventCounters();

        var martenCfg = services.AddMarten(options)
            .UseLightweightSessions()
            .UseNpgsqlDataSource()
            .InitializeWith()
            .ApplyAllDatabaseChangesOnStartup();

        services.AddSingleton<IXmlRepository, MartenXmlRepository>();
    }

    public static void AddRelationalStore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNpgsqlDataSource(
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Missing 'database' connection string"),
            builder =>
            {
                builder.EnableParameterLogging(configuration.GetValue("Marten:ParameterLogging", false));
            }
        );
    }
}
