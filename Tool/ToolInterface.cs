using System;
using System.Windows;
using System.Windows.Media;

namespace SupportTool.Tool
{
    interface ToolInterface
    {
        Window ToolWindow { get; }
        string MenuHeader { get; }
        ImageSource MenuIcon { get; }
        Action OnShow { get; }
        bool RequiresElevatedPermissions { get; }
    }
}
