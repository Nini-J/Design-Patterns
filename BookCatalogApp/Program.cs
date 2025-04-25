using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

// --- Domain Layer ---
namespace BookCatalog.Domain
{
    public class Book
    {
        public int Id { get; set; }
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
        void DeleteBook(int id);
        Book GetById(int id);
    }
}

// --- Infrastructure Layer ---
namespace BookCatalog.Infrastructure
{
    using BookCatalog.Domain;
    

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

            if (existing == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Year = book.Year;
            existing.Genre = book.Genre;
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            _books.Remove(book);
        }

        public Book GetById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }
    }
    
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
            if (book.Id != 0)
                throw new ArgumentException("Book ID must be 0 (auto-assigned).");
            _repository.AddBook(book);
        }

        public List<Book> GetAllBooks() => _repository.GetAllBooks();

        public List<Book> FindByTitle(string title) => _repository.FindByTitle(title);

        public List<Book> FindByAuthor(string author) => _repository.FindByAuthor(author);

        public void UpdateBook(Book book) => _repository.UpdateBook(book);

        public void DeleteBook(int id) => _repository.DeleteBook(id);
        
        public Book GetBookById(int id) => _repository.GetById(id);
    }
}

 // --- Composition Root / Main ---
namespace BookCatalog
{
    using BookCatalog.Domain;
    using BookCatalog.Infrastructure;
    using BookCatalog.Application;

    class Program
    {
        static void Main()
        {
            // ✅ Choose repository type:
            //IBookRepository repository = new InMemoryBookRepository();
            IBookRepository repository = new FileBookRepository(); // ← Uncomment this to use file storage

            var service = new BookCatalogService(repository);

            while (true)
            {
                Console.WriteLine("\nBook Catalog Menu:");
                Console.WriteLine("1. List all books");
                Console.WriteLine("2. Add a book");
                Console.WriteLine("3. Search by title");
                Console.WriteLine("4. Search by author");
                Console.WriteLine("5. Update a book");
                Console.WriteLine("6. Delete a book");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                Console.WriteLine();

                if (input == "0") break;
                switch (input)
                {
                    case "1":
                        var allBooks = service.GetAllBooks();
                        if (allBooks.Count == 0)
                            Console.WriteLine("No books in the catalog.");
                        else
                            foreach (var book in allBooks)
                                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year}) - {book.Genre} [ID: {book.Id}]");
                        break;
                    case "2":
                        Console.Write("Enter title: ");
                        var title = Console.ReadLine();
                        Console.Write("Enter author: ");
                        var author = Console.ReadLine();
                        Console.Write("Enter year: ");
                        int year = int.TryParse(Console.ReadLine(), out var y) ? y : 0;
                        Console.Write("Enter genre: ");
                        var genre = Console.ReadLine();
                        var newBook = new Book { Title = title, Author = author, Year = year, Genre = genre };
                        service.AddBook(newBook);
                        Console.WriteLine($"Book added successfully. [ID: {newBook.Id}]");
                        break;
                    case "3":
                        Console.Write("Enter title to search: ");
                        var searchTitle = Console.ReadLine();
                        var foundByTitle = service.FindByTitle(searchTitle);
                        if (foundByTitle.Count == 0)
                            Console.WriteLine("No books found.");
                        else
                            foreach (var book in foundByTitle)
                                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year}) - {book.Genre} [ID: {book.Id}]");
                        break;
                    case "4":
                        Console.Write("Enter author to search: ");
                        var searchAuthor = Console.ReadLine();
                        var foundByAuthor = service.FindByAuthor(searchAuthor);
                        if (foundByAuthor.Count == 0)
                            Console.WriteLine("No books found.");
                        else
                            foreach (var book in foundByAuthor)
                                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year}) - {book.Genre} [ID: {book.Id}]");
                        break;
                    case "5":
                        Console.Write("Enter Book ID to update: ");
                        if (!int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            Console.WriteLine("Invalid ID.");
                            break;
                        }
                        var bookToUpdate = service.GetBookById(updateId);
                        if (bookToUpdate == null)
                        {
                            Console.WriteLine("Book not found.");
                            break;
                        }
                        Console.WriteLine($"Current: {bookToUpdate.Title} by {bookToUpdate.Author} ({bookToUpdate.Year}) - {bookToUpdate.Genre} [ID: {bookToUpdate.Id}]");
                        Console.Write("New Title: ");
                        bookToUpdate.Title = Console.ReadLine();
                        Console.Write("New Author: ");
                        bookToUpdate.Author = Console.ReadLine();
                        Console.Write("New Year: ");
                        bookToUpdate.Year = int.TryParse(Console.ReadLine(), out var ny) ? ny : 0;
                        Console.Write("New Genre: ");
                        bookToUpdate.Genre = Console.ReadLine();
                        service.UpdateBook(bookToUpdate);
                        Console.WriteLine("Book updated.");
                        break;
                    case "6":
                        Console.Write("Enter Book ID to delete: ");
                        if (!int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            Console.WriteLine("Invalid ID.");
                            break;
                        }
                        try
                        {
                            var bookToDelete = service.GetBookById(deleteId);
                            if (bookToDelete == null)
                            {
                                Console.WriteLine("Book not found.");
                                break;
                            }
                            Console.WriteLine($"Deleting: {bookToDelete.Title} by {bookToDelete.Author} [ID: {bookToDelete.Id}]");
                            service.DeleteBook(deleteId);
                            Console.WriteLine("Book deleted.");
                        }
                        catch
                        {
                            Console.WriteLine("Book not found.");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}   

