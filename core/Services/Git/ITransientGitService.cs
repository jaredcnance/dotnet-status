namespace DotnetStatus.Core.Services
{
    public interface ITransientGitService
    {
        string GetSource(string repository);
    }
}