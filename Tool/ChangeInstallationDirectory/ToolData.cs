using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SupportTool.Tool.ChangeInstallationDirectory
{
    class ToolData : ToolInterface
    {
        private ToolWindow Window;

        public ToolData(LoggerInterface logger)
        {
            Window = new ToolWindow(logger);
        }

        public string MenuHeader
        {
            get
            {
                return "Change Installation Directory";
            }
        }

        public ImageSource MenuIcon
        {
            get
            {
                return new BitmapImage(new Uri("/Assets/DreadGame.ico", UriKind.Relative));
            }
        }

        public Action OnShow
        {
            get
            {
                return delegate {
                    Window.GuessInputValue();
                };
            }
        }

        public bool RequiresElevatedPermissions
        {
            get
            {
                return true;
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
