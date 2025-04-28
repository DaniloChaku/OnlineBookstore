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
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GenreService(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGenres()
    {
        // Arrange
        var expectedGenres = new List<Genre>
        {
            new Genre { Id = 1, Name = "Fiction" },
            new Genre { Id = 2, Name = "Non-Fiction" }
        };
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedGenres);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedGenres.Count, result.Count);
        Assert.Equal(expectedGenres[0].Name, result[0].Name);
        Assert.Equal(expectedGenres[1].Name, result[1].Name);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsGenre()
    {
        // Arrange
        const int genreId = 1;
        var expectedGenre = new Genre { Id = genreId, Name = "Fiction" };
        _mockRepository.Setup(r => r.GetByIdAsync(genreId))
            .ReturnsAsync(expectedGenre);

        // Act
        var result = await _service.GetByIdAsync(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGenre.Id, result.Id);
        Assert.Equal(expectedGenre.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(genreId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(nonExistingId));
        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ValidGenre_ReturnsId()
    {
        // Arrange
        const int expectedId = 10;
        var dto = new GenreCreateDto { Name = "New Genre" };
        Genre savedGenre = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Genre>()))
            .Callback<Genre>(g =>
            {
                g.Id = expectedId;
                savedGenre = g;
            })
            .Returns(Task.CompletedTask);

        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAsync(dto);

        // Assert
        Assert.Equal(expectedId, result);
        Assert.NotNull(savedGenre);
        Assert.Equal(dto.Name, savedGenre.Name);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingGenre_UpdatesGenre()
    {
        // Arrange
        const int genreId = 1;
        var originalName = "Original Genre";
        var genre = new Genre { Id = genreId, Name = originalName };
        var dto = new GenreUpdateDto { Name = "Updated Genre" };

        _mockRepository.Setup(r => r.GetByIdAsync(genreId))
            .ReturnsAsync(genre);
        _mockRepository.Setup(r => r.Update(It.IsAny<Genre>()));
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(genreId, dto);

        // Assert
        Assert.Equal(dto.Name, genre.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(genreId), Times.Once);
        _mockRepository.Verify(r => r.Update(genre), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingGenre_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        var dto = new GenreUpdateDto { Name = "Updated Genre" };
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(nonExistingId, dto));
        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
        _mockRepository.Verify(r => r.Update(It.IsAny<Genre>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingGenre_DeletesGenre()
    {
        // Arrange
        const int genreId = 1;
        var expectedGenre = new Genre { Id = genreId, Name = "Fiction" };
        _mockRepository.Setup(r => r.GetByIdAsync(genreId))
            .ReturnsAsync(expectedGenre);
        _mockRepository.Setup(r => r.Delete(It.IsAny<Genre>()));
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(genreId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(genreId), Times.Once);
        _mockRepository.Verify(r => r.Delete(expectedGenre), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingGenre_ThrowsNotFoundException()
    {
        // Arrange
        const int nonExistingId = 99;
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Genre)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(nonExistingId));
        _mockRepository.Verify(r => r.GetByIdAsync(nonExistingId), Times.Once);
        _mockRepository.Verify(r => r.Delete(It.IsAny<Genre>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}