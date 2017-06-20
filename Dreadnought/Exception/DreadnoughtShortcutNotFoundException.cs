namespace SupportTool.Dreadnought.Exception
{
    class DreadnoughtShortcutNotFoundException : System.Exception
    {
        public DreadnoughtShortcutNotFoundException(): base("Could not find the dreadnought shortcut on the desktop.")
        {

        }
    }
}
