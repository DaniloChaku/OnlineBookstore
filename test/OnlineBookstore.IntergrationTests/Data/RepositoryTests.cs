using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Data;
using OnlineBookstore.Dal.Entities;

namespace OnlineBookstore.IntergrationTests.Data;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "<Pending>")]
public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_Should_Add_Entity()
    {
        var repository = new Repository<Author>(_context);

        var author = new Author { Name = "Test Author" };

        await repository.AddAsync(author);
        await repository.SaveChangesAsync();

        var authors = await repository.GetAllAsync();

        Assert.Single(authors);
        Assert.Equal(author.Name, authors[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Correct_Entity()
    {
        var repository = new Repository<Author>(_context);

        var author = new Author { Name = "Author 1" };
        await repository.AddAsync(author);
        await repository.SaveChangesAsync();

        var result = await repository.GetByIdAsync(author.Id);

        Assert.NotNull(result);
        Assert.Equal(author.Name, result!.Name);
    }

    [Fact]
    public async Task FindAsync_Should_Filter_Entities()
    {
        var repository = new Repository<Author>(_context);

        var author1 = new Author { Name = "Author 1" };
        var author2 = new Author { Name = "Author 2" };

        await repository.AddAsync(author1);
        await repository.AddAsync(author2);
        await repository.SaveChangesAsync();

        var result = await repository.FindAsync(a => a.Name.Contains("1"));

        Assert.Single(result);
        Assert.Equal(author1.Name, result[0].Name);
    }

    [Fact]
    public async Task Update_Should_Modify_Entity()
    {
        var repository = new Repository<Author>(_context);

        var author = new Author { Name = "Original Name" };
        await repository.AddAsync(author);
        await repository.SaveChangesAsync();

        // Act
        author.Name = "Updated Name";
        repository.Update(author);
        await repository.SaveChangesAsync();

        var updatedAuthor = await repository.GetByIdAsync(author.Id);

        Assert.NotNull(updatedAuthor);
        Assert.Equal(author.Name, updatedAuthor!.Name);
    }

    [Fact]
    public async Task Delete_Should_Remove_Entity()
    {
        var repository = new Repository<Author>(_context);

        var author = new Author { Name = "To be deleted" };
        await repository.AddAsync(author);
        await repository.SaveChangesAsync();

        repository.Delete(author);
        await repository.SaveChangesAsync();

        var authors = await repository.GetAllAsync();

        Assert.Empty(authors);
    }

    [Fact]
    public async Task GetAllAsync_Should_Include_Navigation_Properties()
    {
        var repository = new Repository<Author>(_context);

        var genre = new Genre { Name = "Fiction" };
        var book = new Book { Title = "Book 1", Genre = genre, Price = 10, QuantityAvailable = 5 };
        var author = new Author
        {
            Name = "Author With Books",
            Books = new List<Book> { book }
        };

        await _context.Genres.AddAsync(genre);
        await repository.AddAsync(author);
        await repository.SaveChangesAsync();

        var authors = await repository.GetAllAsync(a => a.Books);

        Assert.Single(authors);
        Assert.Single(authors[0].Books);
        Assert.Equal(book.Title, authors[0].Books.First().Title);
    }
}
