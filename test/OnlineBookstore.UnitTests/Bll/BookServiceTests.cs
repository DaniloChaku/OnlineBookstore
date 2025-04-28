using System.Linq.Expressions;
using Moq;
using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.Services;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Entities;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.UnitTests.Bll;

public class BookServiceTests
{
    private readonly Mock<IRepository<Book>> _mockRepository;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _mockRepository = new Mock<IRepository<Book>>();
        _service = new BookService(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookService(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Author 1" };
        var genre = new Genre { Id = 1, Name = "Fiction" };
        var books = new List<Book>
        {
            new Book
            {
                Id = 1,
                Title = "Book 1",
                Author = author,
                Genre = genre,
                AuthorId = author.Id,
                GenreId = genre.Id,
                Price = 19.99m,
                QuantityAvailable = 10
            },
            new Book
            {
                Id = 2,
                Title = "Book 2",
                Author = author,
                Genre = genre,
                AuthorId = author.Id,
                GenreId = genre.Id,
                Price = 29.99m,
                QuantityAvailable = 5
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync(books);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Book 1", result[0].Title);
        Assert.Equal("Book 2", result[1].Title);
        Assert.Equal("Author 1", result[0].AuthorName);
        Assert.Equal("Fiction", result[0].GenreName);
        _mockRepository.Verify(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsBook()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Author 1" };
        var genre = new Genre { Id = 1, Name = "Fiction" };
        var book = new Book
        {
            Id = 1,
            Title = "Book 1",
            Author = author,
            Genre = genre,
            AuthorId = author.Id,
            GenreId = genre.Id,
            Price = 19.99m,
            QuantityAvailable = 10
        };

        _mockRepository.Setup(r => r.GetByIdAsync(
            1,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync(book);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Book 1", result.Title);
        Assert.Equal("Author 1", result.AuthorName);
        Assert.Equal("Fiction", result.GenreName);
        Assert.Equal(19.99m, result.Price);
        _mockRepository.Verify(r => r.GetByIdAsync(
            1,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(
            99,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync((Book)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        _mockRepository.Verify(r => r.GetByIdAsync(
            99,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithCriteria_ReturnsMatchingBooks()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Author 1" };
        var genre = new Genre { Id = 1, Name = "Fiction" };
        var books = new List<Book>
        {
            new Book
            {
                Id = 1,
                Title = "Fantasy Novel",
                Author = author,
                Genre = genre,
                AuthorId = author.Id,
                GenreId = genre.Id,
                Price = 19.99m,
                QuantityAvailable = 10
            }
        };

        _mockRepository.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Book, bool>>>(),
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync(books);

        // Act
        var result = await _service.SearchAsync("Fantasy", "Author", "Fiction");

        // Assert
        Assert.Single(result);
        Assert.Equal("Fantasy Novel", result[0].Title);
        _mockRepository.Verify(r => r.FindAsync(
            It.IsAny<Expression<Func<Book, bool>>>(),
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ValidBook_ReturnsId()
    {
        // Arrange
        var dto = new BookCreateDto
        {
            Title = "New Book",
            AuthorId = 1,
            GenreId = 1,
            Price = 15.99m,
            QuantityAvailable = 20
        };

        Book savedBook = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Book>()))
            .Callback<Book>(b =>
            {
                b.Id = 10; // Simulate DB setting Id
                savedBook = b;
            })
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.Equal(10, result);
        Assert.NotNull(savedBook);
        Assert.Equal("New Book", savedBook.Title);
        Assert.Equal(1, savedBook.AuthorId);
        Assert.Equal(1, savedBook.GenreId);
        Assert.Equal(15.99m, savedBook.Price);
        Assert.Equal(20, savedBook.QuantityAvailable);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingBook_UpdatesBook()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Original Title",
            AuthorId = 1,
            GenreId = 1,
            Price = 10.99m,
            QuantityAvailable = 5
        };

        var dto = new BookUpdateDto
        {
            Title = "Updated Title",
            AuthorId = 2,
            GenreId = 2,
            Price = 12.99m,
            QuantityAvailable = 10
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockRepository.Setup(r => r.Update(It.IsAny<Book>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, dto);

        // Assert
        Assert.Equal("Updated Title", book.Title);
        Assert.Equal(2, book.AuthorId);
        Assert.Equal(2, book.GenreId);
        Assert.Equal(12.99m, book.Price);
        Assert.Equal(10, book.QuantityAvailable);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.Update(book), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingBook_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new BookUpdateDto
        {
            Title = "Updated Title",
            AuthorId = 2,
            GenreId = 2,
            Price = 12.99m,
            QuantityAvailable = 10
        };

        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(99, dto));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_DeletesBook()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Book 1" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockRepository.Setup(r => r.Delete(It.IsAny<Book>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.Delete(book), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingBook_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Book)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
