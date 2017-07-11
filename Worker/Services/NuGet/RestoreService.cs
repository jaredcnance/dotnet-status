using DotnetStatus.Core;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DotnetStatus.Worker.Services.NuGet
{
    class RestoreService : IRestoreService
    {
        private readonly string _nugetPath;
        private readonly string _args;
        private readonly int _timeout;

        public RestoreService(IOptions<WorkerConfiguration> options)
        {
            _nugetPath = options.Value.NuGetPath;
            _args = string.Join(" ", options.Value.RestoreArguments);
            _timeout = options.Value.RestoreTimeoutMilliseconds;
        }

        public RestoreStatus Restore(string projectPath, string dependencyGraphOutputPath)
        {
            var psi = new ProcessStartInfo(Path.GetFullPath(_nugetPath), _args)
            {
                WorkingDirectory = Path.GetFullPath(projectPath),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            SetEnvironmentVariables(psi, dependencyGraphOutputPath);

            var exitCode = 1;

            var output = new StringBuilder();
            var errors = new StringBuilder();

            var p = new Process();

            try
            {
                p.StartInfo = psi;
                p.Start();

                var outputTask = ConsumeStreamReaderAsync(p.StandardOutput, output);
                var errorTask = ConsumeStreamReaderAsync(p.StandardError, errors);

                var processExited = p.WaitForExit(_timeout);

                if (processExited == false)
                {
                    p.Kill();

                    var processName = Path.GetFileName(_nugetPath);

                    return new RestoreStatus(output.ToString(), errors.ToString(), exitCode: -1, success: false);
                }

                Task.WaitAll(outputTask, errorTask);

                exitCode = p.ExitCode;

                return new RestoreStatus(output.ToString(), errors.ToString(), exitCode, success: exitCode == 0);
            }
            finally
            {
                p.Dispose();
            }
        }

        private void SetEnvironmentVariables(ProcessStartInfo psi, string dependencyGraphOutputPath)
        {
#if !IS_CORECLR
            psi.EnvironmentVariables["NUGET_PERSIST_DG"] = "True";
            psi.EnvironmentVariables["NUGET_PERSIST_DG_PATH"] = dependencyGraphOutputPath;
#else
            psi.Environment["NUGET_PERSIST_DG"] = "True";
            psi.Environment["NUGET_PERSIST_DG_PATH"] = dependencyGraphOutputPath;
#endif
        }

        private static async Task ConsumeStreamReaderAsync(StreamReader reader, StringBuilder lines)
        {
            await Task.Yield();

            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lines.AppendLine(line);
            }
        }
    }
}
