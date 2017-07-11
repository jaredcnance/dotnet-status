using System.Collections.Generic;

namespace DotnetStatus.Core.Models
{
    public class RepositoryStatusResult
    {
        public RepositoryStatusResult(RestoreStatus restoreStatus)
        {
            RestoreStatus = restoreStatus;
        }

        public RepositoryStatusResult(RestoreStatus restoreStatus, List<ProjectResult> projectResults)
        {
            RestoreStatus = restoreStatus;
            ProjectResults = projectResults;
        }

        public bool Success => RestoreStatus.Success;
        public RestoreStatus RestoreStatus { get; set; }
        public List<ProjectResult> ProjectResults { get; set; } = new List<ProjectResult>();
    }
}
