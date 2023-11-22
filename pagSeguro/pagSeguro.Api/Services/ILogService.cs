namespace pagSeguro.Api.Services
{
    public interface ILogService
    {
        void LogError(string message);
        void LogInfo(string template, string message);
    }
}
