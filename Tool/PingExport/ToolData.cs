using SupportTool.Ping;
using System;
using System.Windows;
using System.Windows.Media;

namespace SupportTool.Tool.PingExport
{
    class ToolData : ToolInterface
    {
        private ToolWindow Window;

        public ToolData(PingStorage pingStorage, Config config, LoggerInterface logger)
        {
            Window = new ToolWindow(pingStorage, config, logger);
        }

        public string MenuHeader { get { return "Export Ping Data"; } }

        public ImageSource MenuIcon { get { return null; } }

        public Action OnShow { get { return null; } }

        public bool RequiresElevatedPermissions { get { return false; } }

        public Window ToolWindow { get { return Window; } }
    }
}
