using Moq;
using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.Services;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Entities;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.UnitTests.Bll;

public class AuthorServiceTests
{
    private readonly Mock<IRepository<Author>> _mockRepository;
    private readonly AuthorService _service;

    public AuthorServiceTests()
    {
        _mockRepository = new Mock<IRepository<Author>>();
        _service = new AuthorService(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthorService(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new Author { Id = 1, Name = "Author 1" },
            new Author { Id = 2, Name = "Author 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(authors);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Author 1", result[0].Name);
        Assert.Equal("Author 2", result[1].Name);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsAuthor()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Author 1" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(author);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Author 1", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Author)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ValidAuthor_ReturnsId()
    {
        // Arrange
        var dto = new AuthorCreateDto { Name = "New Author" };
        Author savedAuthor = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Author>()))
            .Callback<Author>(a =>
            {
                a.Id = 10; 
                savedAuthor = a;
            })
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.Equal(10, result);
        Assert.NotNull(savedAuthor);
        Assert.Equal("New Author", savedAuthor.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAuthor_UpdatesAuthor()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Original Name" };
        var dto = new AuthorUpdateDto { Name = "Updated Name" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(author);
        _mockRepository.Setup(r => r.Update(It.IsAny<Author>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, dto);

        // Assert
        Assert.Equal("Updated Name", author.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.Update(author), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingAuthor_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new AuthorUpdateDto { Name = "Updated Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Author)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(99, dto));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Author>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingAuthor_DeletesAuthor()
    {
        // Arrange
        var author = new Author { Id = 1, Name = "Author 1" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(author);
        _mockRepository.Setup(r => r.Delete(It.IsAny<Author>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.Delete(author), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingAuthor_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Author)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Author>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}