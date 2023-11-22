using pagSeguro.Api.Entities;
using pagSeguro.Api.Enums;
using pagSeguro.Api.Helpers;

namespace pagSeguro.Api.Services
{
    public class LogService : ILogService
    {
        private DataContext _context;

        public LogService(DataContext context)
        {
            _context = context;
        }

        public void LogError(string message)
        {
            var log = new Log()
            {
                Message = message,
                LevelId = LogType.Error,
                TimeStamp = DateTime.Now,
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }

        public void LogInfo(string template, string message)
        {
            var log = new Log()
            {
                MessageTemplate = template,
                Message = message,
                LevelId = LogType.Info,
                TimeStamp = DateTime.Now,
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
