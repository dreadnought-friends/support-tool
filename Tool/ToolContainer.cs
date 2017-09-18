using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SupportTool.Tool
{
    class ToolContainer
    {
        private Config Config;
        private MenuItem ToolsMenuItem;
        private List<ToolInterface> Tools = new List<ToolInterface>();

        public ToolContainer(Config config, MenuItem toolsMenuItem)
        {
            Config = config;
            ToolsMenuItem = toolsMenuItem;
        }

        public void RegisterTool(ToolInterface tool)
        {
            Tools.Add(tool);

            MenuItem menuItem = new MenuItem()
            {
                Header = tool.MenuHeader
            };

            if (null != tool.MenuIcon)
            {
                menuItem.Icon = new Image()
                {
                    Source = tool.MenuIcon,
                    Stretch = Stretch.Fill,
                    Width = 16,
                    Height = 16,
                };
            }

            ToolsMenuItem.Items.Add(menuItem);

            if (tool.RequiresElevatedPermissions && !Config.IsElevated)
            {
                // explicitly not returning, allows enable overrides
                menuItem.IsEnabled = false;
                menuItem.Header += " (Requires Admin Permissions)";
            }

            // ensures that the tool window doesn't close itself but merely hides.
            tool.ToolWindow.Closing += new CancelEventHandler(delegate (object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                (sender as Window).Hide();
            });

            menuItem.Click += delegate
            {
                // in case the window is already visible, simply bring it to the foreground
                if (tool.ToolWindow.IsVisible)
                {
                    tool.ToolWindow.Activate();
                    return;
                }

                tool.ToolWindow.Show();
                tool.ToolWindow.Activate();

                // Anything the tool wants to do when the window becomes visible
                if (null != tool.OnShow)
                {
                    tool.OnShow.Invoke();
                }
            };
        }
    }
}
