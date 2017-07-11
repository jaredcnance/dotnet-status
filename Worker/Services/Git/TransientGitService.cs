using LibGit2Sharp;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace DotnetStatus.Worker
{
    class TransientGitService : ITransientGitService, IDisposable
    {
        private readonly string _rootDirectory;
        private readonly int _cleanupTimeout;
        private readonly int _maxCleanupAttempts;
        private string _transientPath;

        public TransientGitService(IOptions<WorkerConfiguration> options)
        {
            _rootDirectory = options.Value.SourceRootDirectory;
            _cleanupTimeout = options.Value.CleanupTimeout;
            _maxCleanupAttempts = options.Value.MaxCleanupAttempts;
        }

        public string GetSource(string repository)
        {
            var path = GetPath();

            Repository.Clone(repository, path);

            return path;
        }

        private string GetPath()
        {
            if (Directory.Exists(_rootDirectory) == false)
                Directory.CreateDirectory(_rootDirectory);

            string path;
            do
                path = $"{_rootDirectory}/{Guid.NewGuid().ToString("N")}";
            while (Directory.Exists(path));

            Directory.CreateDirectory(path);

            _transientPath = path;

            return path;
        }

        public void Dispose()
        {
            try
            {
                var cleanupAttempts = 0;
                while(cleanupAttempts < _maxCleanupAttempts)
                {
                    cleanupAttempts++;
                }
            }
            catch (UnauthorizedAccessException)
            {

                throw;
            }            
        }

        private void DeleteDirectory()
        {
            if (string.IsNullOrWhiteSpace(_transientPath) == false)
                Directory.Delete(_transientPath, true);
        }
    }
}
