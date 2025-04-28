using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Dal.Data;

namespace OnlineBookstore.Api.Startup;

public class DatabaseMigrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(IServiceProvider serviceProvider, ILogger<DatabaseMigrator> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ApplyMigrationsAsync()
    {
        _logger.LogInformation("Starting database migration process");

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        ValidateDbContext(dbContext);
        await MigrateDatabaseAsync(dbContext);
    }

    private void ValidateDbContext(ApplicationDbContext dbContext)
    {
        if (dbContext == null)
        {
            _logger.LogError("Could not resolve ApplicationDbContext");
            throw new InvalidOperationException(
                "Could not resolve ApplicationDbContext. Make sure it is registered in the service collection.");
        }
    }

    private async Task MigrateDatabaseAsync(ApplicationDbContext dbContext)
    {
        _logger.LogInformation("Applying pending migrations");
        await dbContext.Database.MigrateAsync();

        _logger.LogInformation("Database migrations applied successfully");
    }
}
