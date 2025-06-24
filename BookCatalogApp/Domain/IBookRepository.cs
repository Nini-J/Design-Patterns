using System.Collections.Generic;
namespace BookCatalogApp.Domain
{
    public interface IBookRepository
    {
        void AddBook(Book book);
        List<Book> GetAllBooks();
        List<Book> FindByTitle(string title);
        List<Book> FindByAuthor(string author);
        void UpdateBook(Book book);
        void DeleteBook(int id);
        Book GetById(int id);
    }
}