-- Módulos de soporte del contexto actual. Ejecutar después de 01_operacion.sql.
-- No inserta roles, usuarios, catálogos ni datos de ejemplo.
-- En el archivo canónico este módulo comparte la transacción de instalación.
BEGIN;

-- LÍMITES: el criterio de cómputo sigue requiriendo acuerdo de negocio.
-- Se guardan límites y mediciones declaradas, no un consumo/emisión inferidos.
CREATE TABLE afrisan_actual."LimiteSucursal" (
  "Id_Limite" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_Sucursal" integer NOT NULL REFERENCES afrisan_actual."Sucursales" ON DELETE RESTRICT,
  "Periodicidad" varchar(10) NOT NULL CHECK ("Periodicidad" IN ('Semana','Mes')),
  "EstadoDefinicion" varchar(15) NOT NULL DEFAULT 'Pendiente'
    CHECK ("EstadoDefinicion" IN ('Pendiente','Definido')),
  "GrLimite" bigint CHECK ("GrLimite" > 0),
  "CriterioComputo" text,
  "VigenteDesde" timestamptz NOT NULL DEFAULT now(),
  "BloqueadoBodega" boolean NOT NULL DEFAULT false,
  "Version" integer NOT NULL DEFAULT 1 CHECK ("Version" > 0),
  "Id_UltimoEditor" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaActualizacion" timestamptz NOT NULL DEFAULT now(),
  UNIQUE ("Id_Sucursal", "Periodicidad"),
  CHECK ("EstadoDefinicion" <> 'Definido' OR
    ("GrLimite" IS NOT NULL AND coalesce(btrim("CriterioComputo"),'') <> ''))
);

CREATE TABLE afrisan_actual."LimiteSucursalHistorial" (
  "Id_HistorialLimite" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_Limite" bigint NOT NULL REFERENCES afrisan_actual."LimiteSucursal" ON DELETE RESTRICT,
  "Version" integer NOT NULL,
  "Id_Sucursal" integer NOT NULL REFERENCES afrisan_actual."Sucursales" ON DELETE RESTRICT,
  "Periodicidad" varchar(10) NOT NULL,
  "EstadoDefinicion" varchar(15) NOT NULL,
  "GrLimite" bigint,
  "CriterioComputo" text,
  "VigenteDesde" timestamptz NOT NULL,
  "BloqueadoBodega" boolean NOT NULL,
  "Id_Actor" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRegistro" timestamptz NOT NULL,
  UNIQUE ("Id_Limite", "Version")
);

CREATE FUNCTION afrisan_actual.validar_limite_sucursal() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, afrisan_actual AS $$
DECLARE v_actor integer; v_rol text; v_cambio boolean;
BEGIN
  IF TG_OP = 'DELETE' THEN
    RAISE EXCEPTION 'El límite y su historial se conservan; no se eliminan';
  END IF;
  v_actor := afrisan_actual.actor(ARRAY['Bodega','Jefatura']);
  SELECT r."Nombre_Rol" INTO v_rol FROM afrisan_actual."Usuario" u
    JOIN afrisan_actual."Rol" r ON r."Id_Rol" = u."Id_Rol"
    WHERE u."Id_Usuario" = v_actor;
  IF TG_OP = 'INSERT' THEN
    NEW."Version" := 1;
    NEW."BloqueadoBodega" := v_rol = 'Jefatura';
  ELSE
    IF (NEW."Id_Limite",NEW."Id_Sucursal",NEW."Periodicidad") IS DISTINCT FROM
       (OLD."Id_Limite",OLD."Id_Sucursal",OLD."Periodicidad") THEN
      RAISE EXCEPTION 'No se cambia la identidad, sucursal ni periodicidad del límite';
    END IF;
    IF v_rol = 'Bodega' AND OLD."BloqueadoBodega" THEN
      RAISE EXCEPTION 'Jefatura mantiene bloqueada la edición por Bodega';
    END IF;
    IF v_rol = 'Bodega' AND NEW."BloqueadoBodega" IS DISTINCT FROM OLD."BloqueadoBodega" THEN
      RAISE EXCEPTION 'Solo Jefatura puede bloquear o habilitar la edición por Bodega';
    END IF;
    v_cambio := (NEW."EstadoDefinicion",NEW."GrLimite",NEW."CriterioComputo",NEW."VigenteDesde")
      IS DISTINCT FROM (OLD."EstadoDefinicion",OLD."GrLimite",OLD."CriterioComputo",OLD."VigenteDesde");
    -- Si Jefatura modifica el valor/criterio/vigencia, vuelve a bloquear.
    -- Para habilitar, cambia solo BloqueadoBodega a false en otra operación.
    IF v_rol = 'Jefatura' AND v_cambio THEN NEW."BloqueadoBodega" := true; END IF;
    NEW."Version" := OLD."Version" + 1;
  END IF;
  NEW."Id_UltimoEditor" := v_actor;
  NEW."FechaActualizacion" := clock_timestamp();
  RETURN NEW;
