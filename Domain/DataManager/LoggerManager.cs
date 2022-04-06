using Domain.Interfaces;

namespace Domain.DataManager
{
    public static class LoggerManager
    {
        private static ILogger _logger;
        private static int _issueCount = 0;

        public static void Init(ILogger logger)
        {
            _logger = logger;
        }

        public static void Log(string method, string message)
        {
            _issueCount++;
            _logger.Log($"{method}: {message}");
        }

    }
}
