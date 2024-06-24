
public class Book
{
    public string Id { get; protected set; }
    public string Title { get; protected set; }
    public string Author { get; protected set; }
    public DateTime PublishedAt { get; protected set; }
    public string ISBN { get; protected set; }
    public bool IsAvailable { get; set; } = false;
    public ICollection<User> BorrowedUsers { get; set; } = new List<User>();

    public Book(BookDTO bookDTO)
    {
        Id = new Guid().ToString();
        Title = bookDTO.Title;
        Author = bookDTO.Author;
        ISBN = bookDTO.ISBN;
        PublishedAt = DateTime.UtcNow;
    }
}

public record BookDTO(string Title, string Author, string ISBN);


public class User
{
    public int Id { get; protected set; }
    public string FullName { get; protected set; }
    public string username { get; protected set; }

}

    public class BookService : IBookService
    {
        private readonly IBookRepository<Book> _bookRepository;
    private readonly IUserRepository<User> _userRepository;
        public BookService(IBookRepository<Book> bookRepository, IUserRepository<User> userRepository) {
            _bookRepository = bookRepository;
        _userRepository = userRepository;

        } 

        public async Task<bool> AddBook(BookDTO book)
        {
            var bookToCreate = new Book(book);
            var createBook = await _bookRepository.Add(bookToCreate);
            return true;
        }

        public Task<BookDTO> GetAllAvailableBooks()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LendBook(string userId, string bookId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user is null) return false;

            var book = await _bookRepository.GetBook(bookId);
            if (book is null) return false;

            if (book.IsAvailable == false)
            return false;

            book.BorrowedUsers.Add(user);
            book.IsAvailable = false;

            await _bookRepository.UpdateBook(book);
            return true;
        }
}

public interface IBookService
{
    Task<bool> AddBook(BookDTO book);
    Task<BookDTO> GetAllAvailableBooks();
    Task<bool> LendBook(string userId, string BookId);
}

public interface IBookRepository<T> where T : class
{
    Task<bool> Add(T entity);
    Task<Book> GetBook(string bookId);
    Task<bool> UpdateBook(Book book);

}

public interface IUserRepository<T> where T: class
{
    Task<User> GetUser(string userId);
}
