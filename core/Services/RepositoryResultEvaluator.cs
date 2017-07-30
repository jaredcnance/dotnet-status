using DotnetStatus.Core.Configuration;
using DotnetStatus.Core.Data;
using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.NuGet;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Core.Messaging;

namespace DotnetStatus.Core.Services
{
    public class RepositoryResultEvaluator : IRepositoryResultEvaluator
    {
        private readonly ITransientGitService _gitService;
        private readonly IRestoreService _restoreService;
        private readonly IDependencyGraphService _dependencyGraphService;
        private readonly string _dgFileName;
        private readonly IRepositoryResultPersistence _repository;
        private readonly IPublishStringMessage _publish;
        private const string CLIENT_MESSAGE_QUEUE = "socket-messages";

        public RepositoryResultEvaluator(
            ITransientGitService transientGitService,
            IRestoreService restoreService,
            IDependencyGraphService dependencyGraphService,
            IOptions<WorkerConfiguration> options,
            IRepositoryResultPersistence repository,
            IPublishStringMessage publish)
        {
            _gitService = transientGitService;
            _restoreService = restoreService;
            _dependencyGraphService = dependencyGraphService;
            _dgFileName = options.Value.DependencyGraphFileName;
            _repository = repository;
            _publish = publish;
        }

        public async Task<RepositoryResult> EvaluateAsync(string socketMessageJson)
        {
            var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(socketMessageJson);

            await SendToClientAsync(socketMessage, $"Cloning {socketMessage.Message}");

            var repoPath = _gitService.GetSource(socketMessage.Message);
            
            await SendToClientAsync(socketMessage, $"Cloned to {repoPath} \n Restoring...");

            var status = _restoreService.Restore(repoPath);

            await SendToClientAsync(socketMessage, $"Restore exited with status code {status.ExitCode} \n {status.Output}");

            if (status.Success == false)
                return await GetFailedResultAsync(socketMessage.Message, status);

            var dependencyGraphPath = $"{repoPath}/{_dgFileName}";
            var projectResults = _dependencyGraphService.GetProjectResults(dependencyGraphPath);

            var result = new RepositoryResult(socketMessage.Message, EvaluationStatus.Complete, status, projectResults);

            await SendToClientAsync(socketMessage, $"Completed with evaluation status {result.EvaluationStatus}");

            await _repository.SaveAsync(result);

            return result;
        }

        private async Task SendToClientAsync(SocketMessage originalMessage, string text)
        {
            if(string.IsNullOrWhiteSpace(originalMessage.ConnectionId)) return;

            var newMessage = JsonConvert.SerializeObject(new SocketMessage(originalMessage.ConnectionId, text));

            await _publish.PublishMessageAsync(CLIENT_MESSAGE_QUEUE, newMessage);
        }

        private async Task<RepositoryResult> GetFailedResultAsync(string repositoryUrl, RestoreStatus status)
        {
            var failedResult = new RepositoryResult(repositoryUrl, EvaluationStatus.Failed, status);

            await _repository.SaveAsync(failedResult);

            return failedResult;
        }
    }
}
