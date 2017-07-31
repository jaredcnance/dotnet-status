using System;
using System.Collections.Generic;
using DotnetStatus.Core.Configuration;
using DotnetStatus.Core.Data;
using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.NuGet;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Core.Messaging;
using Newtonsoft.Json.Serialization;

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
            var respositoryUrl = socketMessage.Message.ToString();
            var restoreStatus = new RestoreStatus();
            var projectResults = new List<ProjectResult>();

            try
            {
                await SendToClientAsync(socketMessage, $"Request picked up by worker.\nCloning {socketMessage.Message}");

                var repoPath = _gitService.GetSource(respositoryUrl);

                await SendToClientAsync(socketMessage, $"Cloned to {repoPath} \n Restoring...");

                restoreStatus = _restoreService.Restore(repoPath);

                if (restoreStatus.Success == false)
                {
                    await SendToClientAsync(socketMessage, $"{restoreStatus.Output}\nRestore failed with status code {restoreStatus.ExitCode}.");
                    return await GetFailedResultAsync(respositoryUrl, restoreStatus);
                }

                await SendToClientAsync(socketMessage, $"{restoreStatus.Output}\nRestore completed successfully with status code {restoreStatus.ExitCode}.\nNow parsing the dependency graph...");

                var dependencyGraphPath = $"{repoPath}/{_dgFileName}";
                projectResults = _dependencyGraphService.GetProjectResults(dependencyGraphPath);

                var result = new RepositoryResult(respositoryUrl, EvaluationStatus.Complete, restoreStatus, projectResults);

                await _repository.SaveAsync(result);

                await SendToClientAsync(socketMessage, $"Completed with evaluation status '{result.EvaluationStatus}'");

                await SendToClientAsync(socketMessage, result, endOfChain: true, completedSuccessfully: true);

                return result;
            }
            catch (Exception e)
            {
                await SendToClientAsync(socketMessage, $"An unexpected failure ocurred. {e.Message} \n{e.StackTrace}.", endOfChain: true);

                return new RepositoryResult(respositoryUrl, EvaluationStatus.Failed, restoreStatus, projectResults);
            }
        }

        private async Task SendToClientAsync(SocketMessage originalMessage, object message, bool endOfChain = false, bool completedSuccessfully = false)
        {
            if (string.IsNullOrWhiteSpace(originalMessage.ConnectionId)) return;

            var socketMessage = new SocketMessage(originalMessage.ConnectionId, message, endOfChain, completedSuccessfully);

            var newMessage = JsonConvert.SerializeObject(socketMessage, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

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
