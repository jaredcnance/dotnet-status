using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Core.Messaging;
using Core.Services;
using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.Scheduling;
using Microsoft.AspNetCore.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotnetStatus.EndPoints
{
    public class GitPackageStatusEndpoint : EndPoint
    {
        private readonly IPublishStringMessage _publish;
        private readonly IRepositoryResultService _repoStatus;
        private const int ResultTTLMinutes = 1440; // 24 hrs
        private readonly ILogger<GitPackageStatusEndpoint> _logger;
        private readonly IConsumeStringMessage _consume;
        private readonly IScheduler _scheduler;
        private readonly TimeSpan _timerInterval = TimeSpan.FromSeconds(5);

        private const string CLIENT_MESSAGE_QUEUE = "socket-messages";
        private const string WORKER_MESSAGE_QUEUE = "git-remote-queue";

        public GitPackageStatusEndpoint(
            IPublishStringMessage publish,
            IConsumeStringMessage consume,
            IRepositoryResultService repoStatus,
            IScheduler scheduler,
            ILogger<GitPackageStatusEndpoint> logger)
        {
            _publish = publish;
            _consume = consume;
            _repoStatus = repoStatus;
            _logger = logger;

            _scheduler = scheduler;
            scheduler.Schedule(OnTimerAsync, _timerInterval);
        }

        private async void OnTimerAsync(object state)
        {
            _scheduler.Pause();
            var messageText = await _consume.DeQueueStringMessageAsync(CLIENT_MESSAGE_QUEUE);
            if (string.IsNullOrWhiteSpace(messageText) == false)
            {
                var message = JsonConvert.DeserializeObject<SocketMessage>(messageText);

                var connection = GetConnection(message.ConnectionId);
                if (connection != null)
                    await SendToAsync(connection, message);

                _scheduler.Change(new TimeSpan(0));
            }
            else
            {
                _scheduler.Change(_timerInterval);
            }
        }

        private ConnectionContext GetConnection(string connectionId)
        {
            foreach (var conn in Connections)
                if (conn.ConnectionId == connectionId)
                    return conn;

            return null;
        }

        public ConnectionList Connections { get; } = new ConnectionList();

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            Connections.Add(connection);

            await SendToAsync(connection, $"{connection.ConnectionId} connected");

            try
            {
                while (await connection.Transport.In.WaitToReadAsync())
                {
                    if (connection.Transport.In.TryRead(out var buffer))
                    {
                        var repositoryUrl = Encoding.UTF8.GetString(buffer);
                        if (string.IsNullOrWhiteSpace(repositoryUrl))
                        {
                            await SendToAsync(connection, "Invalid repository URL", true);
                            continue;
                        }

                        await SendToAsync(connection, "Checking if this repository has already been processed");
                        var result = await _repoStatus.FindAsync(repositoryUrl);

                        await SendToAsync(connection, (result == null ? "Repository not found in database." : $"Repository located with status {result.EvaluationStatus}"));

                        await GetResultAsync(connection, repositoryUrl, result);
                    }
                }
            }
            finally
            {
                Connections.Remove(connection);
            }
        }

        private async Task<RepositoryResult> GetResultAsync(ConnectionContext connection, string repositoryUri, RepositoryResult result)
        {
            var currentStatus = result?.EvaluationStatus;

            if (currentStatus == EvaluationStatus.Complete || currentStatus == EvaluationStatus.Failed)
            {
                // if it is expired, queue a job to update it
                if (result.UpdatedAt.AddMinutes(ResultTTLMinutes) <= DateTimeOffset.UtcNow)
                {
                    await SendToAsync(connection, "Re-queueing this repository for processing since it has expired.");
                    await QueueProcessing(connection.ConnectionId, repositoryUri);
                }

                await SendToAsync(connection, "Returning historical repository status.");
                await SendToAsync(connection, result, true, true);
                return result;
            }

            if (currentStatus != EvaluationStatus.Processing)
            {
                await SendToAsync(connection, "Queuing this repository for processing. Please be patient while waiting for a worker process to connect.");
                await QueueProcessing(connection.ConnectionId, repositoryUri);
            }

            return new RepositoryResult(repositoryUri, EvaluationStatus.Processing);
        }

        private async Task SendToAsync(ConnectionContext connection, object message, bool endOfChain = false, bool completedSuccesfully = false)
        {
            var messageJson = JsonConvert.SerializeObject(new SocketMessage(connection.ConnectionId, message, endOfChain, completedSuccesfully), new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var payload = Encoding.UTF8.GetBytes(messageJson);
            await connection.Transport.Out.WriteAsync(payload);
        }

        private async Task SendToAsync(ConnectionContext connection, SocketMessage message)
        {
            var messageJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var payload = Encoding.UTF8.GetBytes(messageJson);
            await connection.Transport.Out.WriteAsync(payload);
        }

        private async Task QueueProcessing(string connectionId, string repositoryUri)
        {
            var message = JsonConvert.SerializeObject(new SocketMessage(connectionId, repositoryUri));

            await _publish.PublishMessageAsync(WORKER_MESSAGE_QUEUE, message);

            await _repoStatus.SetStatusAsync(repositoryUri, EvaluationStatus.Processing);
        }
    }
}
