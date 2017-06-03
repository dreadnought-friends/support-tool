namespace SupportTool
{
    interface CommandInterface
    {
        void Execute(Config config, FileAggregator fileAggregator, LoggerInterface logger);
    }
}
