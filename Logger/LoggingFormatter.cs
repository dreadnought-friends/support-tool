using System;

namespace SupportTool.Logger
{
    class LoggingFormatter
    {
        public static string FormatTime(string message)
        {
            return String.Format("[{0:HH:mm:ss}] {1}", DateTime.Now, message);
        }
    }
}
