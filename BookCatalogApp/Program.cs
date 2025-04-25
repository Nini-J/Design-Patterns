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
    public class InMemoryBookRepository : IBookRepository
    {
        private readonly List<Book> _books = new List<Book>();

        public void AddBook(Book book)
        {
            if (_books.Any(b => b.Id == book.Id))
                throw new InvalidOperationException("Book with this ID already exists.");
            _books.Add(book);
        }

        public List<Book> GetAllBooks() => new List<Book>(_books);

        public List<Book> FindByTitle(string title)
        {
            return _books.Where(b => b.Title != null && b.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public List<Book> FindByAuthor(string author)
        {
            return _books.Where(b => b.Author != null && b.Author.IndexOf(author, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public void UpdateBook(Book book)
        {
            var existing = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existing == null) throw new InvalidOperationException("Book not found.");
            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Year = book.Year;
            existing.Genre = book.Genre;
        }

        public void DeleteBook(Guid id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null) throw new InvalidOperationException("Book not found.");
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

// --- Composition Root / Main ---
namespace BookCatalog
{
    using BookCatalog.Domain;
    using BookCatalog.Infrastructure;
    using BookCatalog.Application;

    class Program
    {
        static void Main(string[] args)
        {
            IBookRepository repository = new InMemoryBookRepository();
            var service = new BookCatalogService(repository);

            // Example books
            var book1 = new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Year = 1937, Genre = "Fantasy" };
            var book2 = new Book { Title = "1984", Author = "George Orwell", Year = 1949, Genre = "Dystopian" };
            service.AddBook(book1);
            service.AddBook(book2);

            while (true)
            {
                Console.WriteLine("\nBook Catalog Menu:");
                Console.WriteLine("1. List all books");
                Console.WriteLine("2. Add a book");
                Console.WriteLine("3. Search by title");
                Console.WriteLine("4. Search by author");
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
                                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year}) - {book.Genre}");
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
                        Console.WriteLine("Book added successfully.");
                        break;
                    case "3":
                        Console.Write("Enter title to search: ");
                        var searchTitle = Console.ReadLine();
                        var foundByTitle = service.FindByTitle(searchTitle);
                        if (foundByTitle.Count == 0)
                            Console.WriteLine("No books found.");
                        else
                            foreach (var book in foundByTitle)
                                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year}) - {book.Genre}");
                        break;
                    case "4":
                        Console.Write("Enter author to search: ");
                        var searchAuthor = Console.ReadLine();
                        var foundByAuthor = service.FindByAuthor(searchAuthor);
                        if (foundByAuthor.Count == 0)
                            Console.WriteLine("No books found.");
                        else
                            foreach (var book in foundByAuthor)
                                Console.WriteLine($"{book.Title} by {book.Author} ({book.Year}) - {book.Genre}");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}
