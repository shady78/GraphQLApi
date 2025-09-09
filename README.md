# GraphQL Books API · .NET 8 + HotChocolate

<img width="1917" height="836" alt="image" src="https://github.com/user-attachments/assets/5dab1399-b80e-4f7d-9b38-43c822348df5" />
<img width="1903" height="825" alt="image" src="https://github.com/user-attachments/assets/b07c6ca6-a72a-418f-8092-8df794384197" />

A minimal GraphQL API built with **.NET 8** and **HotChocolate** that exposes a simple `Book` model with **Query** and **Mutation** operations. The project also includes the built-in **Banana Cake Pop** IDE for exploring the schema and running queries.

> Demo features: list all books, get a book by id, add/update/delete books (in-memory repository for simplicity).

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Run & Explore](#run--explore)
- [GraphQL Schema (SDL)](#graphql-schema-sdl)
- [Sample Operations](#sample-operations)
  - [Queries](#queries)
  - [Mutations](#mutations)
  - [cURL Examples](#curl-examples)
- [Project Structure](#project-structure)
- [Implementation Notes](#implementation-notes)
- [Troubleshooting](#troubleshooting)
- [Roadmap](#roadmap)
- [License](#license)

---

## Tech Stack

- **.NET 8** (ASP.NET Core Minimal API)
- **HotChocolate** (GraphQL server)
- **Banana Cake Pop** (embedded GraphQL IDE)

NuGet packages (core):
```bash
dotnet add package HotChocolate.AspNetCore
```

---

## Getting Started

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- Any REST/GraphQL client (optional). We’ll use Banana Cake Pop built in.

### Clone & Run
```bash
git clone <your-repo-url>
cd <your-repo-folder>

# Restore & run
dotnet restore
dotnet run
```

By default, HotChocolate maps the GraphQL endpoint to:
```
http://localhost:<PORT>/graphql
```
> In the screenshots I used port `7011`, so the endpoint would be `http://localhost:7011/graphql`.

---

## Run & Explore

Open the **Banana Cake Pop** IDE at:
```
http://localhost:<PORT>/graphql
```

- **Operation** tab → run queries/mutations  
- **Schema** tab → browse the SDL, types, and docs

---

## GraphQL Schema (SDL)

> This is the schema the API exposes. If you prefer a different field name (e.g., `bookById`), update the schema and resolver accordingly.

```graphql
schema {
  query: Query
  mutation: Mutation
}

type Book {
  id: Int!
  title: String
  author: String
}

type Query {
  books: [Book!]!
  book(id: Int!): Book!
}

type Mutation {
  addBook(title: String!, author: String!): Book!
  updateBook(id: Int!, title: String!, author: String!): Book!
  deleteBook(id: Int!): Boolean!
}
```

> Note: If you use a custom directive like `@cost`, make sure you register it server-side before adding it to the SDL.

---

## Sample Operations

### Queries

**Get all books**
```graphql
query {
  books {
    id
    title
    author
  }
}
```

**Get a single book by id**
```graphql
query {
  book(id: 2) {
    id
    title
    author
  }
}
```

> If you try `bookById`, you’ll get an error because the field is named `book` in the schema.

### Mutations

**Add**
```graphql
mutation {
  addBook(title: "DDD", author: "Eric Evans") {
    id
    title
    author
  }
}
```

**Update**
```graphql
mutation {
  updateBook(id: 2, title: "Clean Code (2nd Ed.)", author: "Robert C. Martin") {
    id
    title
    author
  }
}
```

**Delete**
```graphql
mutation {
  deleteBook(id: 2)
}
```

### cURL Examples

**Query all**
```bash
curl -X POST http://localhost:7011/graphql   -H "Content-Type: application/json"   -d '{ "query": "query { books { id title author } }" }'
```

**Query by id (with variables)**
```bash
curl -X POST http://localhost:7011/graphql   -H "Content-Type: application/json"   -d '{ "query": "query GetBook($id:Int!){ book(id:$id){ id title author } }", "variables": {"id": 2} }'
```

**Add book**
```bash
curl -X POST http://localhost:7011/graphql   -H "Content-Type: application/json"   -d '{ "query": "mutation { addBook(title:\"C# in Depth\", author:\"Jon Skeet\"){ id title author } }" }'
```

---

## Project Structure

> Your structure may differ; this is a friendly default.

```
.
├─ Program.cs                # Minimal API bootstrapping
├─ Models/
│  └─ Book.cs
├─ GraphQL/
│  ├─ Query.cs
│  ├─ Mutation.cs
│  └─ Types/                # (optional) custom object/input types
├─ Data/
│  └─ BookRepository.cs     # simple in-memory store for demo
└─ README.md
```

---

## Implementation Notes

**Program.cs (minimal example):**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.MapGraphQL(); // exposes /graphql and Banana Cake Pop

app.Run();
```

**Query.cs**
```csharp
public class Query
{
    public IEnumerable<Book> GetBooks([Service] BookRepository repo) => repo.All();

    public Book GetBook(int id, [Service] BookRepository repo) =>
        repo.GetById(id) ?? throw new GraphQLException(ErrorBuilder.New()
            .SetMessage($"Book with id {id} not found.")
            .SetCode("BOOK_NOT_FOUND")
            .Build());
}
```

**Mutation.cs**
```csharp
public class Mutation
{
    public Book AddBook(string title, string author, [Service] BookRepository repo) =>
        repo.Add(title, author);

    public Book UpdateBook(int id, string title, string author, [Service] BookRepository repo) =>
        repo.Update(id, title, author);

    public bool DeleteBook(int id, [Service] BookRepository repo) => repo.Delete(id);
}
```

**Book.cs**
```csharp
public record Book(int Id, string Title, string Author);
```

**BookRepository.cs** (in-memory)
```csharp
public class BookRepository
{
    private readonly List<Book> _books = new()
    {
        new(1, "C# in Depth", "Jon Skeet"),
        new(2, "Clean Code", "Robert C. Martin")
    };

    public IEnumerable<Book> All() => _books;

    public Book? GetById(int id) => _books.FirstOrDefault(b => b.Id == id);

    public Book Add(string title, string author)
    {
        var id = _books.Count == 0 ? 1 : _books.Max(b => b.Id) + 1;
        var book = new Book(id, title, author);
        _books.Add(book);
        return book;
    }

    public Book Update(int id, string title, string author)
    {
        var existing = GetById(id) ?? throw new InvalidOperationException("Not found");
        var updated = existing with { Title = title, Author = author };
        var idx = _books.FindIndex(b => b.Id == id);
        _books[idx] = updated;
        return updated;
    }

    public bool Delete(int id) => _books.RemoveAll(b => b.Id == id) > 0;
}
```

---

## Troubleshooting

- **“The field `bookById` does not exist on the type `Query`.”**  
  Use `book(id: Int!)` as defined in the schema, or add a new field named `bookById` and implement its resolver.

- **Banana Cake Pop not loading**  
  Ensure you called `app.MapGraphQL();` and you’re hitting the correct port. Try `http://localhost:<PORT>/graphql`.

- **Schema not updating**  
  Stop & rerun the app to rebuild, and hard-refresh the IDE to clear cached SDL.

---

## Roadmap

- ✅ Queries & Mutations (in-memory)
- ☐ Persistence with EF Core + SQLite/SQL Server  
- ☐ Pagination, filtering, sorting  
- ☐ Authorization rules  
- ☐ Input types & validation  
- ☐ Federation / schema stitching (advanced)

---

## License

This project is licensed under the **MIT License**. See [LICENSE](LICENSE) for details.

---
