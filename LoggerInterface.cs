namespace SupportTool
{
    public interface LoggerInterface
    {
        void Log(string message);
        
        void Debug(string message);

        void Clear();
    }
}
