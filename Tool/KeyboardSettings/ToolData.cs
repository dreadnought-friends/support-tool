using System;
using System.Windows;
using System.Windows.Media;

namespace SupportTool.Tool.KeyboardSettings
{
    class ToolData : ToolInterface
    {
        private ToolWindow Window;

        public ToolData(Config config, LoggerInterface logger)
        {
            Window = new ToolWindow(new ModulePreset(config), logger);
        }

        public string MenuHeader
        {
            get
            {
                return "Keyboard Settings";
            }
        }

        public ImageSource MenuIcon
        {
            get
            {
                return null;
            }
        }

        public Action OnShow
        {
            get
            {
                return delegate {
                    Window.InitializeKeybindings();
                };
            }
        }

        public bool RequiresElevatedPermissions
        {
            get
            {
                return false;
            }
        }

        public Window ToolWindow
        {
            get
            {
                return Window;
            }
        }
    }
}
