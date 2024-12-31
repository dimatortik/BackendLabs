using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2;

internal class Database
{
    public static void Migrate(WebApplication app)
    {
        using var container = app.Services.CreateScope();
        var dbContext = container.ServiceProvider.GetService<AppDbContext>();
        var pendingMigrations = dbContext!.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            dbContext.Database.Migrate();
        }
    }
}