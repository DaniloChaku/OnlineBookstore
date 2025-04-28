using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Api.Controllers;
using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.ServiceContracts;

namespace OnlineBookstore.UnitTests.Api.Controllers;

public class GenresControllerTests
{
    private readonly Mock<IGenreService> _mockService;
    private readonly GenresController _controller;

    public GenresControllerTests()
    {
        _mockService = new Mock<IGenreService>();
        _controller = new GenresController(_mockService.Object);
    }

    [Fact]
    public void Constructor_NullService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GenresController(null!));
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<GenreDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<GenreDto>>(okResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkResult()
    {
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new GenreDto());

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GenreDto>(okResult.Value);
    }

    [Fact]
    public async Task Create_ValidGenre_ReturnsCreatedAtAction()
    {
        _mockService.Setup(s => s.AddAsync(It.IsAny<GenreCreateDto>())).ReturnsAsync(1);

        var result = await _controller.Create(new GenreCreateDto());

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Update_ValidId_ReturnsNoContent()
    {
        var result = await _controller.Update(1, new GenreUpdateDto());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }
}

