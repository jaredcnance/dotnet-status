using Autofac;
using DotnetStatus.Core.Services;
using DotnetStatus.Worker;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Worker.Git
{
    public class GetStatus
    {
        private static IContainer ServiceProvider;

        public static async Task Run(string gitRepositoryUrl, TraceWriter log)
        {
            Configure();
            
            try
            {
                using (var scope = ServiceProvider.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IGitRepositoryStatusService>();
                    var repositoryStatus = await service.GetRepositoryStatusAsync(gitRepositoryUrl);

                    log.Info("Complete");

                    // TODO: enqueue the status
                }
            }
            catch (Exception e)
            {
                log.Error("Fail");
            }
        }

        private static void Configure()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var config = builder.Build();
            ServiceProvider = new ServiceProvider(config).Instance;
        }
    }
}
