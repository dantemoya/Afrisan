BEGIN;
CREATE TABLE afrisan_actual."Auditoria" (
  "Id_Auditoria" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  "Fecha" timestamptz NOT NULL DEFAULT clock_timestamp(), "Id_Usuario" integer REFERENCES afrisan_actual."Usuario",
  "UsuarioBD" text NOT NULL DEFAULT session_user, "Tabla" text NOT NULL,
  "Accion" varchar(10) NOT NULL CHECK ("Accion" IN ('INSERT','UPDATE','DELETE')),
  "ValoresAnteriores" jsonb,"ValoresNuevos" jsonb
);
CREATE FUNCTION afrisan_actual.auditar() RETURNS trigger LANGUAGE plpgsql
SET search_path=pg_catalog,afrisan_actual AS $$
DECLARE anterior jsonb; nuevo jsonb;
BEGIN
  IF TG_OP<>'INSERT' THEN anterior:=to_jsonb(OLD); END IF;
  IF TG_OP<>'DELETE' THEN nuevo:=to_jsonb(NEW); END IF;
  IF TG_TABLE_NAME='Usuario' THEN
    anterior:=anterior-ARRAY['Clave_Usuario','VersionSeguridad']; nuevo:=nuevo-ARRAY['Clave_Usuario','VersionSeguridad'];
  END IF;
  INSERT INTO afrisan_actual."Auditoria"("Id_Usuario","Tabla","Accion","ValoresAnteriores","ValoresNuevos")
  VALUES(nullif(current_setting('afrisan_actual.id_usuario',true),'')::integer,TG_TABLE_NAME,TG_OP,anterior,nuevo);
  RETURN NULL;
END $$;
CREATE TRIGGER "Auditoria_Inmutable" BEFORE UPDATE OR DELETE ON afrisan_actual."Auditoria" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.inmutable();

-- Baja administrativa con motivo. No supone que exista certificado todavía.
-- Es una herramienta de administración; el responsable de negocio debe ratificarse.
CREATE TABLE afrisan_actual."BajaCilindro" (
  "Id_Baja" uuid PRIMARY KEY DEFAULT gen_random_uuid(),"Id_Cilindro" bigint NOT NULL UNIQUE REFERENCES afrisan_actual."Cilindros",
  "Motivo" text NOT NULL CHECK(length(btrim("Motivo"))>=5),
  "EstadoDestino" varchar(20) NOT NULL CHECK("EstadoDestino" IN ('Chatarra','Inactivo')),
  "Id_Usuario" integer NOT NULL REFERENCES afrisan_actual."Usuario","Fecha" timestamptz NOT NULL DEFAULT clock_timestamp()
);
CREATE FUNCTION afrisan_actual.baja_validar() RETURNS trigger LANGUAGE plpgsql
SET search_path=pg_catalog,afrisan_actual AS $$
DECLARE c afrisan_actual."Cilindros";
BEGIN
  NEW."Id_Usuario":=afrisan_actual.actor(ARRAY['Administrador']);
  SELECT * INTO STRICT c FROM afrisan_actual."Cilindros" WHERE "Id_Cilindro"=NEW."Id_Cilindro" FOR UPDATE;
  IF c."Ubicacion"<>'Bodega' OR EXISTS(SELECT 1 FROM afrisan_actual."CustodiaActual" WHERE "Id_Cilindro"=c."Id_Cilindro") OR EXISTS(SELECT 1 FROM afrisan_actual."RetiroPrevisto" WHERE "Id_Cilindro"=c."Id_Cilindro" AND "Estado"='Reservado') THEN RAISE EXCEPTION 'No retirar de servicio un envase instalado, reservado o en custodia'; END IF;
  NEW."Fecha":=clock_timestamp(); RETURN NEW;
END $$;
CREATE TRIGGER "Baja_Validar" BEFORE INSERT ON afrisan_actual."BajaCilindro" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.baja_validar();
CREATE TRIGGER "Baja_Inmutable" BEFORE UPDATE OR DELETE ON afrisan_actual."BajaCilindro" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.inmutable();
CREATE FUNCTION afrisan_actual.baja_aplicar() RETURNS trigger LANGUAGE plpgsql
SET search_path=pg_catalog,afrisan_actual AS $$
BEGIN
  UPDATE afrisan_actual."Cilindros" SET "Ubicacion"='FueraServicio',"Estado"=NEW."EstadoDestino","Id_Sucursal"=NULL,"Id_Tecnico"=NULL WHERE "Id_Cilindro"=NEW."Id_Cilindro";
  RETURN NULL;
END $$;
CREATE TRIGGER "Baja_Aplicar" AFTER INSERT ON afrisan_actual."BajaCilindro" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.baja_aplicar();
CREATE FUNCTION afrisan_actual.certificado_aplicar() RETURNS trigger LANGUAGE plpgsql
SET search_path=pg_catalog,afrisan_actual AS $$
BEGIN
  IF NEW."Estado"='Certificado' AND OLD."Estado"='PendienteCertificado' THEN
    UPDATE afrisan_actual."Cilindros" c SET "Estado"='DisposicionFinalCertificada'
    FROM afrisan_actual."CertificadosCilindros" l WHERE l."Id_Certificado"=NEW."Id_Certificado" AND l."Id_Cilindro"=c."Id_Cilindro";
  END IF; RETURN NULL;
