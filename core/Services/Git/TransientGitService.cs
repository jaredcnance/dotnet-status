using Core.Services.Git;
using DotnetStatus.Core.Configuration;
using LibGit2Sharp;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;

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
            DeleteDirectory(_transientPath);
        }

        public void DeleteDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false) return;

            NormalizeAttributes(directoryPath);
            DeleteDirectory(directoryPath, initialTimeout: 16, timeoutFactor: 2);
        }

        private void NormalizeAttributes(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            foreach (string subdirectoryPath in subdirectoryPaths)
            {
                NormalizeAttributes(subdirectoryPath);
            }
            File.SetAttributes(directoryPath, FileAttributes.Normal);
        }

        private void DeleteDirectory(string directoryPath, int initialTimeout, int timeoutFactor)
        {
            for (int attempt = 1; attempt <= _maxCleanupAttempts; attempt++)
            {
                try
                {
                    Directory.Delete(directoryPath, true);
                    return;
                }
                catch (UnauthorizedAccessException uae)
                {
                    if (attempt < _maxCleanupAttempts)
                    {
                        Thread.Sleep(initialTimeout * (int)Math.Pow(timeoutFactor, attempt - 1));
                        continue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unable to delete the source directory {_transientPath}", uae);
                    }
                }
            }
        }
    }
}
