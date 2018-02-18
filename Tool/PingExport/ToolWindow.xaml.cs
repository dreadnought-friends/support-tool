using SupportTool.Ping;
using System.IO;
using System.Text;
using System.Windows;

namespace SupportTool.Tool.PingExport
{
    public partial class ToolWindow : Window
    {
        private LoggerInterface Logger;
        private Config Config;
        private PingStorage PingStorage;

        public ToolWindow(PingStorage pingStorage, Config config, LoggerInterface logger)
        {
            PingStorage = pingStorage;
            Config = config;
            Logger = logger;

            InitializeComponent();
        }

        private void ExportPing_Click(object sender, RoutedEventArgs e)
        {
            var pings = PingStorage.Pings;
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Timestamp, AveragePing");

            foreach (var result in pings)
            {
                stringBuilder.AppendLine(string.Format("{0}, {1}", result.Timestamp, result.AveragePing));
            }

            var exportFile = Path.Combine(Config.ZipFileLocation, "DN_Support_ping_export.csv");
            File.WriteAllText(exportFile, stringBuilder.ToString());

            Logger.Log(string.Format("Ping data exported to {0}", exportFile));

            Hide();
        }
    }
}
