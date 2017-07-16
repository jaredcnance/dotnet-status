using Core.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace Core.Messaging
{
    public class AzureQueueService : IPublishStringMessage, IConsumeStringMessage
    {
        private readonly AzureStorageConfiguration _config;
        private readonly CloudQueueClient _client;

        public AzureQueueService(IOptions<AzureStorageConfiguration> options)
        {
            _config = options.Value;
            _client = GetStorageClient();
        }

        private CloudQueueClient GetStorageClient()
        {
            var credentials = new StorageCredentials(_config.Account, _config.AccountKey);
            var account = new CloudStorageAccount(credentials, useHttps: true);
            var client = account.CreateCloudQueueClient();

            return client;
        }

        /// <summary>
        /// Publishes message to the specified queue
        /// </summary>
        /// <param name="target">Queue name</param>
        /// <param name="message">Message</param>
        public async Task PublishMessageAsync(string target, string message)
        {
            var queue = await GetQueueAsync(target);

            var queueMessage = new CloudQueueMessage(message);

            await queue.AddMessageAsync(queueMessage);
        }

        /// <summary>
        /// Removes the next message from the queue
        /// </summary>
        /// <param name="target">Queue name</param>
        /// <returns>The message content</returns>
        public async Task<string> DeQueueStringMessageAsync(string target)
        {
            var queue = await GetQueueAsync(target);

            var message = await queue.GetMessageAsync();

            if (message == null)
                return null;

            var stringMessage = message.AsString;

            await queue.DeleteMessageAsync(message);

            return stringMessage;
        }

        private async Task<CloudQueue> GetQueueAsync(string name)
        {
            var queue = _client.GetQueueReference(name);

            await queue.CreateIfNotExistsAsync();

            return queue;
        }
    }
}
