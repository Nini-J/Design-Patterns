using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookCatalogApp.Application;
using BookCatalogApp.Infrastructure;
using BookCatalogApp.Domain;

namespace BookCatalog.UnitTests
{
    [TestClass]
    public class BookCatalogServiceTests
    {
        [TestMethod]
        public void AddBook_ShouldAssignIdAndStoreBook()
        {
            var repo = new InMemoryBookRepository();
            var service = new BookCatalogService(repo);

            var book = new Book
            {
                Title = "Test Book",
                Author = "Tester",
                Genre = "Sci-Fi",
                Year = 2025
            };

            service.AddBook(book);
            var result = service.GetAllBooks();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Book", result[0].Title);
            Assert.IsTrue(result[0].Id > 0);
        }
    }
}