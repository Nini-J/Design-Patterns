using System;
using System.Collections.Generic;
using System.Linq;
using BookCatalogApp.Domain; 

namespace BookCatalogApp.Infrastructure
{
    public class InMemoryBookRepository : IBookRepository
    {
        private readonly List<Book> _books = new();
        private int _nextId = 1;

        public void AddBook(Book book)
        {
            book.Id = _nextId++;
            _books.Add(book);
        }

        public List<Book> GetAllBooks() => new List<Book>(_books);

        public List<Book> FindByTitle(string title) =>
            _books.Where(b => b.Title != null &&
                              b.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        public List<Book> FindByAuthor(string author) =>
            _books.Where(b => b.Author != null &&
                              b.Author.IndexOf(author, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

        public void UpdateBook(Book book)
        {
            var existing = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existing == null) throw new InvalidOperationException("Book not found.");
            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Year = book.Year;
            existing.Genre = book.Genre;
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null) throw new InvalidOperationException("Book not found.");
            _books.Remove(book);
        }

        public Book GetById(int id) => _books.FirstOrDefault(b => b.Id == id);
    }
}