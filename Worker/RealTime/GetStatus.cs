using Autofac;
using DotnetStatus.Core.Services;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Worker.RealTime
{
    public static class GetStatus
    {
        private static IContainer ServiceProvider;

        static GetStatus()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var config = builder.Build();
            ServiceProvider = new ServiceProvider(config).Instance;
        }

        public static async Task Run(string gitRepositoryUrl, TraceWriter log)
        {
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
    }
}
