namespace DotnetStatus.Worker
{
    interface ITransientGitService
    {
        string GetSource(string repository);
    }
}