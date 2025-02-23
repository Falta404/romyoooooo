using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Application.Helpers
{
    public class LogManager
    {
        private string logFilePath;
        public LogManager()
        {
            logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "Log.txt");
            CheckPath(logFilePath);
        }

        public async Task WriteLogs(string message, Guid messageId)
        {
            using (var stream = new StreamWriter(logFilePath, true))
                await stream.WriteAsync($"MessageId: {messageId}, Date: {DateTime.UtcNow}, Message: {message}\n");
        }

        private void CheckPath(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            if (!File.Exists(filePath))
            {
                var fs = File.Create(filePath);
                fs.Close();
            }
        }
    }
}
