using System;
using System.Collections.Generic;
using System.Linq;

// --- Domain Layer ---
namespace BookCatalog.Domain
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
    }

    public interface IBookRepository
    {
        void AddBook(Book book);
        List<Book> GetAllBooks();
        List<Book> FindByTitle(string title);
        List<Book> FindByAuthor(string author);
        void UpdateBook(Book book);
        void DeleteBook(Guid id);
        Book GetById(Guid id);
    }
}

// --- Infrastructure Layer ---
namespace BookCatalog.Infrastructure
{
    using BookCatalog.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InMemoryBookRepository : IBookRepository
    {
        private readonly List<Book> _books = new List<Book>();

        public void AddBook(Book book)
        {
            if (_books.Any(b => b.Id == book.Id))
            {
                throw new InvalidOperationException("Book with this ID already exists.");
            }

            _books.Add(book);
        }

        public List<Book> GetAllBooks()
        {
            return new List<Book>(_books);
        }

        public List<Book> FindByTitle(string title)
        {
            return _books
                .Where(b => b.Title != null && 
                            b.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        public List<Book> FindByAuthor(string author)
        {
            return _books
                .Where(b => b.Author != null && 
                            b.Author.IndexOf(author, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        public void UpdateBook(Book book)
        {
            var existing = _books.FirstOrDefault(b => b.Id == book.Id);

            if (existing == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Year = book.Year;
            existing.Genre = book.Genre;
        }

        public void DeleteBook(Guid id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            _books.Remove(book);
        }

        public Book GetById(Guid id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }
    }
}


// --- Application Layer ---
namespace BookCatalog.Application
{
    using BookCatalog.Domain;
    public class BookCatalogService
    {
        private readonly IBookRepository _repository;
        public BookCatalogService(IBookRepository repository)
        {
            _repository = repository;
        }

        public void AddBook(Book book)
        {
            if (book.Id == Guid.Empty)
                book.Id = Guid.NewGuid();
            _repository.AddBook(book);
        }

        public List<Book> GetAllBooks() => _repository.GetAllBooks();

        public List<Book> FindByTitle(string title) => _repository.FindByTitle(title);

        public List<Book> FindByAuthor(string author) => _repository.FindByAuthor(author);

        public void UpdateBook(Book book) => _repository.UpdateBook(book);

        public void DeleteBook(Guid id) => _repository.DeleteBook(id);
    }
}

