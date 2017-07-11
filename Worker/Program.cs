using Autofac;
using DotnetStatus.Core.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace DotnetStatus.Worker
{
    partial class Program
    {
        private static IContainer ServiceProvider;

        //private static string testUrl = "https://github.com/Research-Institute/json-api-dotnet-core.git";
        //private static string testUrl = "https://github.com/jaredcnance/dotnet-status.git";
        private static string testUrl = "https://github.com/jaredcnance/dotnet-status-test.git";

        static void Main(string[] args)
        {
            Configure();
            
            try
            {
                using (var scope = ServiceProvider.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IGitRepositoryStatusService>();
                    var repositoryStatus = service.GetRepositoryStatusAsync(testUrl).GetAwaiter().GetResult();
                    Console.WriteLine("Complete");
                    // TODO: enqueue the status
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail");
            }

            Console.ReadLine();
        }

        private static void Configure()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var config = builder.Build();
            ServiceProvider = new ServiceProvider(config).Instance;
        }
    }
}