END $$;
CREATE TRIGGER "TR_Limite_Validar" BEFORE INSERT OR UPDATE OR DELETE
ON afrisan_actual."LimiteSucursal" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.validar_limite_sucursal();

CREATE FUNCTION afrisan_actual.historiar_limite_sucursal() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, afrisan_actual AS $$
BEGIN
  INSERT INTO afrisan_actual."LimiteSucursalHistorial"
    ("Id_Limite","Version","Id_Sucursal","Periodicidad","EstadoDefinicion","GrLimite",
     "CriterioComputo","VigenteDesde","BloqueadoBodega","Id_Actor","FechaRegistro")
  VALUES (NEW."Id_Limite",NEW."Version",NEW."Id_Sucursal",NEW."Periodicidad",NEW."EstadoDefinicion",
    NEW."GrLimite",NEW."CriterioComputo",NEW."VigenteDesde",NEW."BloqueadoBodega",
    NEW."Id_UltimoEditor",NEW."FechaActualizacion");
  RETURN NEW;
END $$;
CREATE TRIGGER "TR_Limite_Historial" AFTER INSERT OR UPDATE ON afrisan_actual."LimiteSucursal"
FOR EACH ROW EXECUTE FUNCTION afrisan_actual.historiar_limite_sucursal();

CREATE FUNCTION afrisan_actual.proteger_historial_limite() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, afrisan_actual AS $$
BEGIN
  IF TG_OP <> 'INSERT' OR pg_trigger_depth() < 2 THEN
    RAISE EXCEPTION 'El historial de límites solo se genera al registrar una versión del límite';
  END IF;
  RETURN NEW;
END $$;
CREATE TRIGGER "TR_LimiteHistorial_Inmutable" BEFORE INSERT OR UPDATE OR DELETE
ON afrisan_actual."LimiteSucursalHistorial" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.proteger_historial_limite();

CREATE TABLE afrisan_actual."MedicionLimite" (
  "Id_Medicion" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_HistorialLimite" bigint NOT NULL REFERENCES afrisan_actual."LimiteSucursalHistorial" ON DELETE RESTRICT,
  "PeriodoInicio" date NOT NULL,
  "PeriodoFin" date NOT NULL,
  "GrComputados" bigint NOT NULL CHECK ("GrComputados" >= 0),
  "MetodologiaComputo" text NOT NULL CHECK (btrim("MetodologiaComputo") <> ''),
  "Observaciones" text,
  "Id_Registrador" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRegistro" timestamptz NOT NULL DEFAULT now(),
  CHECK ("PeriodoFin" >= "PeriodoInicio")
);
COMMENT ON TABLE afrisan_actual."MedicionLimite" IS
  'Cómputo declarado con metodología explícita y límite histórico. No calcula automáticamente consumo ni el criterio de semana/mes; los períodos se indican expresamente hasta acordar calendario, masa y desglose.';
CREATE TABLE afrisan_actual."MedicionLimiteOperaciones" (
  "Id_Medicion" bigint NOT NULL REFERENCES afrisan_actual."MedicionLimite" ON DELETE RESTRICT,
  "Id_Operacion" uuid NOT NULL REFERENCES afrisan_actual."OperacionTerreno" ON DELETE RESTRICT,
  PRIMARY KEY ("Id_Medicion", "Id_Operacion")
);
CREATE INDEX "IX_MedicionLimite_Periodo" ON afrisan_actual."MedicionLimite" ("Id_HistorialLimite","PeriodoInicio","PeriodoFin");
CREATE INDEX "IX_MedicionLimite_Operacion" ON afrisan_actual."MedicionLimiteOperaciones" ("Id_Operacion");
CREATE TABLE afrisan_actual."RevisionExceso" (
  "Id_RevisionExceso" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_Medicion" bigint NOT NULL UNIQUE REFERENCES afrisan_actual."MedicionLimite" ON DELETE RESTRICT,
  "Conclusion" text NOT NULL CHECK (length(btrim("Conclusion")) >= 5),
  "Id_Revisor" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRevision" timestamptz NOT NULL DEFAULT now()
);
COMMENT ON TABLE afrisan_actual."RevisionExceso" IS
  'Conclusión expresa de Jefatura sobre un exceso declarado. Superar un límite no atribuye culpa ni demuestra una pérdida física.';

