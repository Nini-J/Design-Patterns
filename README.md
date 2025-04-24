Features
Book Catalog system was designed to follow Clean Architecture principles, which emphasize separation of concerns and modular design. The application is organized into three main layers: Domain, Application (Use Cases), and Infrastructre.
Team 2 Members:

Mariam Rusishvili - Developer 1
Nini Jakhaia - Unit tests
Tamar Tateshvili - Project manager
Tamar Kvirikashvili - Developer 2
Clean Architecture Highlights

1. Domain Layer
Defines the core entity: Book, which contains properties like Id, Title, Author, Year, and Genre.

Also includes the IBookRepository interface, which declares the operations supported by any book storage mechanism (e.g., add, retrieve, search, update, delete).

2. Application Layer (Use Cases)
Implements the BookCatalogService class, which contains the business logic for managing books.

This service provides the following functionality:

AddBook(Book) — adds a book while ensuring a unique ID is assigned.

GetAllBooks() — retrieves all books from the repository.

FindByTitle(string) and FindByAuthor(string) — searches books using case-insensitive and partial matching.

UpdateBook(Book) — updates an existing book's details.

DeleteBook(Guid) — removes a book by ID with validation.


3. Infrastructure Layer
   
Contains InMemoryBookRepository, an in-memory implementation of IBookRepository.

Stores books using a List<Book> and handles all data access operations, including filtering and updating.


4. Dependency Injection
The BookCatalogService receives its repository dependency via constructor injection.

In the Main method, the repository is instantiated and passed to the service:

 IBookRepository repository = new InMemoryBookRepository();
var service = new BookCatalogService(repository);


Functional Capabilities
![Description](./images/0.png)


Add Book: Add a book by providing title, author, year, and genre.
![Description](./images/1.png)


List All Books: View all books currently in the catalog.

![Description](./images/2.png)

Search by Title: Find books with titles that match a partial or full search term (case-insensitive).

3.
Search by Author: Same as above, but for author names.

4.

Update Book: Change an existing book’s title, author, year, or genre.

5.

Delete Book: Remove a book from the catalog by its unique ID.

6.

Unit Testing


This project includes automated unit tests using MSTest to verify that key operations work as expected:
Each test checks a specific feature of the catalog to make sure it behaves correctly.

BookCatalogServiceTests.cs

Tests adding a book, searching by title, and deleting a book.
Ensures:
A book is successfully added to the list.
The correct book is found when searching by a keyword in the title.
A book is removed from the list after deletion.


FindByAuthorTests.cs

Focuses on searching by author name.
Confirms:
Case-insensitive searches return all books written by authors whose names match the keyword.
Works even if the author name has different cases (e.g., "Jane", "JANE").

UpdateBookTests.cs

Verifies that book details can be updated.
Ensures:
Title, author, and year of the book are updated correctly.
The update reflects immediately in the book list.

7.


