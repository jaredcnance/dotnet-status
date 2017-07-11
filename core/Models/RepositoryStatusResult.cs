using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DotnetStatus.Core.Models
{
    public class RepositoryStatusResult
    {
        public RepositoryStatusResult(string id, RestoreStatus restoreStatus)
        {
            Id = id;
            RestoreStatus = restoreStatus;
        }

        public RepositoryStatusResult(string id, RestoreStatus restoreStatus, List<ProjectResult> projectResults)
        {
            Id = id;
            RestoreStatus = restoreStatus;
            ProjectResults = projectResults;
        }

        [BsonId]
        public string Id { get; set; }
        public bool Success => RestoreStatus.Success;
        public RestoreStatus RestoreStatus { get; set; }
        public List<ProjectResult> ProjectResults { get; set; } = new List<ProjectResult>();
    }
}
