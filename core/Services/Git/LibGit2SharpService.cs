using LibGit2Sharp;
using System;

namespace Core.Services.Git
{
    public class LibGit2SharpService : IGitCloneService
    {
        public void Clone(string gitUrl, string targetDir)
        {
            try
            {
                Repository.Clone(gitUrl, targetDir);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error ocurred while cloning {gitUrl}", ex);
            }
        }
    }
}
