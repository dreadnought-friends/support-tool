using System;
using System.ComponentModel;

namespace SupportTool.Logger
{
    class BackgroundReportLogger : LoggerInterface
    {
        private int progress = 0;
        private Config Config;
        private LoggerInterface Inner;
        private BackgroundWorker BackgroundWorker;

        /// <summary>
        /// Logs to a specific TextBox.
        /// </summary>
        /// <param name="config">The configuration of the app</param>
        /// <param name="inner">An inner logger</param>
        /// <param name="backgroundWorker">The background worker to log to</param>
        public BackgroundReportLogger(Config config, LoggerInterface inner, BackgroundWorker backgroundWorker)
        {
            Config = config;
            Inner = inner;
            BackgroundWorker = backgroundWorker;
        }

        public void Log(string message)
        {
            if (Config.ShowLogTimes)
            {
                message = LoggingFormatter.FormatTime(message);
            }

            Inner.Log(message);

            BackgroundWorker.ReportProgress(++progress, message);
        }

        public void Clear()
        {
            // cannot clear anything in here, can only propagate the clear
            Inner.Clear(); 
        }
    }
}
