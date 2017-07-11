using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Common;
using DotnetStatus.Worker.Services;
using DotnetStatus.Worker.Services.NuGet;

namespace DotnetStatus.Worker
{
    class ServiceProvider
    {
        private const string _configSectionName = "dotnetStatus";

        public ServiceProvider(IConfiguration config)
        {
            Build(config);
        }

        public IContainer Instance { get; set; }

        private void Build(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddOptions();

            services.Configure<WorkerConfiguration>(options => config.GetSection(_configSectionName).Bind(options));

            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.RegisterType<GitRepositoryStatusService>()
                .AsImplementedInterfaces();

            builder.RegisterType<TransientGitService>()
                .AsImplementedInterfaces();

            builder.RegisterType<DependencyGraphService>()
                .AsImplementedInterfaces();

            builder.RegisterType<PackageStatusStore>()
                .AsImplementedInterfaces();

            builder.RegisterType<PackageStatusRepository>()
                .AsImplementedInterfaces();

            builder.RegisterType<RestoreService>()
                .AsImplementedInterfaces();

            builder.RegisterType<Logger>()
                .As<ILogger>();

            Instance = builder.Build();
        }
    }

}
