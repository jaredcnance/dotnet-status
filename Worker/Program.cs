using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using DotnetStatus.Worker.Services;
namespace DotnetStatus.Worker
{
    partial class Program
    {
        private static IContainer ServiceProvider;

        private static string testUrl = "https://github.com/Research-Institute/json-api-dotnet-core.git";

        static void Main(string[] args)
        {
            Configure();
            
            try
            {
                using (var scope = ServiceProvider.BeginLifetimeScope())
                {
                    var service = scope.Resolve<IGitRepositoryStatusService>();
                    var repositoryStatus = service.GetRepositoryStatus(testUrl);
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