-- VALORACIÓN: cantidades y conceptos explícitos; no se derivan del reemplazo.
CREATE TABLE afrisan_actual."PreciosRefrigerantes" (
  "Id_Precio" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_Refrigerante" bigint NOT NULL REFERENCES afrisan_actual."Refrigerantes" ON DELETE RESTRICT,
  "PrecioGrRefrigerante" numeric(18,8) NOT NULL CHECK ("PrecioGrRefrigerante" >= 0),
  "Moneda" varchar(3) NOT NULL CHECK ("Moneda" ~ '^[A-Z]{3}$'),
  "VigenteDesde" timestamptz NOT NULL,
  "Id_Registrador" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRegistro" timestamptz NOT NULL DEFAULT now(),
  UNIQUE ("Id_Refrigerante","Moneda","VigenteDesde")
);
CREATE TABLE afrisan_actual."Costos" (
  "Id_Costos" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_Operacion" uuid NOT NULL REFERENCES afrisan_actual."OperacionTerreno" ON DELETE RESTRICT,
  "NumeroLinea" integer NOT NULL CHECK ("NumeroLinea" > 0),
  "Concepto" varchar(20) NOT NULL CHECK ("Concepto" IN ('Refrigerante','Envase','ManoObra','Repuesto','Traslado','Otro')),
  "Descripcion" text NOT NULL CHECK (btrim("Descripcion") <> ''),
  "Cantidad" numeric(18,6) NOT NULL CHECK ("Cantidad" > 0),
  "Unidad" varchar(40) NOT NULL CHECK (btrim("Unidad") <> ''),
  "TarifaUnitaria" numeric(18,8) NOT NULL CHECK ("TarifaUnitaria" >= 0),
  "Id_Precio" bigint REFERENCES afrisan_actual."PreciosRefrigerantes" ON DELETE RESTRICT,
  "Moneda" varchar(3) NOT NULL CHECK ("Moneda" ~ '^[A-Z]{3}$'),
  "PrecioTotal" numeric(30,8) GENERATED ALWAYS AS ("Cantidad" * "TarifaUnitaria") STORED,
  "Id_Registrador" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaValoracion" timestamptz NOT NULL DEFAULT now(),
  UNIQUE ("Id_Operacion","NumeroLinea"),
  CHECK ("Id_Precio" IS NULL OR ("Concepto" = 'Refrigerante' AND "Unidad" = 'g'))
);
COMMENT ON TABLE afrisan_actual."Costos" IS
  'Valoración por líneas de una operación física. Cantidad y tarifa son explícitas; instalar/reemplazar un cilindro no significa consumir ni facturar todo su contenido. La API determina la base de cobro acordada y conserva esta tarifa aplicada.';

CREATE TABLE afrisan_actual."EstimacionEmisiones" (
  "Id_Estimacion" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Id_Operacion" uuid NOT NULL UNIQUE REFERENCES afrisan_actual."OperacionTerreno" ON DELETE RESTRICT,
  "Id_Refrigerante" bigint NOT NULL REFERENCES afrisan_actual."Refrigerantes" ON DELETE RESTRICT,
  "GrEmitidos" bigint NOT NULL CHECK ("GrEmitidos" >= 0),
  "FactorGWPAplicado" numeric(18,6) NOT NULL CHECK ("FactorGWPAplicado" >= 0),
  "FuenteGWPAplicada" text NOT NULL CHECK (btrim("FuenteGWPAplicada") <> ''),
  "MetodologiaEstimacion" text NOT NULL CHECK (btrim("MetodologiaEstimacion") <> ''),
  "EmisionesCO2eTon" numeric(38,8) GENERATED ALWAYS AS
    ("GrEmitidos"::numeric * "FactorGWPAplicado" / 1000000) STORED,
  "Id_Registrador" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRegistro" timestamptz NOT NULL DEFAULT now()
);
COMMENT ON TABLE afrisan_actual."EstimacionEmisiones" IS
  'Estimación explícita con masa emitida, factor, fuente y metodología. No se genera a partir de kilos despachados/instalados. Ausencia de fila significa estimación desconocida, nunca cero automático.';

