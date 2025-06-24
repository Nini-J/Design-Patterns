using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using BookCatalogApp.Domain;

namespace BookCatalogApp.Infrastructure
{
    public class FileBookRepository : IBookRepository
    {
        private readonly string _filePath = "books.json";
        private List<Book> _books = new();
        private int _nextId = 1;

        public FileBookRepository()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
                if (_books.Any()) _nextId = _books.Max(b => b.Id) + 1;
            }
        }

        private void SaveToFile()
        {
            var json = JsonSerializer.Serialize(_books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void AddBook(Book book)
        {
            book.Id = _nextId++;
            _books.Add(book);
            SaveToFile();
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
            SaveToFile();
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null) throw new InvalidOperationException("Book not found.");
            _books.Remove(book);
            SaveToFile();
        }

        public Book GetById(int id) => _books.FirstOrDefault(b => b.Id == id);
    }
    
}