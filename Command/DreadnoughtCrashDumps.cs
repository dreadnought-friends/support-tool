using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportTool.Command
{
    class DreadnoughtCrashDumps : CommandInterface, CommandCheckBoxInterface
    {
        public string ConfigPropertyPath
        {
            get
            {
                return "IncludeDreadnoughtCrashDumps";
            }
        }

        public string Text
        {
            get
            {
                return "Crash reports";
            }
        }

        public string ToolTip
        {
            get
            {
                return "Collects crash reports, which can be used for debugging information.";
            }
        }

        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger, Propagation propagation)
        {
            logger.Log("Gathering Dreadnought crash dumps");

            List<DirectoryInfo> directories = Directory.GetDirectories(config.LogFileLocation)
                .Select(x => new DirectoryInfo(x))
                .Where(delegate (DirectoryInfo dir)
                {
                    return dir.EnumerateFiles()
                        .Where(f => f.Name.EndsWith(".dmp"))
                        .Count() > 0;
                })
                .ToList();

            foreach (DirectoryInfo dir in directories)
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    fileAggregator.AddExistingFile(file.FullName, Path.Combine("crash-dumps", dir.Name, file.Name));
                }
            }
        }
    }
}