-- DISPOSICIÓN: lote entregado primero; certificado recibido después.
CREATE TABLE afrisan_actual."CertificadosDisposicion" (
  "Id_Certificado" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Estado" varchar(25) NOT NULL DEFAULT 'PendienteCertificado'
    CHECK ("Estado" IN ('PendienteCertificado','Certificado')),
  "EmpresaEmisora" varchar(150) NOT NULL CHECK (btrim("EmpresaEmisora") <> ''),
  "Folio" varchar(120),
  "FechaEntrega" date NOT NULL,
  "FechaEmision" date,
  "Documento" text,
  "Id_UsuarioRegistro" integer NOT NULL REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRegistro" timestamptz NOT NULL DEFAULT now(),
  "Id_UsuarioCertifica" integer REFERENCES afrisan_actual."Usuario" ON DELETE RESTRICT,
  "FechaRecepcionCertificado" timestamptz,
  UNIQUE ("EmpresaEmisora","Folio"),
  CHECK ("FechaEmision" IS NULL OR "FechaEmision" >= "FechaEntrega"),
  CHECK (("Estado" = 'Certificado') =
    (coalesce(btrim("Folio"),'') <> '' AND "FechaEmision" IS NOT NULL AND
     coalesce(btrim("Documento"),'') <> '' AND "Id_UsuarioCertifica" IS NOT NULL AND "FechaRecepcionCertificado" IS NOT NULL))
);
CREATE TABLE afrisan_actual."CertificadosCilindros" (
  "Id_Certificado" bigint NOT NULL REFERENCES afrisan_actual."CertificadosDisposicion" ON DELETE RESTRICT,
  "Id_Cilindro" bigint NOT NULL UNIQUE REFERENCES afrisan_actual."Cilindros" ON DELETE RESTRICT,
  PRIMARY KEY ("Id_Certificado","Id_Cilindro")
);
COMMENT ON TABLE afrisan_actual."CertificadosCilindros" IS
  'Cilindros en estado Chatarra de un lote pendiente de certificado. No exige intervenciones ni cargas de gas. El certificado y sus enlaces se conservan; no se asignan emisiones automáticamente.';

-- Estos módulos de soporte no amplían permisos operativos sin acuerdo.
-- Administrador registra tarifas/valoraciones/estimaciones/certificados por ahora.
-- La API podrá conceder las tareas a otros roles solo al definir esa política.
CREATE FUNCTION afrisan_actual.validar_soporte() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, afrisan_actual AS $$
DECLARE v_actor integer; v_limite afrisan_actual."LimiteSucursalHistorial"%ROWTYPE;
  v_medicion afrisan_actual."MedicionLimite"%ROWTYPE; v_precio afrisan_actual."PreciosRefrigerantes"%ROWTYPE;
  v_gas bigint; v_sucursal integer; v_cert afrisan_actual."CertificadosDisposicion"%ROWTYPE; v_estado text;
