using GraphQLApi.Model;

namespace GraphQLApi.Repositories;

public class BookRepository
{
    private readonly List<Book> _books = new()
        {
            new Book { Id = 1, Title = "C# in Depth", Author = "Jon Skeet" },
            new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin" }
        };

    public IQueryable<Book> GetBooks() => _books.AsQueryable();

    public Book GetBook(int id) => _books.FirstOrDefault(x => x.Id == id)!;

    public Book AddBook(Book book)
    {
        book.Id = _books.Max(b => b.Id) + 1;
        _books.Add(book);
        return book;
    }

    public Book UpdateBook(int id, Book book)
    {
        var bookDetail = GetBook(id);
        if (bookDetail != null)
        {
            _books.Remove(bookDetail);
            book.Id = bookDetail.Id;
            _books.Add(book);
            return book;
        }
        return null!;
    }

    public bool DeleteBook(int id)
    {
        var bookDetail = GetBook(id);
        if (bookDetail != null)
        {
            _books.Remove(bookDetail);
            return true;
        }
        return false;
    }
}