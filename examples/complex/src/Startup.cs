﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using TinyFp.Extensions;
using TinyFpTest.Configuration;
using TinyFpTest.Services;
using TinyFpTest.Services.Api;
using static TinyFp.Extensions.FunctionalExtension;

namespace TinyFpTest.Complex
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        protected IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
            => services
                .Tee(_ => _.AddControllers())
                .Tee(_ => _.AddHttpClient())
                .Tee(_ => Configuration
                            .GetSection(typeof(ProductsApiConfiguration).Name)
                            .Get<ProductsApiConfiguration>()
                            .Tee(_ => services.AddSingleton(_)))
                .Tee(_ => _.AddSingleton<SearchService>())
                .Tee(_ => _.AddSingleton(_ =>
                                new CachedSearchService(_.GetRequiredService<SearchService>())))
                .Tee(_ => _.AddSingleton<ISearchService>(_ =>
                                new LoggedSearchService(_.GetRequiredService<CachedSearchService>())))
                .Tee(_ => _.AddSingleton<IApiClient>(_ =>
                            new ApiClient(() => _.GetRequiredService<IHttpClientFactory>().CreateClient())))
                .Tee(_ => InitializeSerilog(_));

        private void InitializeSerilog(IServiceCollection services)
            => Configuration
                .GetSection(typeof(SerilogConfiguration).Name).Get<SerilogConfiguration>()
                .Map(_ => (serilogConfig: _, loggerConfig: new LoggerConfiguration()))
                .Tee(_ => InitializeConfiguration(_.loggerConfig, _.serilogConfig))
                .Map(_ => _.loggerConfig.CreateLogger())
                .Tee(_ => services.AddSingleton<ILogger>(_));

        private static (LoggerConfiguration, SerilogConfiguration) InitializeConfiguration(LoggerConfiguration loggerConfig, SerilogConfiguration serilogConfig)
        {
            const string Microsoft = "Microsoft";
            const string System = "System";

            loggerConfig.Enrich.WithProperty(nameof(serilogConfig.Environment), serilogConfig.Environment);
            loggerConfig.Enrich.WithProperty(nameof(serilogConfig.System), serilogConfig.System);
            loggerConfig.Enrich.WithProperty(nameof(serilogConfig.Customer), serilogConfig.Customer);
            loggerConfig.Enrich.FromLogContext();
            var parseSucceeded = Enum.TryParse(serilogConfig.LogEventLevel, true, out LogEventLevel logEventLevel);
            loggerConfig.MinimumLevel.Is(parseSucceeded ? logEventLevel : LogEventLevel.Debug);
            loggerConfig.MinimumLevel.Override(Microsoft, serilogConfig.MicrosoftLogEventLevel);
            loggerConfig.MinimumLevel.Override(System, serilogConfig.SystemLogEventLevel);
            return (loggerConfig, serilogConfig);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => app
                .UseRouting()
                .UseEndpoints(_ => _.MapControllers());
    }
}