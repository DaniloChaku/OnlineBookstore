using Microsoft.AspNetCore.Http;
using Moq;
using OnlineBookstore.Api.Middleware;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.UnitTests.Api.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _next;
    private readonly GlobalExceptionMiddleware _middleware;

    public GlobalExceptionMiddlewareTests()
    {
        _next = new Mock<RequestDelegate>();
        _middleware = new GlobalExceptionMiddleware(_next.Object);
    }

    [Fact]
    public async Task InvokeAsync_HandlesNotFoundException_Returns404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var notFoundException = new NotFoundException("Resource not found");

        _next.Setup(m => m(It.IsAny<HttpContext>())).Throws(notFoundException);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_HandlesGenericException_Returns500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var genericException = new Exception("An unexpected error occurred");

        _next.Setup(m => m(It.IsAny<HttpContext>())).Throws(genericException);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
    }
}