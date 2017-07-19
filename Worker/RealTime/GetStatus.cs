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
                .AddJsonFile("appsettings.json", optional: true)
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
                    var service = scope.Resolve<IRepositoryStatusEvaluator>();
                    var repositoryStatus = await service.EvaluateAsync(gitRepositoryUrl);

                    log.Info("Complete");
                }
            }
            catch (Exception e)
            {
                var message = GetErrorMessage(e);
                log.Error(message);
            }
        }

        private static string GetErrorMessage(Exception e)
        {
            var error = $"{e.Message} \n  {e.Message} \n {e.StackTrace}";
            while(e.InnerException != null)
            {
                e = e.InnerException;
                error += $"\n\n {e.Message} \n  {e.Message} \n {e.StackTrace}";
            }
            return error;
        }
    }
}