BEGIN
  IF TG_OP <> 'INSERT' THEN RAISE EXCEPTION 'Este registro histórico no se modifica ni elimina'; END IF;
  IF TG_TABLE_NAME = 'MedicionLimite' THEN
    v_actor := afrisan_actual.actor(ARRAY['Bodega','Autorizador','Jefatura']);
    SELECT * INTO STRICT v_limite FROM afrisan_actual."LimiteSucursalHistorial"
      WHERE "Id_HistorialLimite" = NEW."Id_HistorialLimite";
    IF v_limite."EstadoDefinicion" <> 'Definido' THEN RAISE EXCEPTION 'No se evalúa un límite cuyo criterio sigue pendiente'; END IF;
    NEW."Id_Registrador" := v_actor; NEW."FechaRegistro" := clock_timestamp();
  ELSIF TG_TABLE_NAME = 'MedicionLimiteOperaciones' THEN
    PERFORM afrisan_actual.actor(ARRAY['Bodega','Autorizador','Jefatura']);
    SELECT * INTO STRICT v_medicion FROM afrisan_actual."MedicionLimite" WHERE "Id_Medicion" = NEW."Id_Medicion";
    SELECT "Id_Sucursal" INTO STRICT v_sucursal FROM afrisan_actual."LimiteSucursalHistorial"
      WHERE "Id_HistorialLimite" = v_medicion."Id_HistorialLimite";
    IF NOT EXISTS (SELECT 1 FROM afrisan_actual."OperacionTerreno" op
      JOIN afrisan_actual."OrdenTrabajo" ot ON ot."Id_OrdenTrabajo" = op."Id_OrdenTrabajo"
      WHERE op."Id_Operacion" = NEW."Id_Operacion" AND op."Estado" = 'Registrada' AND ot."Id_Sucursal" = v_sucursal) THEN
      RAISE EXCEPTION 'La operación debe estar registrada y pertenecer a la sucursal del cómputo';
    END IF;
  ELSIF TG_TABLE_NAME = 'RevisionExceso' THEN
    v_actor := afrisan_actual.actor(ARRAY['Jefatura']);
    SELECT * INTO STRICT v_medicion FROM afrisan_actual."MedicionLimite" WHERE "Id_Medicion" = NEW."Id_Medicion";
    SELECT * INTO STRICT v_limite FROM afrisan_actual."LimiteSucursalHistorial"
      WHERE "Id_HistorialLimite" = v_medicion."Id_HistorialLimite";
    IF v_medicion."GrComputados" <= v_limite."GrLimite" THEN RAISE EXCEPTION 'La medición no registra un exceso sobre el límite aplicado'; END IF;
    NEW."Id_Revisor" := v_actor; NEW."FechaRevision" := clock_timestamp();
  ELSIF TG_TABLE_NAME = 'PreciosRefrigerantes' THEN
    NEW."Id_Registrador" := afrisan_actual.actor(ARRAY['Administrador']); NEW."FechaRegistro" := clock_timestamp();
  ELSIF TG_TABLE_NAME IN ('Costos','EstimacionEmisiones') THEN
    v_actor := afrisan_actual.actor(ARRAY['Administrador']);
    SELECT c."Id_Refrigerante" INTO v_gas FROM afrisan_actual."OperacionTerreno" op
      JOIN afrisan_actual."Cilindros" c ON c."Id_Cilindro" = op."Id_Instalado"
      WHERE op."Id_Operacion" = NEW."Id_Operacion" AND op."Estado" = 'Registrada';
    IF v_gas IS NULL THEN RAISE EXCEPTION 'Solo se valora o estima una operación física registrada'; END IF;
    NEW."Id_Registrador" := v_actor;
    IF TG_TABLE_NAME = 'Costos' THEN
      NEW."FechaValoracion" := clock_timestamp();
      IF NEW."Id_Precio" IS NOT NULL THEN
        SELECT * INTO STRICT v_precio FROM afrisan_actual."PreciosRefrigerantes" WHERE "Id_Precio" = NEW."Id_Precio";
        IF v_precio."Id_Refrigerante" <> v_gas OR NEW."TarifaUnitaria" <> v_precio."PrecioGrRefrigerante"
          OR NEW."Moneda" <> v_precio."Moneda" THEN RAISE EXCEPTION 'La tarifa aplicada no coincide con el refrigerante, valor o moneda del precio referenciado'; END IF;
      END IF;
    ELSE
      IF NEW."Id_Refrigerante" <> v_gas THEN RAISE EXCEPTION 'El refrigerante estimado no coincide con el de la operación'; END IF;
      NEW."FechaRegistro" := clock_timestamp();
    END IF;
  ELSIF TG_TABLE_NAME = 'CertificadosCilindros' THEN
    PERFORM afrisan_actual.actor(ARRAY['Administrador']);
    SELECT * INTO STRICT v_cert FROM afrisan_actual."CertificadosDisposicion" WHERE "Id_Certificado" = NEW."Id_Certificado" FOR UPDATE;
    IF v_cert."Estado" <> 'PendienteCertificado' THEN RAISE EXCEPTION 'El lote certificado ya está cerrado'; END IF;
    SELECT "Estado" INTO STRICT v_estado FROM afrisan_actual."Cilindros" WHERE "Id_Cilindro" = NEW."Id_Cilindro" FOR UPDATE;
    IF v_estado <> 'Chatarra' THEN RAISE EXCEPTION 'Solo se incluye un cilindro clasificado como Chatarra'; END IF;
  ELSE RAISE EXCEPTION 'Tabla de soporte no reconocida';
  END IF;
  RETURN NEW;
