using System.IO;
using System.IO.Compression;

namespace SupportTool.Command
{
    class Archiver : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "CreateZipArchive";
            }
        }

        public string Text
        {
            get
            {
                return "Create Zip Archive";
            }
        }

        public string ToolTip
        {
            get
            {
                return "Creates a ZIP file in which all collected files are present.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            if (!config.CanCreateArchive)
            {
                logger.Log("Skipping archiving, nothing selected to archive");
                return;
            }

            string zipFile = Path.Combine(config.ZipFileLocation, config.ZipFileName);

            FileInfo file = new FileInfo(zipFile);

            if (file.Exists)
            {
                logger.Log(string.Format("Deleting old zip file {0}", zipFile));
                file.Delete();
            }

            // the readme file is always created
            string[] files = Directory.GetFileSystemEntries(fileAggregator.TempDir);
            string readme = Path.Combine(fileAggregator.TempDir, "Readme.txt");

            if (1 == files.Length && files[0].Equals(readme))
            {
                logger.Log("Skipping archiving, no files collected to archive");
                return;
            }

            logger.Log(string.Format("Creating {0}", zipFile));
            ZipFile.CreateFromDirectory(fileAggregator.TempDir, Path.Combine(config.ZipFileLocation, config.ZipFileName));

            logger.Log("Done! On your desktop you will have a file called 'DN_Support.zip' which you can attach to your support ticket or reply.");
        }
    }
}
