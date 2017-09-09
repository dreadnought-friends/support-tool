using System.IO;

namespace SupportTool.Command
{
    class AggregatedFileCollector : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            logger.Log("Preparing files to be archived");

            foreach (AggregatedFile file in fileAggregator.AggregatedFiles)
            {
                if (!file.From.Equals(file.To))
                {
                    file.To.Directory.Create();
                    File.Copy(file.From.FullName, file.To.FullName, true);
                }

                if (!File.Exists(file.To.FullName))
                {
                    logger.Log(string.Format("Expected file not found: {0}", file.To.FullName));
                    propagation.ShouldStop = true;
                    return;
                }

                logger.Debug(file.To.FullName);
            }
        }
    }
}
