# Verificación del cambio de modelo

Fecha: 19 de octubre de 2026.

- Compilación de API y Web: `dotnet build AFRISAN.slnx --no-restore --nologo -m:1 -p:UseSharedCompilation=false`. Resultado: 0 errores, 0 advertencias.
- Consistencia EF: `dotnet ef migrations has-pending-model-changes --project Afrisan.Api --no-build`. Resultado: sin cambios de modelo pendientes.
- Generación de SQL completa e incremental desde las migraciones: finalizada sin conexión a un servidor.
- Comprobación temporal con el modelo real de EF y la configuración DI: tipos de todas las FK compatibles; ninguna columna sombra accidental; rol único por FK; sin tabla puente de roles ni tabla de permisos; relación OT–intervenciones correcta.
- Comprobación de restricciones y migración: foto al cierre, pesajes con tres decimales, cálculos basados en masa emitida, conservación de roles al migrar y restauración al revertir.
- DI: UserManager mantiene soporte de roles y resuelve UsuarioStore.

Estas comprobaciones no ejecutaron SQL en PostgreSQL. No se verificaron la aplicación
de migraciones, los bloqueos concurrentes, login ni peticiones HTTP sobre una base real.
El ensayo de migración y las pruebas de API requieren primero reconciliar las tablas
manuales de Supabase y configurar la conexión/JWT.
