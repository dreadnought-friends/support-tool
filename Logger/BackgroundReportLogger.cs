using System.ComponentModel;

namespace SupportTool.Logger
{
    class BackgroundReportLogger : LoggerInterface
    {
        private int progress = 0;
        private LoggerInterface inner;
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// Logs to a specific TextBox.
        /// </summary>
        /// <param name="inner">An inner logger</param>
        /// <param name="backgroundWorker">The background worker to log to</param>
        public BackgroundReportLogger(LoggerInterface inner, BackgroundWorker backgroundWorker)
        {
            this.inner = inner;
            this.backgroundWorker = backgroundWorker;
        }

        public void Log(string message)
        {
            inner.Log(message);

            backgroundWorker.ReportProgress(++progress, message);
        }

        public void Clear()
        {
            // cannot clear anything in here, can only propagate the clear
            inner.Clear(); 
        }
    }
}
