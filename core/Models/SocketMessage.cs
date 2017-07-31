namespace DotnetStatus.Core.Models
{
    public class SocketMessage
    {
        public SocketMessage(string connectionId, object message, bool endOfChain = false, bool completedSuccessfully = false)
        {
            ConnectionId = connectionId;
            Message = message;
            EndOfChain = endOfChain;
            CompletedSuccessfully = completedSuccessfully;
        }

        public string ConnectionId { get; set; }
        public object Message { get; set; }
        public bool EndOfChain { get; set; }
        public bool CompletedSuccessfully { get; set; }
    }
}
