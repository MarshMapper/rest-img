using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumCrawler.Models
{
    public class FolderCollection
    {
        public FolderCollection() { }

        // this is the starting folder path used by the crawler.  it is relative to the
        // web root.  the path to files found in the folder collection will be this path
        // plus the folder name plus the file name.
        public string StartingFolderWebPath { get; set; } = "/";
        private HashSet<Folder> _folders = new(new FolderComparer());

        public void AddFolder(Folder folder)
        {
            _folders.Add(folder);
        }

        public IEnumerable<Folder> Folders => _folders;
        public Folder? GetFolder(Folder folder)
        {
            Folder? foundFolder = null;
            _folders.TryGetValue(folder, out foundFolder);
            return foundFolder;
        }

        public Folder? GetFolder(string folderName)
        {
            return GetFolder(new Folder(folderName));
        }
    }
}
