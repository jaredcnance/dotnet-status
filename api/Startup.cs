using Core.Configuration;
using Core.Messaging;
using Core.Services;
using DotnetStatus.Core.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DotnetStatus
{
    public class Startup
    {
        public readonly IConfiguration Config;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Config = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            services.AddSingleton<ILoggerFactory>(loggerFactory);

            services.AddMvc();
            services.AddCors();
            services.AddMemoryCache();

            services.AddOptions();

            services.Configure<AzureStorageConfiguration>(options => Config.GetSection("AzureStorage").Bind(options));
            services.AddScoped<IPublishStringMessage, AzureQueueService>();
            services.AddScoped<ICache, Cache>();
            services.AddScoped<IRepositoryStatusService, RepositoryStatusService>();
            services.AddScoped<IRepositoryResultPersistence, RepositoryResultPersistence>();
            services.AddSingleton<IMongoClient>(new MongoClient(Config["Data:ConnectionString"]));
            services.Configure<DatabaseConfiguration>(options => Config.GetSection("data").Bind(options));
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
                app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));

            app.UseMvc();
        }
    }
}
