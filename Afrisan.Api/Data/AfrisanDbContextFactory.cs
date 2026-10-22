using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Afrisan.Api.Data;

public sealed class AfrisanDbContextFactory : IDesignTimeDbContextFactory<AfrisanDbContext>
{
    public AfrisanDbContext CreateDbContext(string[] args)
    {
        var directory = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(directory, "Afrisan.Api.csproj")))
        {
            var apiDirectory = Path.Combine(directory, "Afrisan.Api");
            if (Directory.Exists(apiDirectory)) directory = apiDirectory;
        }

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(directory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<AfrisanDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Solo sirve para generar migraciones/SQL sin conectarse a un servidor.
        // La API normal exige una conexión configurada explícitamente.
        var connection = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=afrisan;Username=postgres";
        var options = new DbContextOptionsBuilder<AfrisanDbContext>()
            .UseNpgsql(connection).Options;
        return new AfrisanDbContext(options);
    }
}
