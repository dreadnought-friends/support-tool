using System;
using System.Windows.Controls;

namespace SupportTool.Logger
{
    class TextBoxLogger : LoggerInterface
    {
        private Config Config;
        private LoggerInterface Inner;
        private TextBox TextBox;

        public TextBoxLogger(Config config, LoggerInterface inner, TextBox textBox)
        {
            Config = config;
            Inner = inner;
            TextBox = textBox;
        }

        public void Clear()
        {
            Inner.Clear();
        }

        public void Log(string message)
        {
            if (Config.ShowLogTimes)
            {
                message = LoggingFormatter.FormatTime(message);
            }

            Inner.Log(message);

            TextBox.AppendText(message + Environment.NewLine);
            TextBox.ScrollToEnd();
        }

        public void Debug(string message)
        {
#if DEBUG
            Log(LoggingFormatter.FormatDebug(message));
#endif
        }
    }
}
