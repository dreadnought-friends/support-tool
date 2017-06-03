using System.IO;

namespace SupportTool.Command
{
    class AggregatedFileCollector : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
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
                    throw new FileNotFoundException("Expected file not found", file.To.FullName);
                }

                logger.Log(file.To.FullName);
            }
        }
    }
}
