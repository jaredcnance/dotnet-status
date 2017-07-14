using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;
using MongoDB.Driver;
using DotnetStatus.Core.Data;
using DotnetStatus.Core.Configuration;
using DotnetStatus.Core.Services;
using DotnetStatus.Core.Services.NuGet;
using Core.Services.Git;
using Core.Services;
using Microsoft.Extensions.Options;
using Worker.Helpers;

namespace Worker.RealTime
{
    class ServiceProvider
    {
        private readonly IConfiguration _config;

        public ServiceProvider(IConfiguration config)
        {
            _config = config;
            Build(config);
        }

        public IContainer Instance { get; set; }

        private void Build(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddOptions();

            services.Configure<WorkerConfiguration>(options => config.GetSection("dotnetStatus").Bind(options));
            services.Configure<DatabaseConfiguration>(options => config.GetSection("data").Bind(options));

            var builder = new ContainerBuilder();

            // HACK: this shim is required because the function runtime is loading in
            // a version of Autofac where the 'Populate' method is not defined
            builder.PopulateShim(services);

            builder.RegisterType<GitRepositoryStatusService>()
                .AsImplementedInterfaces();

            builder.RegisterType<TransientGitService>()
                .AsImplementedInterfaces();

            builder.RegisterType<LibGit2SharpService>()
                .AsImplementedInterfaces();

            builder.RegisterType<DependencyGraphService>()
                .AsImplementedInterfaces();

            builder.RegisterType<JsonFileReader>()
                .AsImplementedInterfaces();

            builder.RegisterType<PackageStatusStore>()
                .AsImplementedInterfaces();

            builder.RegisterType<PackageStatusRepository>()
                .AsImplementedInterfaces();

            builder.RegisterType<RestoreService>()
                .AsImplementedInterfaces();

            builder.RegisterType<Logger>()
                .As<ILogger>();

            AddRepositoryStatusServices(builder);

            Instance = builder.Build();
        }

        private void AddRepositoryStatusServices(ContainerBuilder builder)
        {
            builder.RegisterType<RepositoryResultService>()
                .As<IRepositoryResultService>();

            builder.RegisterInstance(new MongoClient(_config["Data:ConnectionString"]))
                .As<IMongoClient>();
        }
    }
}
