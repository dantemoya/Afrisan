-- Ejecutar después del esquema EF con un usuario administrador.
-- Los usuarios de la aplicación pasan por Afrisan.Api. Las políticas/usuarios
-- de reportería se configuran por separado; este script no concede acceso a Power BI.
DO $$
DECLARE tabla text;
BEGIN
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
