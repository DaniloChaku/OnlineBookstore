using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineBookstore.Api.Controllers;
using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.ServiceContracts;

namespace OnlineBookstore.UnitTests.Api.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockService;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockService = new Mock<IBookService>();
        _controller = new BooksController(_mockService.Object);
    }

    [Fact]
    public void Constructor_NullService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BooksController(null!));
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfBooks()
    {
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BookDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<BookDto>>(okResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkResult()
    {
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new BookDto());

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<BookDto>(okResult.Value);
    }

    [Fact]
    public async Task Search_ReturnsOkResult()
    {
        _mockService.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new List<BookDto>());

        var result = await _controller.Search("title", "author", "genre");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<BookDto>>(okResult.Value);
    }

    [Fact]
    public async Task Create_ValidBook_ReturnsCreatedAtAction()
    {
        _mockService.Setup(s => s.AddAsync(It.IsAny<BookCreateDto>())).ReturnsAsync(1);

        var result = await _controller.Create(new BookCreateDto());

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Update_ValidIdAndBook_ReturnsNoContent()
    {
        var result = await _controller.Update(1, new BookUpdateDto());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }
}