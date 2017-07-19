namespace DotnetStatus.Core.Models
{
    public class RestoreStatus
    {
        public RestoreStatus() { }

        public RestoreStatus(string output, string errors, int exitCode, bool success)
        {
            Output = output;
            Errors = errors;
            ExitCode = exitCode;
            Success = success;
        }

        public string Output { get; set; }
        public string Errors { get; set; }
        public int ExitCode { get; set; }
        public bool Success { get; set; }
    }
}
