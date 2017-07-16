using System;

namespace Core.Messaging
{
    public class GitRepositoryRequest
    {
        public string GitRemoteUrl { get; set; }
        public Guid? ConnectionId { get; set; }
    }
}