END $$;

CREATE FUNCTION afrisan_actual.validar_certificado() RETURNS trigger
LANGUAGE plpgsql SET search_path = pg_catalog, afrisan_actual AS $$
DECLARE v_actor integer;
BEGIN
  IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'No se elimina el historial de disposición'; END IF;
  v_actor := afrisan_actual.actor(ARRAY['Administrador']);
  IF TG_OP = 'INSERT' THEN
    IF NEW."Estado" <> 'PendienteCertificado' THEN RAISE EXCEPTION 'Registra primero la entrega del lote y después su certificado'; END IF;
    NEW."Id_UsuarioRegistro" := v_actor; NEW."FechaRegistro" := clock_timestamp();
    NEW."Id_UsuarioCertifica" := NULL; NEW."FechaRecepcionCertificado" := NULL;
  ELSE
    IF OLD."Estado" <> 'PendienteCertificado' OR NEW."Estado" <> 'Certificado' THEN
      RAISE EXCEPTION 'Solo se permite completar un certificado pendiente; el documento confirmado es inmutable';
    END IF;
    IF (NEW."Id_Certificado",NEW."EmpresaEmisora",NEW."FechaEntrega",NEW."Id_UsuarioRegistro",NEW."FechaRegistro")
      IS DISTINCT FROM (OLD."Id_Certificado",OLD."EmpresaEmisora",OLD."FechaEntrega",OLD."Id_UsuarioRegistro",OLD."FechaRegistro") THEN
      RAISE EXCEPTION 'No se reemplazan los datos históricos de entrega del lote';
    END IF;
    IF NOT EXISTS (SELECT 1 FROM afrisan_actual."CertificadosCilindros" WHERE "Id_Certificado" = OLD."Id_Certificado") THEN
      RAISE EXCEPTION 'El certificado debe identificar al menos un cilindro entregado';
    END IF;
    PERFORM c."Id_Cilindro" FROM afrisan_actual."Cilindros" c
      JOIN afrisan_actual."CertificadosCilindros" l ON l."Id_Cilindro" = c."Id_Cilindro"
      WHERE l."Id_Certificado" = OLD."Id_Certificado" FOR UPDATE OF c;
    IF EXISTS (SELECT 1 FROM afrisan_actual."Cilindros" c
      JOIN afrisan_actual."CertificadosCilindros" l ON l."Id_Cilindro" = c."Id_Cilindro"
      WHERE l."Id_Certificado" = OLD."Id_Certificado" AND c."Estado" <> 'Chatarra') THEN
      RAISE EXCEPTION 'El lote ya no corresponde a cilindros clasificados como Chatarra';
    END IF;
    NEW."Id_UsuarioCertifica" := v_actor; NEW."FechaRecepcionCertificado" := clock_timestamp();
  END IF;
  RETURN NEW;
END $$;
CREATE TRIGGER "TR_Certificado_Validar" BEFORE INSERT OR UPDATE OR DELETE
ON afrisan_actual."CertificadosDisposicion" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.validar_certificado();

DO $$ DECLARE t text; BEGIN
  FOREACH t IN ARRAY ARRAY['MedicionLimite','MedicionLimiteOperaciones','RevisionExceso',
    'PreciosRefrigerantes','Costos','EstimacionEmisiones','CertificadosCilindros'] LOOP
    EXECUTE format('CREATE TRIGGER %I BEFORE INSERT OR UPDATE OR DELETE ON afrisan_actual.%I FOR EACH ROW EXECUTE FUNCTION afrisan_actual.validar_soporte()', 'TR_' || t || '_Validar', t);
  END LOOP;
  FOREACH t IN ARRAY ARRAY['LimiteSucursal','LimiteSucursalHistorial','MedicionLimite','MedicionLimiteOperaciones',
    'RevisionExceso','PreciosRefrigerantes','Costos','EstimacionEmisiones','CertificadosDisposicion','CertificadosCilindros'] LOOP
    EXECUTE format('ALTER TABLE afrisan_actual.%I ENABLE ROW LEVEL SECURITY', t);
    EXECUTE format('REVOKE ALL ON afrisan_actual.%I FROM PUBLIC', t);
  END LOOP;
