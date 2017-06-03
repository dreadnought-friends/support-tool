using System.IO;

namespace SupportTool
{
    class AggregatedFile
    {
        public FileInfo From { get; private set; }
        public FileInfo To { get; private set; }

        /// <summary>
        /// Keeps track of which file to copy where.
        /// </summary>
        /// <param name="from">Source file</param>
        /// <param name="to">Target file in tmp</param>
        public AggregatedFile(FileInfo from, FileInfo to)
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// When the file is already created in tmp, there's no need to copy it.
        /// </summary>
        /// <param name="from"></param>
        public AggregatedFile(FileInfo from)
        {
            this.From = from;
            this.To = from;
        }
    }
}
