using System.IO;
using System.Reflection;

namespace SupportTool.Command
{
    class CustomerSupportReadme : CommandInterface
    {
        public void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger)
        {
            FileInfo readmeFile = fileAggregator.AddVirtualFile("Readme.txt");

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SupportTool.Assets.CustomerSupportReadme.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                File.WriteAllText(readmeFile.FullName, reader.ReadToEnd().Replace("{version}", config.Version));
            }
        }
    }
}
