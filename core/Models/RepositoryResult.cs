using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DotnetStatus.Core.Models
{
    public class RepositoryResult
    {
        public RepositoryResult() { }

        public RepositoryResult(string id, EvaluationStatus evalStatus)
        {
            Id = id;
            EvaluationStatus = evalStatus;
        }

        public RepositoryResult(string id, EvaluationStatus evalStatus, RestoreStatus restoreStatus)
        {
            Id = id;
            RestoreStatus = restoreStatus;
        }

        public RepositoryResult(string id, EvaluationStatus evalStatus, RestoreStatus restoreStatus, List<ProjectResult> projectResults)
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
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public EvaluationStatus EvaluationStatus { get; set; }
    }
}
