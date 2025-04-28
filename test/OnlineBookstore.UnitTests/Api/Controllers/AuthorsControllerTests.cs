using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Api.Controllers;
using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.ServiceContracts;

namespace OnlineBookstore.UnitTests.Api.Controllers;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorService> _mockService;
    private readonly AuthorsController _controller;

    public AuthorsControllerTests()
    {
        _mockService = new Mock<IAuthorService>();
        _controller = new AuthorsController(_mockService.Object);
    }

    [Fact]
    public void Constructor_NullService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new AuthorsController(null!));
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<AuthorDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<AuthorDto>>(okResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkResult()
    {
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new AuthorDto());

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<AuthorDto>(okResult.Value);
    }

    [Fact]
    public async Task Create_ValidAuthor_ReturnsCreatedAtAction()
    {
        _mockService.Setup(s => s.AddAsync(It.IsAny<AuthorCreateDto>())).ReturnsAsync(1);

        var result = await _controller.Create(new AuthorCreateDto());

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Update_ValidId_ReturnsNoContent()
    {
        var result = await _controller.Update(1, new AuthorUpdateDto());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }
}

