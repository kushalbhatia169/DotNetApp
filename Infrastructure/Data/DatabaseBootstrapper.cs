using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Infrastructure.Data;

public static class DatabaseBootstrapper
{
    public static async Task InitializeAsync(IServiceProvider services, string contentRootPath, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync(ct);
        await SqliteDataInitializer.ImportLegacyJsonAsync(dbContext, contentRootPath, ct);
    }
}
