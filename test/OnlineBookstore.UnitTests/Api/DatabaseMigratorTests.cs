using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineBookstore.Api.Startup;
using OnlineBookstore.Dal.Data;
using Microsoft.EntityFrameworkCore;

namespace OnlineBookstore.UnitTests.Api;

public class DatabaseMigratorTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<DatabaseFacade> _mockDatabase;
    private readonly FakeLogger<DatabaseMigrator> _fakeLogger;

    public DatabaseMigratorTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockScope = new Mock<IServiceScope>();
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        var options = new DbContextOptions<ApplicationDbContext>();
        _mockDbContext = new Mock<ApplicationDbContext>(options);
        _mockDatabase = new Mock<DatabaseFacade>(_mockDbContext.Object);
        _fakeLogger = new FakeLogger<DatabaseMigrator>();
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DatabaseMigrator(null!, _fakeLogger));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DatabaseMigrator(_mockServiceProvider.Object, null!));
    }
}
