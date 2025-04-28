using Moq;
using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.Services;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Entities;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.UnitTests.Bll;

public class GenreServiceTests
{
    private readonly Mock<IRepository<Genre>> _mockRepository;
    private readonly GenreService _service;

    public GenreServiceTests()
    {
        _mockRepository = new Mock<IRepository<Genre>>();
        _service = new GenreService(_mockRepository.Object);
    }

    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GenreService(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGenres()
    {
        // Arrange
        var genres = new List<Genre>
        {
            new Genre { Id = 1, Name = "Fiction" },
            new Genre { Id = 2, Name = "Non-Fiction" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Fiction", result[0].Name);
        Assert.Equal("Non-Fiction", result[1].Name);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsGenre()
    {
        // Arrange
        var genre = new Genre { Id = 1, Name = "Fiction" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Fiction", result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ValidGenre_ReturnsId()
    {
        // Arrange
        var dto = new GenreCreateDto { Name = "New Genre" };
        Genre savedGenre = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Genre>()))
            .Callback<Genre>(g =>
            {
                g.Id = 10; // Simulate DB setting Id
                savedGenre = g;
            })
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.Equal(10, result);
        Assert.NotNull(savedGenre);
        Assert.Equal("New Genre", savedGenre.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingGenre_UpdatesGenre()
    {
        // Arrange
        var genre = new Genre { Id = 1, Name = "Original Genre" };
        var dto = new GenreUpdateDto { Name = "Updated Genre" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);
        _mockRepository.Setup(r => r.Update(It.IsAny<Genre>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, dto);

        // Assert
        Assert.Equal("Updated Genre", genre.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.Update(genre), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingGenre_ThrowsNotFoundException()
    {
        // Arrange
        var dto = new GenreUpdateDto { Name = "Updated Genre" };
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(99, dto));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Genre>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingGenre_DeletesGenre()
    {
        // Arrange
        var genre = new Genre { Id = 1, Name = "Fiction" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);
        _mockRepository.Setup(r => r.Delete(It.IsAny<Genre>()));
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.Delete(genre), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingGenre_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));
        _mockRepository.Verify(r => r.GetByIdAsync(99), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Genre>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
