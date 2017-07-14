using Autofac;
using DotnetStatus.Core.Services;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Worker.Helpers;

namespace Worker.RealTime
{
    public static class GetStatus
    {
        private static IContainer ServiceProvider;

        static GetStatus()
        {
            var names = Assembly.GetExecutingAssembly().GetModules();
            BindingRedirect.AddRedirects();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(Directory.GetCurrentDirectory())
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
                }
            }
            catch (Exception e)
            {
                log.Error("Fail");
            }
        }
    }
}
