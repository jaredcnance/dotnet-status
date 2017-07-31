using Core.Configuration;
using Core.Messaging;
using Core.Services;
using DotnetStatus.Core.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DotnetStatus.EndPoints;
using DotnetStatus.Core.Services.Scheduling;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace DotnetStatus
{
    public class Startup
    {
        public readonly IConfiguration Config;

        public Startup(IConfiguration configuration)
        {
            Config = configuration;
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
            services.AddTransient<IPublishStringMessage, AzureQueueService>();
            services.AddTransient<IConsumeStringMessage, AzureQueueService>();
            services.AddTransient<ICache, Cache>();
            services.AddTransient<IRepositoryResultService, RepositoryResultService>();
            services.AddTransient<IRepositoryResultPersistence, RepositoryResultPersistence>();
            services.AddSingleton<IMongoClient>(new MongoClient(Config["Data:ConnectionString"]));
            services.Configure<DatabaseConfiguration>(options => Config.GetSection("data").Bind(options));
            services.AddTransient<IScheduler, Scheduler>();
            services.AddSockets();
            services.AddSignalR();
            services.AddEndPoint<GitPackageStatusEndpoint>();
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseSockets(routes => routes.MapEndPoint<GitPackageStatusEndpoint>("sockets/git-package-status"));

            app.UseMvc();
        }
    }
}
