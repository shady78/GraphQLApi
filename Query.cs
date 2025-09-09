using System;
namespace GraphQLApi.GraphQL;

public class Query
{
    private readonly BookRepository _repository;

    public Query(BookRepository repository)
    {
        _repository = repository;
    }

    public IQueryable<Book> GetBooks() => _repository.GetBooks();
    public Book GetBook(int id) => _repository.GetBook(id);
}