using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;


 // --- Composition Root / Main ---
namespace BookCatalog
{
    using BookCatalogApp.Domain;
    using BookCatalogApp.Infrastructure;
    using BookCatalogApp.Application;

    class Program
    {
        static void Main()
        {
            // Choose repository type:
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

