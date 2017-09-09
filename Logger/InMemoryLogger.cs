using System.Collections.Generic;

namespace SupportTool.Logger
{
    class InMemoryLogger : LoggerInterface
    {
        public Queue<string> Queue { get; private set; } = new Queue<string>();

        public void Log(string message)
        {
            Queue.Enqueue(message);
        }

        public void Clear()
        {
            Queue.Clear();
        }
        
        public void Debug(string message)
        {
#if DEBUG
            Log(LoggingFormatter.FormatDebug(message));
#endif
        }
    }
}
