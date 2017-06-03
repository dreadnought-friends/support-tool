using System.IO;
using System.IO.Compression;

namespace SupportTool.Command
{
    class Archiver : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
        {
            if (!config.CreateZipArchive)
            {
                logger.Log("Skipping archiving");
                return;
            }

            if (!config.CanCreateArchive)
            {
                logger.Log("Skipping archiving, nothing to archive");
                return;
            }

            string zipFile = Path.Combine(config.ZipFileLocation, config.ZipFileName);

            FileInfo file = new FileInfo(zipFile);

            if (file.Exists)
            {
                logger.Log("Deleting old zip file " + zipFile);
                file.Delete();
            }

            logger.Log("Creating " + zipFile);
            ZipFile.CreateFromDirectory(fileAggregator.TempDir, Path.Combine(config.ZipFileLocation, config.ZipFileName));

            logger.Log("Done! On your desktop you will have a file called 'DN_Support.zip' which you can attach to your support ticket or reply.");
        }
    }
}