END $$;

-- Vistas para un lector BI de servidor confiable, con acceso expresamente otorgado
-- por el administrador. No crear GRANT a anon/authenticated ni publicar Data API.
CREATE VIEW afrisan_bi_actual."LimitesVigentes" AS
SELECT DISTINCT ON ("Id_Limite") "Id_Limite","Id_Sucursal","Periodicidad","EstadoDefinicion","GrLimite","CriterioComputo",
  "VigenteDesde","BloqueadoBodega","Version","FechaRegistro" AS "FechaActualizacion"
FROM afrisan_actual."LimiteSucursalHistorial"
WHERE "VigenteDesde" <= current_timestamp
ORDER BY "Id_Limite","VigenteDesde" DESC,"Version" DESC;
CREATE VIEW afrisan_bi_actual."ExcesosRegistrados" AS
SELECT m."Id_Medicion",h."Id_Sucursal",h."Periodicidad",m."PeriodoInicio",m."PeriodoFin",
  h."Version" AS "VersionLimiteAplicada",h."GrLimite",m."GrComputados",
  greatest(m."GrComputados" - h."GrLimite",0) AS "GrExcesoDeclarado",
  h."CriterioComputo",m."MetodologiaComputo",m."FechaRegistro",
  r."Conclusion",r."FechaRevision"
FROM afrisan_actual."MedicionLimite" m
JOIN afrisan_actual."LimiteSucursalHistorial" h ON h."Id_HistorialLimite" = m."Id_HistorialLimite"
LEFT JOIN afrisan_actual."RevisionExceso" r ON r."Id_Medicion" = m."Id_Medicion";
CREATE VIEW afrisan_bi_actual."CostosOperaciones" AS
SELECT c."Id_Costos",c."Id_Operacion",op."Id_OrdenTrabajo",ot."Id_Sucursal",c."NumeroLinea",c."Concepto",
  c."Descripcion",c."Cantidad",c."Unidad",c."TarifaUnitaria",c."Moneda",c."PrecioTotal",c."FechaValoracion"
FROM afrisan_actual."Costos" c JOIN afrisan_actual."OperacionTerreno" op ON op."Id_Operacion" = c."Id_Operacion"
JOIN afrisan_actual."OrdenTrabajo" ot ON ot."Id_OrdenTrabajo" = op."Id_OrdenTrabajo";
CREATE VIEW afrisan_bi_actual."EmisionesEstimadas" AS
SELECT op."Id_Operacion",op."Id_OrdenTrabajo",ot."Id_Sucursal",e."Id_Refrigerante",e."GrEmitidos",
  e."FactorGWPAplicado",e."FuenteGWPAplicada",e."MetodologiaEstimacion",e."EmisionesCO2eTon",
  e."FechaRegistro",(e."Id_Estimacion" IS NOT NULL) AS "TieneEstimacion"
FROM afrisan_actual."OperacionTerreno" op JOIN afrisan_actual."OrdenTrabajo" ot ON ot."Id_OrdenTrabajo" = op."Id_OrdenTrabajo"
LEFT JOIN afrisan_actual."EstimacionEmisiones" e ON e."Id_Operacion" = op."Id_Operacion"
WHERE op."Estado" = 'Registrada';
CREATE VIEW afrisan_bi_actual."DisposicionesCertificadas" AS
SELECT d."Id_Certificado",d."Estado",d."EmpresaEmisora",d."Folio",d."FechaEntrega",d."FechaEmision",c."Id_Cilindro"
FROM afrisan_actual."CertificadosDisposicion" d
JOIN afrisan_actual."CertificadosCilindros" c ON c."Id_Certificado" = d."Id_Certificado";

REVOKE ALL ON ALL TABLES IN SCHEMA afrisan_bi_actual FROM PUBLIC;
REVOKE EXECUTE ON FUNCTION afrisan_actual.validar_limite_sucursal(),
  afrisan_actual.historiar_limite_sucursal(), afrisan_actual.proteger_historial_limite(),
  afrisan_actual.validar_soporte(), afrisan_actual.validar_certificado() FROM PUBLIC;
COMMIT;
