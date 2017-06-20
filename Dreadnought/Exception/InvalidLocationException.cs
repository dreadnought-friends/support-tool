using System;

namespace SupportTool.Dreadnought.Exception
{
    class InvalidLocationException : System.Exception
    {
        public string Location { get; private set; }

        public InvalidLocationException(string location): base(String.Format("Dreadnought installation was told to be at {0}, but can't be found here."))
        {
            Location = location;
        }
    }
}
