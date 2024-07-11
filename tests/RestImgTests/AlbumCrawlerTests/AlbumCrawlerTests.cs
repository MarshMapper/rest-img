using AlbumCrawler;
using AlbumCrawler.Models;

namespace RestImgTests.AlbumCrawlerTests
{
    public class AlbumCrawlerTests
    {
        private string _webRootPath = "/wwwroot";
        [Fact]
        public void CreateFoldersFromMatches_ReturnsUniqueFolders()
        {
            // Arrange
            string[] matchingPaths = new string[] { "\\testFolder\\file1.jpg", "\\testFolder\\file2.png", "\\testFolder\\subfolder\\file3.jpg" };
            FolderCrawler crawler = new FolderCrawler(_webRootPath);

            // Act
            FolderCollection folders = crawler.CreateFoldersFromMatches(matchingPaths);

            // Assert
            Assert.NotNull(folders);
            Assert.NotEmpty(folders.Folders);
        }
        [Fact]
        public void CreateFoldersFromMatches_ReturnsFoldersWithMatchingFiles()
        {
            // Arrange
            string[] matchingPaths = new string[] { "\\testFolder\\file1.jpg", "\\testFolder\\file2.png", "\\testFolder\\subfolder\\file3.jpg" };
            FolderCrawler crawler = new FolderCrawler(_webRootPath);

            // Act
            FolderCollection folders = crawler.CreateFoldersFromMatches(matchingPaths);

            // Assert
            foreach (Folder folder in folders.Folders)
            {
                Assert.Contains(folder.Files, file => file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png"));
            }
        }
        [Fact]
        public void CreateFoldersFromMatches_ReturnsFoldersWithCorrectNames()
        {
            // Arrange
            string[] matchingPaths = new string[] { "\\testFolder\\file1.jpg", "\\testFolder\\file2.png", "\\testFolder\\file3.jpg" };
            FolderCrawler crawler = new FolderCrawler(_webRootPath);

            // Act
            FolderCollection folders = crawler.CreateFoldersFromMatches(matchingPaths);

            // Assert
            foreach (Folder folder in folders.Folders)
            {
                Assert.StartsWith("testFolder", folder.Name);
            }
        }
        [Fact]
        public void CreateFoldersFromMatches_ReturnsFoldersWithCorrectFileNames()
        {
            // Arrange
            string[] matchingPaths = new string[] { "\\testFolder\\file1.jpg", "\\testFolder\\file2.png", "\\testFolder\\subfolder\\file3.jpg" };
            FolderCrawler crawler = new FolderCrawler(_webRootPath);

            // Act
            FolderCollection folders = crawler.CreateFoldersFromMatches(matchingPaths);

            // Assert
            foreach (Folder folder in folders.Folders)
            {
                foreach (MatchingFile file in folder.Files)
                {
                    Assert.StartsWith("file", file.Name);
                }
            }
        }
        [Fact]
        public void CreateFoldersFromMatches_ReturnsFoldersWithCorrectFileExtensions()
        {
            // Arrange
            string[] matchingPaths = new string[] { "\\testFolder\\file1.jpg", "\\testFolder\\file2.png", "\\testFolder\\subfolder\\file3.jpg" };
            FolderCrawler crawler = new FolderCrawler(_webRootPath);

            // Act
            FolderCollection folders = crawler.CreateFoldersFromMatches(matchingPaths);

            // Assert
            foreach (Folder folder in folders.Folders)
            {
                foreach (MatchingFile file in folder.Files)
                {
                    Assert.True(file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png"));
                }
            }
        }
        [Fact]
        public void CreateFoldersFromMatches_ReturnsFoldersWithCorrectFileCount()
        {
            // Arrange
            string[] matchingPaths = new string[] { "\\testFolder\\file1.jpg", "\\testFolder2\\file2.png", "\\testFolder\\subfolder\\file3.jpg" };
            FolderCrawler crawler = new FolderCrawler(_webRootPath);

            // Act
            FolderCollection folders = crawler.CreateFoldersFromMatches(matchingPaths);

            // Assert
            foreach (Folder folder in folders.Folders)
            {
                Assert.Single(folder.Files);
            }
        }
    }
}
