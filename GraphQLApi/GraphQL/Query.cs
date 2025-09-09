using GraphQLApi.Model;
using GraphQLApi.Repositories;

namespace GraphQLApi.GraphQL;

// Step 4. Defining the GraphQL Schema.

public class Query
{
    private readonly BookRepository _repository;

    public Query(BookRepository repository)
    {
        _repository = repository;
    }

    public IQueryable<Book> GetBooks() => _repository.GetBooks();
    //[GraphQLName("bookById")]
    public Book GetBook(int id) => _repository.GetBook(id);
}