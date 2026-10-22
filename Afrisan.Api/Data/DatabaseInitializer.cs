using Microsoft.EntityFrameworkCore;

namespace Afrisan.Api.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AfrisanDbContext>();
        // Las tablas dibujadas manualmente en Supabase no son una migración EF.
        // Detectarlas antes de Migrate evita crear un segundo conjunto de tablas.
        var appliedBefore = await db.Database.GetAppliedMigrationsAsync(cancellationToken);
        if (!appliedBefore.Any())
        {
            var manualTables = await db.Database.SqlQueryRaw<int>("""
                SELECT count(*)::integer AS "Value" FROM pg_tables
                WHERE schemaname = current_schema() AND tablename IN
                    ('Usuario', 'Rol', 'Cliente', 'Sucursales', 'OrdenTrabajo', 'Movimientos', 'Refrigerantes', 'Costos')
                """).SingleAsync(cancellationToken);
            if (manualTables > 0)
                throw new InvalidOperationException(
                    "Se detectaron tablas manuales sin historial EF. Revisa Database/README.md y reconcilia el esquema antes de inicializar.");
        }
        await db.Database.MigrateAsync(cancellationToken);
        await IdentitySeeder.InicializarAsync(scope.ServiceProvider, configuration);

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitializer");
        var applied = await db.Database.GetAppliedMigrationsAsync(cancellationToken);
        logger.LogInformation("Base de datos inicializada con {MigrationCount} migraciones y roles de AFRISAN.", applied.Count());
        if (string.IsNullOrWhiteSpace(configuration["AdminSeed:Email"]))
            logger.LogWarning("No se creó administrador: configura AdminSeed mediante secretos o variables de entorno.");
    }
}
