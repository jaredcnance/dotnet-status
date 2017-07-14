using System.Threading.Tasks;

namespace Core.Messaging
{
    public interface IConsumeStringMessage
    {
        Task<string> DeQueueStringMessageAsync(string target);
    }
}
