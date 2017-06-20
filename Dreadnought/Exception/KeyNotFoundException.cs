namespace SupportTool.Dreadnought.Exception
{
    class KeyNotFoundException : System.Exception
    {
        public KeyNotFoundException(): base("Dreadnought installation could not be found.")
        {

        }
    }
}
