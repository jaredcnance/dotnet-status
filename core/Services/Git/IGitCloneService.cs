namespace Core.Services.Git
{
    public interface IGitCloneService
    {
        void Clone(string gitUrl, string targetDir);
    }
}