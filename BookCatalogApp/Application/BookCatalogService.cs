using BookCatalogApp.Domain;

namespace BookCatalogApp.Application
{
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