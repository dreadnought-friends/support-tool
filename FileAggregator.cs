using System.Collections.Generic;
using System.IO;

namespace SupportTool
{
    class FileAggregator
    {
        public string TempDir { get; private set; }

        public List<AggregatedFile> AggregatedFiles { get; private set; } = new List<AggregatedFile>();

        public FileAggregator(string tempDir)
        {
            this.TempDir = tempDir;
        }

        /// <summary>
        /// Tell the aggregator to add a file that doesn't exist yet.
        ///   
        /// Example: adding foo.txt will return the full path where this file
        /// can be created (e.g. tmp\foo.txt) and will add this file to be
        /// copied.
        /// 
        /// It's up to the developer to ensure that the file exists afterwards,
        /// as it will throw an exception otherwise on archiving.
        /// </summary>
        /// <param name="destinationFileName">Filename relative from the destination directory</param>
        /// <returns>The destination file name</returns>
        public string AddVirtualFile(string destinationFileName)
        {
            string tempFileName = Path.Combine(TempDir, destinationFileName);

            AggregatedFiles.Add(new AggregatedFile(new FileInfo(tempFileName)));

            return tempFileName;
        }

        /// <summary>
        /// Tell the aggregator to add an existing file.
        /// 
        /// The destination file will be the name relative from the
        /// destination directory.
        /// </summary>
        /// <param name="sourceFileName">The file to be aggregated</param>
        /// <param name="destinationFileName">The relative filename it should get</param>
        /// <returns>The destination file name</returns>
        public string AddExistingFile(string sourceFileName, string destinationFileName)
        {
            destinationFileName = Path.Combine(TempDir, destinationFileName);

            AggregatedFiles.Add(new AggregatedFile(new FileInfo(sourceFileName), new FileInfo(destinationFileName)));

            return destinationFileName;
        }
    }
}
