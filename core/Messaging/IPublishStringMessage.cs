using System.Threading.Tasks;

namespace Core.Messaging
{
    public interface IPublishStringMessage
    {
        Task PublishMessageAsync(string target, string message);
    }
}
