using System.Xml.Linq; // Add this for XElement
using Microsoft.AspNetCore.DataProtection.Repositories; // Add this for IXmlRepository
using Domain.Images; // Import the domain models
using ImgGen.Application.Infrastructure;
using ImgGen.Application.Infrastructure.DataProtection;
using FluentValidation;
using JasperFx;
using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// Adds EF Core image storage services with PostgreSQL
    /// </summary>
    public static void AddImageStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ImageDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection") 
                ?? throw new Exception("Missing 'database' connection string"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "images");
                    npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                }
            );

            // Configure lazy loading for binary data
            options.UseLazyLoadingProxies();
            
            // Enable detailed errors in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });

        // Register the image storage service and validators
        services.AddScoped<Services.ImageStorageService>();
        services.AddScoped<IValidator<ImageMetaData>, ImageMetaDataValidator>();
    }
}
