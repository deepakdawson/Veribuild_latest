namespace App.Foundation.Common
{
    public class LoggerHelper
    {
        public static void LogError(Exception e)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");

            string datetime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            string message1 = $"ERR: {datetime}\t {e.Message}";
            string message2 = $"\t\t\t {e.StackTrace}";
            string message3 = $"\t\t\t {e.InnerException?.Message}";
            string message = $"{message1}{Environment.NewLine}{message2}{Environment.NewLine}{message3}{Environment.NewLine}";
            File.AppendAllText(path, message);
        }

        public static void LogInfo(string message)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");

            string datetime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
            message = $"INFO: {datetime}\t {message} {Environment.NewLine}{Environment.NewLine}";
            File.AppendAllText(path, message);
        }
    }
}
