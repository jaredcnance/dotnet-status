namespace DotnetStatus.Core.Models
{
    public class SocketMessage
    {
        public SocketMessage(string connectionId, string message)
        {
            ConnectionId = connectionId;
            Message = message;
        }

        public string ConnectionId { get; set; }
        public string Message { get; set; }
    }
}
