# Conexión de diagnóstico a Supabase

Proyecto: `sugsgvrqxtzeaddkvzii`. El usuario confirmó el 29 de octubre de 2026 que
ejecutó el SQL adjunto, cuyo contenido coincide con el instalador
`04_Crear_Afrisan_Contexto_Actual.sql`. La instalación remota aún debe verificarse.

## Configurar y comprobar

En Supabase, abrir **Connect → Direct · Connection string**, buscar **Method** y
seleccionar **Session pooler**. Copiar los valores exactos: el host depende del
clúster, y el usuario del pooler lleva el identificador del proyecto. El host
directo suministrado (`db.sugsgvrqxtzeaddkvzii.supabase.co`) solo resolvió a IPv6
y no fue accesible en la comprobación inicial desde este equipo.

Desde PowerShell, en `D:\Afrisan\AFRISAN_NUEVO\03_CODIGO`:

```powershell
.\Configurar-Supabase.ps1
```

El script solicita la contraseña mediante entrada oculta y guarda la cadena en
**User Secrets**, fuera del repositorio y sin enviarla como argumento de proceso.
Este almacén de desarrollo no cifra el archivo del perfil local. No compartir
su contenido ni ejecutar `dotnet user-secrets list` para enviar resultados al chat.
No se necesitan claves `anon` o `service_role` para este diagnóstico PostgreSQL.

Valores predeterminados confirmados por el usuario: servidor
`aws-0-us-east-1.pooler.supabase.com`, puerto `5432`, base `postgres` y usuario
`postgres.sugsgvrqxtzeaddkvzii`. Se pueden cambiar mediante los parámetros
`-Servidor`, `-Puerto`, `-BaseDatos` y `-Usuario`.

Se exige TLS con validación de certificado y nombre del servidor (`VerifyFull`).
Si el certificado no está respaldado por una CA confiable en el equipo, descargar
el certificado raíz desde Supabase y añadir `-CertificadoRaiz 'RUTA_DEL_CERTIFICADO'`.

Para repetir la comprobación sin introducir nuevamente la contraseña:

```powershell
dotnet run --project .\Afrisan.Nuevo.Api --launch-profile http -- --verificar-supabase
```

También puede usarse la variable de entorno `ConnectionStrings__Supabase` con
una cadena Npgsql, evitando guardarla en archivos versionados. Esta variable tiene
prioridad sobre User Secrets. Las credenciales pertenecen únicamente a la API.

## Alcance del resultado

La comprobación abre una transacción **READ ONLY**, consulta el catálogo de
PostgreSQL y revierte la transacción. No instala SQL ni crea usuarios, roles,
permisos, datos o buckets. Termina sin iniciar el servidor HTTP.

- Salida 0: conexión verificada, presentes las 31 tablas y 9 vistas esperadas,
  con RLS habilitado en las tablas.
- Salida 1: configuración, autenticación, certificado o red pendientes.
- Salida 2: conexión correcta, objetos esperados ausentes o tablas sin RLS.

Comprueba nombres de objetos y RLS; no certifica cada columna, trigger, política
ni permiso. Una conexión como propietario `postgres` tampoco acredita el acceso
de una futura identidad de aplicación con permisos restringidos.

**La API normal sigue usando `InventarioStore` y `TrabajoStore` en memoria.**
Este paso prepara y comprueba la conexión; quedan pendientes los repositorios
transaccionales, usuarios reales, Storage privado y las pruebas de conservación
de datos después de reiniciar. No se habilitan escrituras con las cuentas demo.

Referencias: [conexiones de Supabase](https://supabase.com/docs/guides/database/connecting-to-postgres),
[TLS en Npgsql](https://www.npgsql.org/doc/security.html) y
[User Secrets de .NET](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0).
