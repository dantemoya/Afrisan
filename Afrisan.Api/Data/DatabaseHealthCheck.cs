using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Afrisan.Api.Data;

public sealed class DatabaseHealthCheck(AfrisanDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await db.Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Unhealthy("Base de datos no disponible.");

            var pending = await db.Database.GetPendingMigrationsAsync(cancellationToken);
            return pending.Any()
                ? HealthCheckResult.Unhealthy("Hay migraciones pendientes.")
                : HealthCheckResult.Healthy();
        }
        catch (Exception)
        {
            // La respuesta pública no incluye conexión, credenciales ni detalles internos.
            return HealthCheckResult.Unhealthy("No se pudo comprobar la base de datos.");
        }
    }
}
