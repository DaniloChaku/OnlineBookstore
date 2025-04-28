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
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookService(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Author 1" };
        var genre = new Genre { Id = 1, Name = "Fiction" };
        var expectedBooks = new List<Book>
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
            .ReturnsAsync(expectedBooks);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedBooks.Count, result.Count);
        Assert.Equal(expectedBooks[0].Title, result[0].Title);
        Assert.Equal(expectedBooks[1].Title, result[1].Title);
        Assert.Equal(author.Name, result[0].AuthorName);
        Assert.Equal(genre.Name, result[0].GenreName);

        _mockRepository.Verify(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsBook()
    {
        // Arrange
        const int bookId = 1;
        var author = new Author { Id = 1, Name = "Author 1" };
        var genre = new Genre { Id = 1, Name = "Fiction" };
        var expectedBook = new Book
        {
            Id = bookId,
            Title = "Book 1",
            Author = author,
            Genre = genre,
            AuthorId = author.Id,
            GenreId = genre.Id,
            Price = 19.99m,
            QuantityAvailable = 10
        };

        _mockRepository.Setup(r => r.GetByIdAsync(
            bookId,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _service.GetByIdAsync(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedBook.Id, result.Id);
        Assert.Equal(expectedBook.Title, result.Title);
        Assert.Equal(author.Name, result.AuthorName);
        Assert.Equal(genre.Name, result.GenreName);
        Assert.Equal(expectedBook.Price, result.Price);

        _mockRepository.Verify(r => r.GetByIdAsync(
            bookId,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;

        _mockRepository.Setup(r => r.GetByIdAsync(
            nonExistingId,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()))
            .ReturnsAsync((Book)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(nonExistingId));

        _mockRepository.Verify(r => r.GetByIdAsync(
            nonExistingId,
            It.IsAny<Expression<Func<Book, object>>>(),
            It.IsAny<Expression<Func<Book, object>>>()
        ), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithCriteria_ReturnsMatchingBooks()
    {
        // Arrange
        const string searchTitle = "Fantasy";
        const string searchAuthor = "Author";
        const string searchGenre = "Fiction";

        var author = new Author { Id = 1, Name = "Author 1" };
        var genre = new Genre { Id = 1, Name = "Fiction" };
        var expectedBooks = new List<Book>
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
            .ReturnsAsync(expectedBooks);

        // Act
        var result = await _service.SearchAsync(searchTitle, searchAuthor, searchGenre);

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedBooks[0].Title, result[0].Title);

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
        const int expectedId = 10;
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
                b.Id = expectedId; // Simulate DB setting Id
                savedBook = b;
            })
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.Equal(expectedId, result);
        Assert.NotNull(savedBook);
        Assert.Equal(dto.Title, savedBook.Title);
        Assert.Equal(dto.AuthorId, savedBook.AuthorId);
        Assert.Equal(dto.GenreId, savedBook.GenreId);
        Assert.Equal(dto.Price, savedBook.Price);
        Assert.Equal(dto.QuantityAvailable, savedBook.QuantityAvailable);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingBook_UpdatesBook()
    {
        // Arrange
        const int bookId = 1;
        var book = new Book
        {
            Id = bookId,
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

        _mockRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _mockRepository.Setup(r => r.Update(It.IsAny<Book>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(bookId, dto);

        // Assert
        Assert.Equal(dto.Title, book.Title);
        Assert.Equal(dto.AuthorId, book.AuthorId);
        Assert.Equal(dto.GenreId, book.GenreId);
        Assert.Equal(dto.Price, book.Price);
        Assert.Equal(dto.QuantityAvailable, book.QuantityAvailable);

        _mockRepository.Verify(r => r.GetByIdAsync(bookId), Times.Once);
        _mockRepository.Verify(r => r.Update(book), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingBook_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        var dto = new BookUpdateDto
        {
            Title = "Updated Title",
            AuthorId = 2,
            GenreId = 2,
            Price = 12.99m,
            QuantityAvailable = 10
        };

        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId)).ReturnsAsync((Book)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(nonExistingId, dto));

        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_DeletesBook()
    {
        // Arrange
        const int bookId = 1;
        var book = new Book { Id = bookId, Title = "Book 1" };

        _mockRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _mockRepository.Setup(r => r.Delete(It.IsAny<Book>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(bookId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(bookId), Times.Once);
        _mockRepository.Verify(r => r.Delete(book), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingBook_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;

        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId)).ReturnsAsync((Book)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(nonExistingId));

        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Book>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}