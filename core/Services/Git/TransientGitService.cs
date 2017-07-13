using Core.Services.Git;
using DotnetStatus.Core.Configuration;
using LibGit2Sharp;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace DotnetStatus.Core.Services
{
    public class TransientGitService : ITransientGitService, IDisposable
    {
        private readonly string _rootDirectory;
        private readonly int _cleanupTimeout;
        private readonly int _maxCleanupAttempts;
        private readonly IGitCloneService _gitService;
        private string _transientPath;

        public TransientGitService(
            IOptions<WorkerConfiguration> options, 
            IGitCloneService gitService)
        {
            _rootDirectory = options.Value.SourceRootDirectory;
            _cleanupTimeout = options.Value.CleanupTimeout;
            _maxCleanupAttempts = options.Value.MaxCleanupAttempts;
            _gitService = gitService;
        }

        public string GetSource(string repository)
        {
            var path = GetPath();

            _gitService.Clone(repository, path);

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
                    DeleteDirectory();
                    cleanupAttempts++;
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                throw new InvalidOperationException($"Unable to delete the source directory {_transientPath}", uae);
            }            
        }

        private void DeleteDirectory()
        {
            if (string.IsNullOrWhiteSpace(_transientPath) == false)
                Directory.Delete(_transientPath, true);
        }
    }
}
