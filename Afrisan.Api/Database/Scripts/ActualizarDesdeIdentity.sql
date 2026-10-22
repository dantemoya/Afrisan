-- Solo para el esquema EF que termina en AgregarIdentityUsuariosRoles.
-- No ejecutar directamente sobre las tablas manuales de las capturas.
DO $guard$
BEGIN
    IF to_regclass('"__EFMigrationsHistory"') IS NULL THEN
        RAISE EXCEPTION 'Esta actualización requiere una base administrada por EF.';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261007175616_AgregarIdentityUsuariosRoles') THEN
        RAISE EXCEPTION 'Primero se requiere la migración AgregarIdentityUsuariosRoles.';
    END IF;
END;
$guard$;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    DROP TABLE "AspNetUserRoles";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    DROP INDEX "IX_MovimientosInventario_CilindroId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD "AutorizadorId" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD "CodigoExterno" character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD "FechaCierre" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD "Origen" character varying(50) NOT NULL DEFAULT 'Local';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD "SucursalId" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD "TecnicoAsignadoId" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ALTER COLUMN "PesoKg" TYPE numeric(18,3);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD "CustodiaAbierta" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD "DespachoId" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD "FotoPesajeRuta" character varying(1024);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD "IdOperacion" uuid NOT NULL DEFAULT (gen_random_uuid());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD "UsuarioId" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "GasesRefrigerantes" ADD "FactorGwp" numeric(12,3);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "GasesRefrigerantes" ADD "FuenteGwp" character varying(300);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "GasesRefrigerantes" ADD "MetodologiaGwp" character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "Cilindros" ALTER COLUMN "TaraKg" TYPE numeric(18,3);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "Cilindros" ALTER COLUMN "PesoActualKg" TYPE numeric(18,3);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "Cilindros" ALTER COLUMN "CapacidadKg" TYPE numeric(18,3);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "Cilindros" ADD "FechaImportacion" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "Cilindros" ADD "NumeroSerie" character varying(100);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "AspNetUsers" ADD "RolId" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "AK_MovimientosInventario_Id_CilindroId" UNIQUE ("Id", "CilindroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "CausasFalla" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "Codigo" character varying(50) NOT NULL,
        "Nombre" character varying(200) NOT NULL,
        "Activa" boolean NOT NULL,
        CONSTRAINT "PK_CausasFalla" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_CausasFalla_Codigo" CHECK (length(btrim("Codigo")) > 0)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "CertificadosDisposicion" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "Folio" character varying(100) NOT NULL,
        "EmpresaEmisora" character varying(200) NOT NULL,
        "FechaEmision" date NOT NULL,
        "RutaDocumento" character varying(1024) NOT NULL,
        "UsuarioId" text NOT NULL,
        "FechaRegistro" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_CertificadosDisposicion" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_Certificados_Folio" CHECK (length(btrim("Folio")) > 0),
        CONSTRAINT "FK_CertificadosDisposicion_AspNetUsers_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "Clientes" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "Nombre" character varying(200) NOT NULL,
        "Rut" character varying(20),
        "Telefono" character varying(40),
        "Activo" boolean NOT NULL,
        CONSTRAINT "PK_Clientes" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_Clientes_Nombre" CHECK (length(btrim("Nombre")) > 0)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "PreciosRefrigerantes" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "GasRefrigeranteId" integer NOT NULL,
        "VigenteDesde" timestamp with time zone NOT NULL,
        "PrecioKg" numeric(18,4) NOT NULL,
        "Moneda" character varying(3) NOT NULL,
        CONSTRAINT "PK_PreciosRefrigerantes" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_Precios_Importe" CHECK ("PrecioKg" >= 0),
        CONSTRAINT "CK_Precios_Moneda" CHECK ("Moneda" ~ '^[A-Z]{3}$'),
        CONSTRAINT "FK_PreciosRefrigerantes_GasesRefrigerantes_GasRefrigeranteId" FOREIGN KEY ("GasRefrigeranteId") REFERENCES "GasesRefrigerantes" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "CertificadosCilindros" (
        "CertificadoDisposicionId" integer NOT NULL,
        "CilindroId" integer NOT NULL,
        "PesoEntregadoKg" numeric(18,3),
        CONSTRAINT "PK_CertificadosCilindros" PRIMARY KEY ("CertificadoDisposicionId", "CilindroId"),
        CONSTRAINT "CK_CertificadosCilindros_Peso" CHECK ("PesoEntregadoKg" IS NULL OR "PesoEntregadoKg" >= 0),
        CONSTRAINT "FK_CertificadosCilindros_CertificadosDisposicion_CertificadoDi~" FOREIGN KEY ("CertificadoDisposicionId") REFERENCES "CertificadosDisposicion" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_CertificadosCilindros_Cilindros_CilindroId" FOREIGN KEY ("CilindroId") REFERENCES "Cilindros" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "Sucursales" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "ClienteId" integer NOT NULL,
        "Nombre" character varying(200) NOT NULL,
        "Direccion" character varying(400) NOT NULL,
        "Activo" boolean NOT NULL,
        CONSTRAINT "PK_Sucursales" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_Sucursales_Nombre" CHECK (length(btrim("Nombre")) > 0),
        CONSTRAINT "FK_Sucursales_Clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES "Clientes" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "Equipos" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "SucursalId" integer NOT NULL,
        "Codigo" character varying(80) NOT NULL,
        "Nombre" character varying(200) NOT NULL,
        "Tipo" character varying(100) NOT NULL,
        "CapacidadNominalKg" numeric(18,3) NOT NULL,
        "GasRefrigeranteId" integer,
        "Activo" boolean NOT NULL,
        CONSTRAINT "PK_Equipos" PRIMARY KEY ("Id"),
        CONSTRAINT "AK_Equipos_Id_SucursalId" UNIQUE ("Id", "SucursalId"),
        CONSTRAINT "CK_Equipos_Capacidad" CHECK ("CapacidadNominalKg" > 0),
        CONSTRAINT "CK_Equipos_Codigo" CHECK (length(btrim("Codigo")) > 0),
        CONSTRAINT "FK_Equipos_GasesRefrigerantes_GasRefrigeranteId" FOREIGN KEY ("GasRefrigeranteId") REFERENCES "GasesRefrigerantes" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Equipos_Sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES "Sucursales" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "Intervenciones" (
        "Id" integer GENERATED BY DEFAULT AS IDENTITY,
        "IdOperacion" uuid NOT NULL DEFAULT (gen_random_uuid()),
        "OrdenTrabajoId" integer NOT NULL,
        "CilindroId" integer NOT NULL,
        "DespachoId" integer NOT NULL,
        "SucursalId" integer NOT NULL,
        "EquipoId" integer NOT NULL,
        "CausaFallaId" integer NOT NULL,
        "TecnicoId" text NOT NULL,
        "AutorizadorId" text,
        "FechaIntervencion" timestamp with time zone NOT NULL,
        "KgUtilizados" numeric(18,3) NOT NULL,
        "PesoRestanteBrutoKg" numeric(18,3),
        "FotoBalanzaRuta" character varying(1024),
        "EstadoIntervencion" character varying(20) NOT NULL,
        "PlanCorreccion" character varying(2000),
        "EstadoSemaforo" character varying(20) NOT NULL,
        "FactorGwpAplicado" numeric(12,3),
        "FuenteGwpAplicada" character varying(300),
        "MetodologiaGwpAplicada" character varying(100),
        "PrecioKgAplicado" numeric(18,4),
        "Moneda" character varying(3),
        "KgEmitidos" numeric(18,3),
        "EmisionesCo2eToneladas" numeric(20,6) GENERATED ALWAYS AS ("KgEmitidos" * "FactorGwpAplicado" / 1000.0) STORED,
        "CostoTotal" numeric(20,4) GENERATED ALWAYS AS ("KgUtilizados" * "PrecioKgAplicado") STORED,
        "EstadoSincronizacion" character varying(20) NOT NULL,
        "DispositivoId" character varying(100),
        "FechaCreacionLocal" timestamp with time zone,
        "FechaRecepcion" timestamp with time zone NOT NULL,
        "FechaCierre" timestamp with time zone,
        "Observaciones" character varying(2000),
        CONSTRAINT "PK_Intervenciones" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_Intervenciones_Estado" CHECK ("EstadoIntervencion" IN ('Registrada', 'Revisada', 'Cerrada')),
        CONSTRAINT "CK_Intervenciones_Factores" CHECK (("FactorGwpAplicado" IS NULL OR "FactorGwpAplicado" >= 0) AND ("PrecioKgAplicado" IS NULL OR "PrecioKgAplicado" >= 0)),
        CONSTRAINT "CK_Intervenciones_FotoCierre" CHECK ("EstadoIntervencion" <> 'Cerrada' OR ("FechaCierre" IS NOT NULL AND length(btrim(coalesce("FotoBalanzaRuta", ''))) > 0 AND "PesoRestanteBrutoKg" IS NOT NULL)),
        CONSTRAINT "CK_Intervenciones_Moneda" CHECK ("PrecioKgAplicado" IS NULL OR ("Moneda" IS NOT NULL AND "Moneda" ~ '^[A-Z]{3}$')),
        CONSTRAINT "CK_Intervenciones_Pesos" CHECK ("KgUtilizados" > 0 AND ("KgEmitidos" IS NULL OR "KgEmitidos" >= 0) AND ("PesoRestanteBrutoKg" IS NULL OR "PesoRestanteBrutoKg" >= 0)),
        CONSTRAINT "CK_Intervenciones_PlanRojo" CHECK ("EstadoSemaforo" <> 'Rojo' OR length(btrim(coalesce("PlanCorreccion", ''))) > 0),
        CONSTRAINT "CK_Intervenciones_Semaforo" CHECK ("EstadoSemaforo" IN ('Pendiente', 'Verde', 'Amarillo', 'Rojo')),
        CONSTRAINT "CK_Intervenciones_Sync" CHECK ("EstadoSincronizacion" IN ('Pendiente', 'Sincronizado', 'Conflicto')),
        CONSTRAINT "FK_Intervenciones_AspNetUsers_AutorizadorId" FOREIGN KEY ("AutorizadorId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_AspNetUsers_TecnicoId" FOREIGN KEY ("TecnicoId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_CausasFalla_CausaFallaId" FOREIGN KEY ("CausaFallaId") REFERENCES "CausasFalla" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_Cilindros_CilindroId" FOREIGN KEY ("CilindroId") REFERENCES "Cilindros" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_Equipos_EquipoId_SucursalId" FOREIGN KEY ("EquipoId", "SucursalId") REFERENCES "Equipos" ("Id", "SucursalId") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_MovimientosInventario_DespachoId_CilindroId" FOREIGN KEY ("DespachoId", "CilindroId") REFERENCES "MovimientosInventario" ("Id", "CilindroId") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_OrdenesTrabajo_OrdenTrabajoId" FOREIGN KEY ("OrdenTrabajoId") REFERENCES "OrdenesTrabajo" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Intervenciones_Sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES "Sucursales" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE TABLE "Auditorias" (
        "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
        "UsuarioId" text NOT NULL,
        "IntervencionId" integer,
        "Accion" character varying(50) NOT NULL,
        "Entidad" character varying(100) NOT NULL,
        "RegistroId" character varying(100) NOT NULL,
        "ValoresAnteriores" jsonb,
        "ValoresNuevos" jsonb,
        "Fecha" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Auditorias" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Auditorias_AspNetUsers_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Auditorias_Intervenciones_IntervencionId" FOREIGN KEY ("IntervencionId") REFERENCES "Intervenciones" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_OrdenesTrabajo_AutorizadorId" ON "OrdenesTrabajo" ("AutorizadorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_OrdenesTrabajo_Origen_CodigoExterno" ON "OrdenesTrabajo" ("Origen", "CodigoExterno") WHERE "CodigoExterno" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_OrdenesTrabajo_SucursalId" ON "OrdenesTrabajo" ("SucursalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_OrdenesTrabajo_TecnicoAsignadoId" ON "OrdenesTrabajo" ("TecnicoAsignadoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_MovimientosInventario_CilindroId_FechaMovimiento" ON "MovimientosInventario" ("CilindroId", "FechaMovimiento");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_MovimientosInventario_DespachoId" ON "MovimientosInventario" ("DespachoId") WHERE "DespachoId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_MovimientosInventario_DespachoId_CilindroId" ON "MovimientosInventario" ("DespachoId", "CilindroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_MovimientosInventario_IdOperacion" ON "MovimientosInventario" ("IdOperacion");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_MovimientosInventario_UsuarioId" ON "MovimientosInventario" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "UX_Movimientos_CustodiaAbierta" ON "MovimientosInventario" ("CilindroId") WHERE "CustodiaAbierta";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "CK_Movimientos_Custodia" CHECK (NOT "CustodiaAbierta" OR "TipoMovimiento" = 'Despacho');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "CK_Movimientos_Peso" CHECK ("PesoKg" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "CK_Movimientos_Retorno" CHECK ("DespachoId" IS NULL OR "TipoMovimiento" = 'Retorno');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "GasesRefrigerantes" ADD CONSTRAINT "CK_Gases_FactorGwp" CHECK ("FactorGwp" IS NULL OR "FactorGwp" >= 0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_Cilindros_NumeroSerie" ON "Cilindros" ("NumeroSerie") WHERE "NumeroSerie" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_AspNetUsers_RolId" ON "AspNetUsers" ("RolId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Auditorias_Entidad_RegistroId" ON "Auditorias" ("Entidad", "RegistroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Auditorias_Fecha" ON "Auditorias" ("Fecha");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Auditorias_IntervencionId" ON "Auditorias" ("IntervencionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Auditorias_UsuarioId" ON "Auditorias" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_CausasFalla_Codigo" ON "CausasFalla" ("Codigo");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_CertificadosCilindros_CilindroId" ON "CertificadosCilindros" ("CilindroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_CertificadosDisposicion_EmpresaEmisora_Folio" ON "CertificadosDisposicion" ("EmpresaEmisora", "Folio");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_CertificadosDisposicion_UsuarioId" ON "CertificadosDisposicion" ("UsuarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_Clientes_Rut" ON "Clientes" ("Rut") WHERE "Rut" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Equipos_GasRefrigeranteId" ON "Equipos" ("GasRefrigeranteId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_Equipos_SucursalId_Codigo" ON "Equipos" ("SucursalId", "Codigo");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_AutorizadorId" ON "Intervenciones" ("AutorizadorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_CausaFallaId" ON "Intervenciones" ("CausaFallaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_CilindroId" ON "Intervenciones" ("CilindroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_DespachoId_CilindroId" ON "Intervenciones" ("DespachoId", "CilindroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_EquipoId_FechaIntervencion" ON "Intervenciones" ("EquipoId", "FechaIntervencion");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_EquipoId_SucursalId" ON "Intervenciones" ("EquipoId", "SucursalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_Intervenciones_IdOperacion" ON "Intervenciones" ("IdOperacion");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_OrdenTrabajoId_CilindroId" ON "Intervenciones" ("OrdenTrabajoId", "CilindroId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_SucursalId" ON "Intervenciones" ("SucursalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Intervenciones_TecnicoId" ON "Intervenciones" ("TecnicoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE UNIQUE INDEX "IX_PreciosRefrigerantes_GasRefrigeranteId_Moneda_VigenteDesde" ON "PreciosRefrigerantes" ("GasRefrigeranteId", "Moneda", "VigenteDesde");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    CREATE INDEX "IX_Sucursales_ClienteId_Nombre" ON "Sucursales" ("ClienteId", "Nombre");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "AspNetUsers" ADD CONSTRAINT "FK_AspNetUsers_AspNetRoles_RolId" FOREIGN KEY ("RolId") REFERENCES "AspNetRoles" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "FK_MovimientosInventario_AspNetUsers_UsuarioId" FOREIGN KEY ("UsuarioId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "MovimientosInventario" ADD CONSTRAINT "FK_MovimientosInventario_MovimientosInventario_DespachoId_Cili~" FOREIGN KEY ("DespachoId", "CilindroId") REFERENCES "MovimientosInventario" ("Id", "CilindroId") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD CONSTRAINT "FK_OrdenesTrabajo_AspNetUsers_AutorizadorId" FOREIGN KEY ("AutorizadorId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD CONSTRAINT "FK_OrdenesTrabajo_AspNetUsers_TecnicoAsignadoId" FOREIGN KEY ("TecnicoAsignadoId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    ALTER TABLE "OrdenesTrabajo" ADD CONSTRAINT "FK_OrdenesTrabajo_Sucursales_SucursalId" FOREIGN KEY ("SucursalId") REFERENCES "Sucursales" ("Id") ON DELETE RESTRICT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261019031737_AlinearMerRolesSucursalesIntervenciones') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20261019031737_AlinearMerRolesSucursalesIntervenciones', '8.0.0');
    END IF;
END $EF$;
COMMIT;