END $$;
CREATE TRIGGER "Certificado_Aplicar" AFTER UPDATE ON afrisan_actual."CertificadosDisposicion" FOR EACH ROW EXECUTE FUNCTION afrisan_actual.certificado_aplicar();

CREATE VIEW afrisan_bi_actual."PedidoYEjecucion" AS
SELECT d."Id_OrdenTrabajo",o."Id_Sucursal",d."Id_Refrigerante",d."GrSolicitados",d."GrAutorizados",
 (SELECT sum(p."PesoBrutoGr"-c."TaraGr") FROM afrisan_actual."Despacho" e JOIN afrisan_actual."Cilindros" c USING("Id_Cilindro")
  JOIN afrisan_actual."Pesaje" p ON p."Id_Pesaje"=e."Id_Pesaje" WHERE e."Id_OrdenTrabajo"=d."Id_OrdenTrabajo" AND c."Id_Refrigerante"=d."Id_Refrigerante") AS "GrContenidoDespachado",
 (SELECT sum(p."PesoBrutoGr"-c."TaraGr") FROM afrisan_actual."OperacionTerreno" op JOIN afrisan_actual."Cilindros" c ON c."Id_Cilindro"=op."Id_Instalado"
  JOIN afrisan_actual."Pesaje" p ON p."Id_Pesaje"=op."Id_PesajeInstalado" WHERE op."Id_OrdenTrabajo"=d."Id_OrdenTrabajo" AND op."Estado"='Registrada' AND c."Id_Refrigerante"=d."Id_Refrigerante") AS "GrContenidoInstalado"
FROM afrisan_actual."DetalleOT" d JOIN afrisan_actual."OrdenTrabajo" o USING("Id_OrdenTrabajo");
COMMENT ON VIEW afrisan_bi_actual."PedidoYEjecucion" IS 'Masas de etapas diferentes. No sumar despacho e instalación ni interpretar esas masas como consumo/emisiones. Sin registros permanece NULL.';

-- Bloqueos, auditoría e índices de consulta. No se instala ninguna cuenta.
DO $$ DECLARE t record; BEGIN
  FOR t IN SELECT tablename FROM pg_tables WHERE schemaname='afrisan_actual' AND tablename<>'Auditoria' LOOP
    EXECUTE format('CREATE TRIGGER %I AFTER INSERT OR UPDATE OR DELETE ON afrisan_actual.%I FOR EACH ROW EXECUTE FUNCTION afrisan_actual.auditar()','Auditar_'||t.tablename,t.tablename);
  END LOOP;
  FOR t IN SELECT tablename FROM pg_tables WHERE schemaname='afrisan_actual' LOOP
    EXECUTE format('ALTER TABLE afrisan_actual.%I ENABLE ROW LEVEL SECURITY',t.tablename);
  END LOOP;
END $$;
CREATE INDEX "IX_Pesaje_Cilindro_Fecha" ON afrisan_actual."Pesaje"("Id_Cilindro","Fecha");
CREATE INDEX "IX_Despacho_OT" ON afrisan_actual."Despacho"("Id_OrdenTrabajo");
CREATE INDEX "IX_Operacion_OT" ON afrisan_actual."OperacionTerreno"("Id_OrdenTrabajo");
CREATE INDEX "IX_Recepcion_OT" ON afrisan_actual."RecepcionBodega"("Id_OrdenTrabajo");
CREATE INDEX "IX_Incidencia_OT" ON afrisan_actual."Incidencia"("Id_OrdenTrabajo");
CREATE INDEX "IX_Evento_OT" ON afrisan_actual."EventoOT"("Id_OrdenTrabajo","Fecha");
CREATE INDEX "IX_Auditoria_Tabla_Fecha" ON afrisan_actual."Auditoria"("Tabla","Fecha");

REVOKE ALL ON ALL TABLES IN SCHEMA afrisan_actual,afrisan_bi_actual FROM PUBLIC;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA afrisan_actual FROM PUBLIC;
REVOKE EXECUTE ON ALL FUNCTIONS IN SCHEMA afrisan_actual FROM PUBLIC;
ALTER DEFAULT PRIVILEGES IN SCHEMA afrisan_actual REVOKE EXECUTE ON FUNCTIONS FROM PUBLIC;
ALTER DEFAULT PRIVILEGES IN SCHEMA afrisan_actual REVOKE ALL ON TABLES FROM PUBLIC;
DO $$ DECLARE r text; BEGIN
  FOREACH r IN ARRAY ARRAY['anon','authenticated'] LOOP
    IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname=r) THEN
      EXECUTE format('REVOKE ALL ON SCHEMA afrisan_actual,afrisan_bi_actual FROM %I',r);
      EXECUTE format('REVOKE ALL ON ALL TABLES IN SCHEMA afrisan_actual,afrisan_bi_actual FROM %I',r);
      EXECUTE format('REVOKE ALL ON ALL SEQUENCES IN SCHEMA afrisan_actual FROM %I',r);
      EXECUTE format('REVOKE EXECUTE ON ALL FUNCTIONS IN SCHEMA afrisan_actual FROM %I',r);
    END IF;
  END LOOP;
END $$;
COMMENT ON SCHEMA afrisan_actual IS 'Instalación vacía del contexto actual. Acceso solo desde backend de confianza. Sin políticas de navegador ni credenciales precargadas.';
COMMIT;
