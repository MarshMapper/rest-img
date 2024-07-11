using Microsoft.Extensions.FileSystemGlobbing;
using AlbumCrawler.Models;

namespace AlbumCrawler
{
    public class FolderCrawler
    {
        private string _webroot = "/wwwroot";
        public FolderCrawler(string webroot)
        {
            _webroot = webroot;
        }
        private string _startingFolderFullPath = String.Empty;

        public string StartingFolderWebPath { get; set; } = "/";

        // method to "crawl" the specified folder and file all files matching
        // any of the specified patterns.  The method should return enumerable of
        // all Folders containing at least one file matching the pattern.  the
        // Folders contain the matching files.
        //
        // The FileSystemGlobbing package does the actual crawling of subfolders for us
        public FolderCollection Crawl(string startingFolderWebPath, string[] patterns)
        {
            StartingFolderWebPath = startingFolderWebPath;
            _startingFolderFullPath = MapPath(startingFolderWebPath);

            Matcher matcher = new();

            if (patterns != null && patterns.Length > 0)
            {
                matcher.AddIncludePatterns(patterns);
            }

            IEnumerable<string> matchingPaths = matcher.GetResultsInFullPath(_startingFolderFullPath);

            return CreateFoldersFromMatches(matchingPaths);
        }

        /// <summary>
        /// takes a list of full paths containing matched files and creates a HashSet of the unique
        /// folders that contain those files.
        /// </summary>
        /// <param name="matchingPaths"></param>
        /// <returns></returns>
        public FolderCollection CreateFoldersFromMatches(IEnumerable<string> matchingPaths)
        {
            FolderCollection folders = new();
            folders.StartingFolderWebPath = StartingFolderWebPath;

            foreach (string path in matchingPaths)
            {
                // given these are matches, GetDirectoryName should always return a value
                string folderPath = Path.GetDirectoryName(path) ?? _startingFolderFullPath;
                // similarly, GetFileName should always return a value for matches
                string fileName = Path.GetFileName(path) ?? "";
                string folderName = new DirectoryInfo(folderPath).Name;

                int leadingCharsToRemove = GetLeadingCharsToRemove(folderPath);

                // remove the repeated starting folder path
                folderPath = folderPath.Remove(0, leadingCharsToRemove);

                Folder currentFolder = new Folder(String.IsNullOrEmpty(folderName) ? "." : folderName);
                currentFolder.WebPath = folderPath;

                Folder? existingFolder = folders.GetFolder(currentFolder);

                if (existingFolder != null)
                {
                    currentFolder = existingFolder;
                }
                else
                {
                    // unique new folder found
                    folders.AddFolder(currentFolder);
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
        /// <summary>
        /// maps the path relative to the web root to a full path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MapPath(string path)
        {
            if (path.EndsWith("/") || path.EndsWith("\\"))
            {
                path = path.Remove(path.Length - 1);
            }
            return _webroot + path;
        }
    }
}
