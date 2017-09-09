using System;
using System.IO;

namespace SupportTool.Command
{
    class TempDirectoryPreparation : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            DirectoryInfo tempDir = new DirectoryInfo(fileAggregator.TempDir);

            if (!tempDir.Exists)
            {
                logger.Debug(string.Format("Created {0}", fileAggregator.TempDir));
                Directory.CreateDirectory(fileAggregator.TempDir);
                return;
            }

            logger.Debug(string.Format("Cleaning up old files in {0}", fileAggregator.TempDir));

            foreach (FileInfo file in tempDir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in tempDir.GetDirectories())
            {
                dir.Delete(true);
            }

            fileAggregator.AggregatedFiles.Clear();
        }
    }
}
