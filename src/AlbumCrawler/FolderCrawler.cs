using AlbumCrawler.Models;
using Microsoft.Extensions.FileSystemGlobbing;

namespace AlbumCrawler
{
    public class FolderCrawler
    {
        private string _startingFolderFullPath = ".";

        // method to "crawl" the specified folder and file all files matching
        // any of the specified patterns.  The method should return enumerable of
        // all Folders containing at least one file matching the pattern.  the
        // Folders contain the matching files.
        //
        // The FileSystemGlobbing package does the actual crawling of subfolders for us
        public IEnumerable<Folder> Crawl(string startingFolderFullPath, string[] patterns)
        {
            _startingFolderFullPath = startingFolderFullPath ?? ".";
            Matcher matcher = new();

            if (patterns != null && patterns.Length > 0)
            {
                matcher.AddIncludePatterns(patterns);
            }

            IEnumerable<string> matchingPaths = matcher.GetResultsInFullPath(_startingFolderFullPath);

            HashSet<Folder> folders = CreateFoldersFromMatches(matchingPaths);

            return folders;
        }

        /// <summary>
        /// takes a list of full paths containing matched files and creates a HashSet of the unique
        /// folders that contain those files.
        /// </summary>
        /// <param name="matchingPaths"></param>
        /// <returns></returns>
        public HashSet<Folder> CreateFoldersFromMatches(IEnumerable<string> matchingPaths)
        {
            HashSet<Folder> folders = new HashSet<Folder>(new FolderComparer());

            foreach (string path in matchingPaths)
            {
                // given these are matches, GetDirectoryName should always return a value
                string folderName = Path.GetDirectoryName(path) ?? _startingFolderFullPath;
                int leadingCharsToRemove = GetLeadingCharsToRemove(folderName);

                // remove the repeated starting folder path
                folderName = folderName.Remove(0, leadingCharsToRemove);

                // similarly, GetFileName should always return a value for matches
                string fileName = Path.GetFileName(path) ?? "";

                Folder currentFolder = new Folder(String.IsNullOrEmpty(folderName) ? "." : folderName);
                if (folders.TryGetValue(currentFolder, out Folder? existingFolder))
                {
                    currentFolder = existingFolder;
                }
                else
                {
                    // unique new folder found
                    folders.Add(currentFolder);
                }
                currentFolder.AddFile(fileName);
            }
            return folders;
        }
        /// <summary>
        /// In most cases, removes the starting folder path from the folder name.  exception is when files 
        /// are found in that starting folder, in which case the path is replaced with "."
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        protected int GetLeadingCharsToRemove(string folderName)
        {
            bool hasLeadingSeparator = folderName.Length > _startingFolderFullPath.Length &&
                folderName[_startingFolderFullPath.Length] == Path.DirectorySeparatorChar;
            return hasLeadingSeparator ? _startingFolderFullPath.Length + 1 : _startingFolderFullPath.Length;
        }
    }
}
