using System.Collections.Generic;
using System.IO;

namespace SupportTool
{
    class Runner
    {
        private Config config;
        private FileAggregator fileAggregator;
        private LoggerInterface logger;

        public Runner(Config config, FileAggregator fileAggregator, LoggerInterface logger)
        {
            this.config = config;
            this.fileAggregator = fileAggregator;
            this.logger = logger;
        }

        public void Run(List<CommandInterface> commands)
        {
            foreach (CommandInterface command in commands)
            {
                command.Execute(config, fileAggregator, logger);
            }
        }
    }
}
