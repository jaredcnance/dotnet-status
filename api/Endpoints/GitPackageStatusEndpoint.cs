using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Messaging;
using Core.Services;
using DotnetStatus.Core.Models;
using DotnetStatus.Core.Services.Scheduling;
using Microsoft.AspNetCore.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                await SendToAsync(connection, message.Message);
            }
            _scheduler.Change(_timerInterval);
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

            await Broadcast($"{connection.ConnectionId} connected");

            try
            {
                while (await connection.Transport.In.WaitToReadAsync())
                {
                    if (connection.Transport.In.TryRead(out var buffer))
                    {
                        var text = Encoding.UTF8.GetString(buffer);
                        var result = await _repoStatus.FindAsync(text);

                        result = await GetResultAsync(connection.ConnectionId, text, result);

                        text = $"{connection.ConnectionId}: {text} => {JsonConvert.SerializeObject(result)}";

                        await Broadcast(text);
                    }
                }
            }
            finally
            {
                Connections.Remove(connection);

                await Broadcast($"{connection.ConnectionId} disconnected");
            }
        }

        private async Task SendToAsync(ConnectionContext connection, string text)
        {
            var payload = Encoding.UTF8.GetBytes(text);
            var tasks = new List<Task>(Connections.Count);
            await connection.Transport.Out.WriteAsync(payload);
        }

        private Task Broadcast(string text)
        {
            var payload = Encoding.UTF8.GetBytes(text);
            var tasks = new List<Task>(Connections.Count);

            foreach (var c in Connections)
            {
                tasks.Add(c.Transport.Out.WriteAsync(payload));
            }

            return Task.WhenAll(tasks);
        }

        private async Task<RepositoryResult> GetResultAsync(string connectionId, string repositoryUri, RepositoryResult result)
        {
            var currentStatus = result?.EvaluationStatus;

            if (currentStatus == EvaluationStatus.Complete || currentStatus == EvaluationStatus.Failed)
            {
                // if it is expired, queue a job to update it
                if (result.UpdatedAt.AddMinutes(ResultTTLMinutes) <= DateTimeOffset.UtcNow)
                    await QueueProcessing(connectionId, repositoryUri);

                return result;
            }

            if (currentStatus != EvaluationStatus.Processing)
                await QueueProcessing(connectionId, repositoryUri);

            return new RepositoryResult(repositoryUri, EvaluationStatus.Processing);
        }

        private async Task QueueProcessing(string connectionId, string repositoryUri)
        {
            var message = JsonConvert.SerializeObject(new SocketMessage(connectionId, repositoryUri));
            await _publish.PublishMessageAsync(WORKER_MESSAGE_QUEUE, message);

            await _repoStatus.SetStatusAsync(repositoryUri, EvaluationStatus.Processing);
        }
    }
}
