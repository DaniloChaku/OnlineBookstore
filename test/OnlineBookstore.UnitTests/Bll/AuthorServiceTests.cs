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
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthorService(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAuthors()
    {
        // Arrange
        var expectedAuthors = new List<Author>
        {
            new Author { Id = 1, Name = "Author 1" },
            new Author { Id = 2, Name = "Author 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedAuthors);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedAuthors.Count, result.Count);
        Assert.Equal(expectedAuthors[0].Name, result[0].Name);
        Assert.Equal(expectedAuthors[1].Name, result[1].Name);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsAuthor()
    {
        // Arrange
        const int authorId = 1;
        var expectedAuthor = new Author { Id = authorId, Name = "Author 1" };
        _mockRepository.Setup(r => r.GetByIdAsync(authorId))
            .ReturnsAsync(expectedAuthor);

        // Act
        var result = await _service.GetByIdAsync(authorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAuthor.Id, result.Id);
        Assert.Equal(expectedAuthor.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(authorId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Author)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(nonExistingId));
        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ValidAuthor_ReturnsId()
    {
        // Arrange
        const int expectedId = 10;
        var dto = new AuthorCreateDto { Name = "New Author" };
        Author savedAuthor = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Author>()))
            .Callback<Author>(a =>
            {
                a.Id = expectedId;
                savedAuthor = a;
            })
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.Equal(expectedId, result);
        Assert.NotNull(savedAuthor);
        Assert.Equal(dto.Name, savedAuthor.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAuthor_UpdatesAuthor()
    {
        // Arrange
        const int authorId = 1;
        var originalName = "Original Name";
        var author = new Author { Id = authorId, Name = originalName };
        var dto = new AuthorUpdateDto { Name = "Updated Name" };

        _mockRepository.Setup(r => r.GetByIdAsync(authorId))
            .ReturnsAsync(author);
        _mockRepository.Setup(r => r.Update(It.IsAny<Author>()));
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(authorId, dto);

        // Assert
        Assert.Equal(dto.Name, author.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(authorId), Times.Once);
        _mockRepository.Verify(r => r.Update(author), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingAuthor_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        var dto = new AuthorUpdateDto { Name = "Updated Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Author)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(nonExistingId, dto));
        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Author>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingAuthor_DeletesAuthor()
    {
        // Arrange
        const int authorId = 1;
        var author = new Author { Id = authorId, Name = "Author 1" };
        _mockRepository.Setup(r => r.GetByIdAsync(authorId))
            .ReturnsAsync(author);
        _mockRepository.Setup(r => r.Delete(It.IsAny<Author>()));
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(authorId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(authorId), Times.Once);
        _mockRepository.Verify(r => r.Delete(author), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingAuthor_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Author)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(nonExistingId));
        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Author>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}