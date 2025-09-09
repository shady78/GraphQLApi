using GraphQLApi.Model;
using GraphQLApi.Repositories;

namespace GraphQLApi;
// The Mutation class defines methods for modifying data.

public class Mutation
{
    private readonly BookRepository _repository;

    public Mutation(BookRepository repository)
    {
        _repository = repository;
    }

    public Book AddBook(string title, string author)
    {
        var book = new Book { Title = title, Author = author };
        return _repository.AddBook(book);
    }

    public Book UpdateBook(int id, string title, string author)
    {
        var book = new Book { Id = id, Title = title, Author = author };
        return _repository.UpdateBook(id, book);
    }

    public bool DeleteBook(int id)
    {
        return _repository.DeleteBook(id);
    }
}