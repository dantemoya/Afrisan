using Npgsql;

namespace Afrisan.Nuevo.Api;

// Verifica transporte y objetos del esquema. No sustituye los stores en memoria.
public static class ConexionSupabase
{
    private static readonly string[] Tablas =
    [
        "Rol", "Usuario", "Cliente", "Sucursales", "Refrigerantes", "Cilindros",
        "OrdenTrabajo", "DetalleOT", "RetiroPrevisto", "Pesaje", "EvidenciaPesaje",
        "Despacho", "OperacionTerreno", "CustodiaActual", "RecepcionBodega", "RevisionOT",
        "Incidencia", "HistorialCilindro", "EventoOT", "LimiteSucursal",
        "LimiteSucursalHistorial", "MedicionLimite", "MedicionLimiteOperaciones",
        "RevisionExceso", "PreciosRefrigerantes", "Costos", "EstimacionEmisiones",
        "CertificadosDisposicion", "CertificadosCilindros", "Auditoria", "BajaCilindro"
    ];
    private static readonly string[] Vistas =
    [
        "Inventario", "ResumenOT", "PesajesRecepcion", "LimitesVigentes", "ExcesosRegistrados",
        "CostosOperaciones", "EmisionesEstimadas", "DisposicionesCertificadas", "PedidoYEjecucion"
    ];

    public static async Task<int> VerificarAsync(IConfiguration configuration)
    {
        var cadena = configuration.GetConnectionString("Supabase");
        if (string.IsNullOrWhiteSpace(cadena))
        {
            Console.Error.WriteLine("Falta ConnectionStrings:Supabase. Ejecuta Configurar-Supabase.ps1 para configurarla localmente.");
            return 1;
        }

        try
        {
            var opciones = new NpgsqlConnectionStringBuilder(cadena);
            if (string.IsNullOrWhiteSpace(opciones.Host) || string.IsNullOrWhiteSpace(opciones.Username)
                || string.IsNullOrWhiteSpace(opciones.Database))
                throw new ArgumentException();

            // Se exige cifrado y validación del certificado y nombre del servidor.
            opciones.SslMode = SslMode.VerifyFull;
            opciones.Timeout = 10;
            opciones.CommandTimeout = 15;
            opciones.IncludeErrorDetail = false;
            opciones.PersistSecurityInfo = false;
            opciones.Pooling = false;
            opciones.ApplicationName = "AFRISAN-Verificar-Supabase";

            using var limite = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await using var fuente = NpgsqlDataSource.Create(opciones.ConnectionString);
            await using var conexion = await fuente.OpenConnectionAsync(limite.Token);
            await using var transaccion = await conexion.BeginTransactionAsync(limite.Token);
            await using (var soloLectura = new NpgsqlCommand("SET TRANSACTION READ ONLY", conexion, transaccion))
                await soloLectura.ExecuteNonQueryAsync(limite.Token);

            const string sql = """
                SELECT n.nspname, c.relname, c.relkind::text, c.relrowsecurity
                FROM pg_catalog.pg_class c
                JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                WHERE n.nspname IN ('afrisan_actual', 'afrisan_bi_actual')
                  AND c.relkind IN ('r', 'p', 'v')
                """;
            var tablas = new HashSet<string>(StringComparer.Ordinal);
            var vistas = new HashSet<string>(StringComparer.Ordinal);
            var sinRls = new List<string>();
            await using (var comando = new NpgsqlCommand(sql, conexion, transaccion))
            await using (var lector = await comando.ExecuteReaderAsync(limite.Token))
            {
                while (await lector.ReadAsync(limite.Token))
                {
                    var esquema = lector.GetString(0);
                    var nombre = lector.GetString(1);
                    var tipo = lector.GetString(2);
                    if (esquema == "afrisan_actual" && tipo is "r" or "p")
                    {
                        tablas.Add(nombre);
                        if (!lector.GetBoolean(3)) sinRls.Add(nombre);
                    }
                    if (esquema == "afrisan_bi_actual" && tipo == "v") vistas.Add(nombre);
                }
            }
            await transaccion.RollbackAsync(limite.Token);
            Console.WriteLine("Conexión PostgreSQL correcta, con TLS y certificado verificado.");
            var faltantes = Tablas.Except(tablas).Select(t => $"afrisan_actual.{t}")
                .Concat(Vistas.Except(vistas).Select(v => $"afrisan_bi_actual.{v}")).ToArray();
            if (faltantes.Length > 0)
                Console.Error.WriteLine($"Objetos esperados ausentes: {string.Join(", ", faltantes)}.");
            if (sinRls.Count > 0)
                Console.Error.WriteLine($"Tablas sin RLS habilitado: {string.Join(", ", sinRls)}.");
            if (faltantes.Length > 0 || sinRls.Count > 0) return 2;

            Console.WriteLine("Presentes las 31 tablas y 9 vistas esperadas; RLS habilitado en las tablas.");
            Console.WriteLine("Diagnóstico de solo lectura. La aplicación sigue en memoria; faltan repositorios, autenticación persistente y Storage.");
            return 0;
        }
        catch (PostgresException ex)
        {
            Console.Error.WriteLine(ex.SqlState == "28P01"
                ? "PostgreSQL rechazó la autenticación. Revisa usuario y contraseña de la base de datos."
                : $"PostgreSQL rechazó la comprobación (SQLSTATE {ex.SqlState}). No se modificaron datos.");
        }
        catch (Exception ex) when (ex is NpgsqlException or TimeoutException or OperationCanceledException)
        {
            Console.Error.WriteLine("No se pudo completar la conexión. Revisa host, puerto, acceso de red y certificado raíz de Supabase. Si el host directo solo ofrece IPv6, usa Session pooler.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or System.IO.IOException)
        {
            Console.Error.WriteLine("Configuración inválida. Revisa ConnectionStrings:Supabase y, si se indicó, la ruta del certificado raíz.");
        }
        // No mostrar excepciones completas: pueden incluir datos de la conexión.
        return 1;
    }
}
