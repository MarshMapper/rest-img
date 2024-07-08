namespace AlbumCrawler.Models
{
    public class FolderComparer : IEqualityComparer<Folder>
    {
        public bool Equals(Folder? x, Folder? y)
        {
            return x?.Name == y?.Name;
        }

        public int GetHashCode(Folder obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
