using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Afrisan.Api.Migrations
{
    /// <inheritdoc />
    public partial class AlinearMerRolesSucursalesIntervenciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La migración parte del esquema EF anterior. No elige arbitrariamente
            // un rol si una cuenta tiene varios, ni redondea pesajes históricos.
            migrationBuilder.Sql("""
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM "AspNetUserRoles" GROUP BY "UserId" HAVING count(*) > 1) THEN
                        RAISE EXCEPTION 'Hay cuentas con varios roles. Define un solo rol antes de migrar.';
                    END IF;
                    IF EXISTS (
                        SELECT 1 FROM (
                            SELECT "PesoKg" AS peso FROM "MovimientosInventario"
                            UNION ALL SELECT "CapacidadKg" FROM "Cilindros"
                            UNION ALL SELECT "PesoActualKg" FROM "Cilindros"
                            UNION ALL SELECT "TaraKg" FROM "Cilindros"
                        ) p WHERE abs(peso) >= 1000000000000000 OR peso <> round(peso, 3)
                    ) THEN
                        RAISE EXCEPTION 'Hay pesajes que no caben exactamente en numeric(18,3). Revisa los datos antes de migrar.';
                    END IF;
                END; $$;
                CREATE TEMP TABLE "__afrisan_roles_previos" ON COMMIT DROP
                    AS SELECT "UserId", "RoleId" FROM "AspNetUserRoles";
                """);

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_CilindroId",
                table: "MovimientosInventario");

            migrationBuilder.AddColumn<string>(
                name: "AutorizadorId",
                table: "OrdenesTrabajo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoExterno",
                table: "OrdenesTrabajo",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "OrdenesTrabajo",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "OrdenesTrabajo",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Local");

            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "OrdenesTrabajo",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TecnicoAsignadoId",
                table: "OrdenesTrabajo",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PesoKg",
                table: "MovimientosInventario",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<bool>(
                name: "CustodiaAbierta",
                table: "MovimientosInventario",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DespachoId",
                table: "MovimientosInventario",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPesajeRuta",
                table: "MovimientosInventario",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdOperacion",
                table: "MovimientosInventario",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "MovimientosInventario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FactorGwp",
                table: "GasesRefrigerantes",
                type: "numeric(12,3)",
                precision: 12,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuenteGwp",
                table: "GasesRefrigerantes",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodologiaGwp",
                table: "GasesRefrigerantes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaraKg",
                table: "Cilindros",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PesoActualKg",
                table: "Cilindros",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "CapacidadKg",
                table: "Cilindros",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaImportacion",
                table: "Cilindros",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroSerie",
                table: "Cilindros",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RolId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "AspNetUsers" u SET "RolId" = r."RoleId"
                FROM "__afrisan_roles_previos" r WHERE r."UserId" = u."Id";

                -- Solo se emparejan eventos consecutivos salida/retorno del mismo
                -- cilindro. No se inventa el usuario de movimientos antiguos.
                WITH secuencia AS (
                    SELECT "Id", "TipoMovimiento",
                        lag("Id") OVER w AS anterior,
                        lag("TipoMovimiento") OVER w AS tipo_anterior
                    FROM "MovimientosInventario"
                    WINDOW w AS (PARTITION BY "CilindroId" ORDER BY "FechaMovimiento", "Id")
                )
                UPDATE "MovimientosInventario" r
                SET "DespachoId" = d."Id", "OrdenTrabajoId" = coalesce(r."OrdenTrabajoId", d."OrdenTrabajoId")
                FROM secuencia s JOIN "MovimientosInventario" d ON d."Id" = s.anterior
                WHERE r."Id" = s."Id" AND s."TipoMovimiento" = 'Retorno' AND s.tipo_anterior = 'Despacho';

                WITH ultimos AS (
                    SELECT DISTINCT ON ("CilindroId") "Id", "CilindroId", "TipoMovimiento"
                    FROM "MovimientosInventario" ORDER BY "CilindroId", "FechaMovimiento" DESC, "Id" DESC
                )
                UPDATE "MovimientosInventario" d SET "CustodiaAbierta" = true
                FROM ultimos u JOIN "Cilindros" c ON c."Id" = u."CilindroId"
                WHERE d."Id" = u."Id" AND u."TipoMovimiento" = 'Despacho'
                    AND lower(btrim(c."Estado")) = 'despachado';
                """);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MovimientosInventario_Id_CilindroId",
                table: "MovimientosInventario",
                columns: new[] { "Id", "CilindroId" });

            migrationBuilder.CreateTable(
                name: "CausasFalla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CausasFalla", x => x.Id);
                    table.CheckConstraint("CK_CausasFalla_Codigo", "length(btrim(\"Codigo\")) > 0");
                });

            migrationBuilder.CreateTable(
                name: "CertificadosDisposicion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Folio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmpresaEmisora = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaEmision = table.Column<DateOnly>(type: "date", nullable: false),
                    RutaDocumento = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificadosDisposicion", x => x.Id);
                    table.CheckConstraint("CK_Certificados_Folio", "length(btrim(\"Folio\")) > 0");
                    table.ForeignKey(
                        name: "FK_CertificadosDisposicion_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Rut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.CheckConstraint("CK_Clientes_Nombre", "length(btrim(\"Nombre\")) > 0");
                });

            migrationBuilder.CreateTable(
                name: "PreciosRefrigerantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GasRefrigeranteId = table.Column<int>(type: "integer", nullable: false),
                    VigenteDesde = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrecioKg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosRefrigerantes", x => x.Id);
                    table.CheckConstraint("CK_Precios_Importe", "\"PrecioKg\" >= 0");
                    table.CheckConstraint("CK_Precios_Moneda", "\"Moneda\" ~ '^[A-Z]{3}$'");
                    table.ForeignKey(
                        name: "FK_PreciosRefrigerantes_GasesRefrigerantes_GasRefrigeranteId",
                        column: x => x.GasRefrigeranteId,
                        principalTable: "GasesRefrigerantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CertificadosCilindros",
                columns: table => new
                {
                    CertificadoDisposicionId = table.Column<int>(type: "integer", nullable: false),
                    CilindroId = table.Column<int>(type: "integer", nullable: false),
                    PesoEntregadoKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificadosCilindros", x => new { x.CertificadoDisposicionId, x.CilindroId });
                    table.CheckConstraint("CK_CertificadosCilindros_Peso", "\"PesoEntregadoKg\" IS NULL OR \"PesoEntregadoKg\" >= 0");
                    table.ForeignKey(
                        name: "FK_CertificadosCilindros_CertificadosDisposicion_CertificadoDi~",
                        column: x => x.CertificadoDisposicionId,
                        principalTable: "CertificadosDisposicion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificadosCilindros_Cilindros_CilindroId",
                        column: x => x.CilindroId,
                        principalTable: "Cilindros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.Id);
                    table.CheckConstraint("CK_Sucursales_Nombre", "length(btrim(\"Nombre\")) > 0");
                    table.ForeignKey(
                        name: "FK_Sucursales_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CapacidadNominalKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    GasRefrigeranteId = table.Column<int>(type: "integer", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                    table.UniqueConstraint("AK_Equipos_Id_SucursalId", x => new { x.Id, x.SucursalId });
                    table.CheckConstraint("CK_Equipos_Capacidad", "\"CapacidadNominalKg\" > 0");
                    table.CheckConstraint("CK_Equipos_Codigo", "length(btrim(\"Codigo\")) > 0");
                    table.ForeignKey(
                        name: "FK_Equipos_GasesRefrigerantes_GasRefrigeranteId",
                        column: x => x.GasRefrigeranteId,
                        principalTable: "GasesRefrigerantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipos_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Intervenciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOperacion = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    OrdenTrabajoId = table.Column<int>(type: "integer", nullable: false),
                    CilindroId = table.Column<int>(type: "integer", nullable: false),
                    DespachoId = table.Column<int>(type: "integer", nullable: false),
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    CausaFallaId = table.Column<int>(type: "integer", nullable: false),
                    TecnicoId = table.Column<string>(type: "text", nullable: false),
                    AutorizadorId = table.Column<string>(type: "text", nullable: true),
                    FechaIntervencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KgUtilizados = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    PesoRestanteBrutoKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    FotoBalanzaRuta = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    EstadoIntervencion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlanCorreccion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EstadoSemaforo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FactorGwpAplicado = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: true),
                    FuenteGwpAplicada = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MetodologiaGwpAplicada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrecioKgAplicado = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    Moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    KgEmitidos = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    EmisionesCo2eToneladas = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: true, computedColumnSql: "\"KgEmitidos\" * \"FactorGwpAplicado\" / 1000.0", stored: true),
                    CostoTotal = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: true, computedColumnSql: "\"KgUtilizados\" * \"PrecioKgAplicado\"", stored: true),
                    EstadoSincronizacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DispositivoId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaCreacionLocal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervenciones", x => x.Id);
                    table.CheckConstraint("CK_Intervenciones_Estado", "\"EstadoIntervencion\" IN ('Registrada', 'Revisada', 'Cerrada')");
                    table.CheckConstraint("CK_Intervenciones_Factores", "(\"FactorGwpAplicado\" IS NULL OR \"FactorGwpAplicado\" >= 0) AND (\"PrecioKgAplicado\" IS NULL OR \"PrecioKgAplicado\" >= 0)");
                    table.CheckConstraint("CK_Intervenciones_FotoCierre", "\"EstadoIntervencion\" <> 'Cerrada' OR (\"FechaCierre\" IS NOT NULL AND length(btrim(coalesce(\"FotoBalanzaRuta\", ''))) > 0 AND \"PesoRestanteBrutoKg\" IS NOT NULL)");
                    table.CheckConstraint("CK_Intervenciones_Moneda", "\"PrecioKgAplicado\" IS NULL OR (\"Moneda\" IS NOT NULL AND \"Moneda\" ~ '^[A-Z]{3}$')");
                    table.CheckConstraint("CK_Intervenciones_Pesos", "\"KgUtilizados\" > 0 AND (\"KgEmitidos\" IS NULL OR \"KgEmitidos\" >= 0) AND (\"PesoRestanteBrutoKg\" IS NULL OR \"PesoRestanteBrutoKg\" >= 0)");
                    table.CheckConstraint("CK_Intervenciones_PlanRojo", "\"EstadoSemaforo\" <> 'Rojo' OR length(btrim(coalesce(\"PlanCorreccion\", ''))) > 0");
                    table.CheckConstraint("CK_Intervenciones_Semaforo", "\"EstadoSemaforo\" IN ('Pendiente', 'Verde', 'Amarillo', 'Rojo')");
                    table.CheckConstraint("CK_Intervenciones_Sync", "\"EstadoSincronizacion\" IN ('Pendiente', 'Sincronizado', 'Conflicto')");
                    table.ForeignKey(
                        name: "FK_Intervenciones_AspNetUsers_AutorizadorId",
                        column: x => x.AutorizadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_AspNetUsers_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_CausasFalla_CausaFallaId",
                        column: x => x.CausaFallaId,
                        principalTable: "CausasFalla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_Cilindros_CilindroId",
                        column: x => x.CilindroId,
                        principalTable: "Cilindros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_Equipos_EquipoId_SucursalId",
                        columns: x => new { x.EquipoId, x.SucursalId },
                        principalTable: "Equipos",
                        principalColumns: new[] { "Id", "SucursalId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_MovimientosInventario_DespachoId_CilindroId",
                        columns: x => new { x.DespachoId, x.CilindroId },
                        principalTable: "MovimientosInventario",
                        principalColumns: new[] { "Id", "CilindroId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_OrdenesTrabajo_OrdenTrabajoId",
                        column: x => x.OrdenTrabajoId,
                        principalTable: "OrdenesTrabajo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Intervenciones_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    IntervencionId = table.Column<int>(type: "integer", nullable: true),
                    Accion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Entidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegistroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "jsonb", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "jsonb", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auditorias_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auditorias_Intervenciones_IntervencionId",
                        column: x => x.IntervencionId,
                        principalTable: "Intervenciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_AutorizadorId",
                table: "OrdenesTrabajo",
                column: "AutorizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_Origen_CodigoExterno",
                table: "OrdenesTrabajo",
                columns: new[] { "Origen", "CodigoExterno" },
                unique: true,
                filter: "\"CodigoExterno\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_SucursalId",
                table: "OrdenesTrabajo",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_TecnicoAsignadoId",
                table: "OrdenesTrabajo",
                column: "TecnicoAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_CilindroId_FechaMovimiento",
                table: "MovimientosInventario",
                columns: new[] { "CilindroId", "FechaMovimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_DespachoId",
                table: "MovimientosInventario",
                column: "DespachoId",
                unique: true,
                filter: "\"DespachoId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_DespachoId_CilindroId",
                table: "MovimientosInventario",
                columns: new[] { "DespachoId", "CilindroId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdOperacion",
                table: "MovimientosInventario",
                column: "IdOperacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_UsuarioId",
                table: "MovimientosInventario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "UX_Movimientos_CustodiaAbierta",
                table: "MovimientosInventario",
                column: "CilindroId",
                unique: true,
                filter: "\"CustodiaAbierta\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movimientos_Custodia",
                table: "MovimientosInventario",
                sql: "NOT \"CustodiaAbierta\" OR \"TipoMovimiento\" = 'Despacho'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movimientos_Peso",
                table: "MovimientosInventario",
                sql: "\"PesoKg\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movimientos_Retorno",
                table: "MovimientosInventario",
                sql: "\"DespachoId\" IS NULL OR \"TipoMovimiento\" = 'Retorno'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Gases_FactorGwp",
                table: "GasesRefrigerantes",
                sql: "\"FactorGwp\" IS NULL OR \"FactorGwp\" >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Cilindros_NumeroSerie",
                table: "Cilindros",
                column: "NumeroSerie",
                unique: true,
                filter: "\"NumeroSerie\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RolId",
                table: "AspNetUsers",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Entidad_RegistroId",
                table: "Auditorias",
                columns: new[] { "Entidad", "RegistroId" });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Fecha",
                table: "Auditorias",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_IntervencionId",
                table: "Auditorias",
                column: "IntervencionId");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_UsuarioId",
                table: "Auditorias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CausasFalla_Codigo",
                table: "CausasFalla",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosCilindros_CilindroId",
                table: "CertificadosCilindros",
                column: "CilindroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosDisposicion_EmpresaEmisora_Folio",
                table: "CertificadosDisposicion",
                columns: new[] { "EmpresaEmisora", "Folio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosDisposicion_UsuarioId",
                table: "CertificadosDisposicion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Rut",
                table: "Clientes",
                column: "Rut",
                unique: true,
                filter: "\"Rut\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_GasRefrigeranteId",
                table: "Equipos",
                column: "GasRefrigeranteId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_SucursalId_Codigo",
                table: "Equipos",
                columns: new[] { "SucursalId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_AutorizadorId",
                table: "Intervenciones",
                column: "AutorizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_CausaFallaId",
                table: "Intervenciones",
                column: "CausaFallaId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_CilindroId",
                table: "Intervenciones",
                column: "CilindroId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_DespachoId_CilindroId",
                table: "Intervenciones",
                columns: new[] { "DespachoId", "CilindroId" });

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_EquipoId_FechaIntervencion",
                table: "Intervenciones",
                columns: new[] { "EquipoId", "FechaIntervencion" });

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_EquipoId_SucursalId",
                table: "Intervenciones",
                columns: new[] { "EquipoId", "SucursalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_IdOperacion",
                table: "Intervenciones",
                column: "IdOperacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_OrdenTrabajoId_CilindroId",
                table: "Intervenciones",
                columns: new[] { "OrdenTrabajoId", "CilindroId" });

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_SucursalId",
                table: "Intervenciones",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervenciones_TecnicoId",
                table: "Intervenciones",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosRefrigerantes_GasRefrigeranteId_Moneda_VigenteDesde",
                table: "PreciosRefrigerantes",
                columns: new[] { "GasRefrigeranteId", "Moneda", "VigenteDesde" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_ClienteId_Nombre",
                table: "Sucursales",
                columns: new[] { "ClienteId", "Nombre" });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RolId",
                table: "AspNetUsers",
                column: "RolId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInventario_AspNetUsers_UsuarioId",
                table: "MovimientosInventario",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInventario_MovimientosInventario_DespachoId_Cili~",
                table: "MovimientosInventario",
                columns: new[] { "DespachoId", "CilindroId" },
                principalTable: "MovimientosInventario",
                principalColumns: new[] { "Id", "CilindroId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesTrabajo_AspNetUsers_AutorizadorId",
                table: "OrdenesTrabajo",
                column: "AutorizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesTrabajo_AspNetUsers_TecnicoAsignadoId",
                table: "OrdenesTrabajo",
                column: "TecnicoAsignadoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesTrabajo_Sucursales_SucursalId",
                table: "OrdenesTrabajo",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // La aplicación accede por su backend. La Data API de Supabase no
            // recibe permisos para leer contraseñas, auditoría ni tablas operativas.
            migrationBuilder.Sql("""
                DO $$ DECLARE tabla text; BEGIN
                    FOREACH tabla IN ARRAY ARRAY[
                        'AspNetUsers', 'AspNetRoles', 'AspNetUserClaims', 'AspNetUserLogins',
                        'AspNetUserTokens', 'AspNetRoleClaims', 'Clientes', 'Sucursales', 'Equipos',
                        'CausasFalla', 'Cilindros', 'GasesRefrigerantes', 'PreciosRefrigerantes',
                        'OrdenesTrabajo', 'MovimientosInventario', 'Intervenciones', 'Auditorias',
                        'CertificadosDisposicion', 'CertificadosCilindros'
                    ] LOOP
                        EXECUTE format('ALTER TABLE %I ENABLE ROW LEVEL SECURITY', tabla);
                        IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'anon') THEN
                            EXECUTE format('REVOKE ALL ON TABLE %I FROM anon', tabla);
                        END IF;
                        IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'authenticated') THEN
                            EXECUTE format('REVOKE ALL ON TABLE %I FROM authenticated', tabla);
                        END IF;
                    END LOOP;
                END; $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Conservar la asignación al volver al esquema de roles de Identity anterior.
            migrationBuilder.Sql("""
                CREATE TEMP TABLE "__afrisan_roles_actuales" ON COMMIT DROP AS
                    SELECT "Id" AS "UserId", "RolId" AS "RoleId"
                    FROM "AspNetUsers" WHERE "RolId" IS NOT NULL;
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RolId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInventario_AspNetUsers_UsuarioId",
                table: "MovimientosInventario");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInventario_MovimientosInventario_DespachoId_Cili~",
                table: "MovimientosInventario");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesTrabajo_AspNetUsers_AutorizadorId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesTrabajo_AspNetUsers_TecnicoAsignadoId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesTrabajo_Sucursales_SucursalId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "CertificadosCilindros");

            migrationBuilder.DropTable(
                name: "PreciosRefrigerantes");

            migrationBuilder.DropTable(
                name: "Intervenciones");

            migrationBuilder.DropTable(
                name: "CertificadosDisposicion");

            migrationBuilder.DropTable(
                name: "CausasFalla");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Sucursales");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesTrabajo_AutorizadorId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesTrabajo_Origen_CodigoExterno",
                table: "OrdenesTrabajo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesTrabajo_SucursalId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesTrabajo_TecnicoAsignadoId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_MovimientosInventario_Id_CilindroId",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_CilindroId_FechaMovimiento",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_DespachoId",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_DespachoId_CilindroId",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_IdOperacion",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_UsuarioId",
                table: "MovimientosInventario");

            migrationBuilder.DropIndex(
                name: "UX_Movimientos_CustodiaAbierta",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movimientos_Custodia",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movimientos_Peso",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movimientos_Retorno",
                table: "MovimientosInventario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Gases_FactorGwp",
                table: "GasesRefrigerantes");

            migrationBuilder.DropIndex(
                name: "IX_Cilindros_NumeroSerie",
                table: "Cilindros");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RolId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AutorizadorId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "CodigoExterno",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "FechaCierre",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "TecnicoAsignadoId",
                table: "OrdenesTrabajo");

            migrationBuilder.DropColumn(
                name: "CustodiaAbierta",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "DespachoId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "FotoPesajeRuta",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "IdOperacion",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "FactorGwp",
                table: "GasesRefrigerantes");

            migrationBuilder.DropColumn(
                name: "FuenteGwp",
                table: "GasesRefrigerantes");

            migrationBuilder.DropColumn(
                name: "MetodologiaGwp",
                table: "GasesRefrigerantes");

            migrationBuilder.DropColumn(
                name: "FechaImportacion",
                table: "Cilindros");

            migrationBuilder.DropColumn(
                name: "NumeroSerie",
                table: "Cilindros");

            migrationBuilder.DropColumn(
                name: "RolId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "PesoKg",
                table: "MovimientosInventario",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaraKg",
                table: "Cilindros",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PesoActualKg",
                table: "Cilindros",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "CapacidadKg",
                table: "Cilindros",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_CilindroId",
                table: "MovimientosInventario",
                column: "CilindroId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.Sql("""
                INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
                    SELECT "UserId", "RoleId" FROM "__afrisan_roles_actuales";
                ALTER TABLE "AspNetUserRoles" ENABLE ROW LEVEL SECURITY;
                """);
        }
    }
}
