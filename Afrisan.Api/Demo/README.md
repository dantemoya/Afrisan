# Demostración sin base de datos

El modo demo está habilitado en `appsettings.Development.json` mediante
`Demo:Enabled`. Solo se permite en Development. Usa memoria compartida durante
la ejecución y no registra DbContext, consulta Supabase ni aplica migraciones.

## Arranque

### Visual Studio

Abre `AFRISAN.slnx` y selecciona **AMBOS** en el selector de inicio junto al
botón verde. Pulsa F5: el perfil inicia API y Web con HTTP local y abre el login
en el navegador configurado en Visual Studio. Si no aparece AMBOS, vuelve a abrir
la solución; alternativamente, en Propiedades de la solución configura varios
proyectos de inicio con acción Iniciar para API y Web y selecciona http en ambos.
Detén antes cualquier ejecución de estos proyectos iniciada desde terminal para
liberar los puertos 5141 y 5226.

### Terminal

Puedes ejecutar `./Iniciar-Demo.ps1` en PowerShell desde la raíz. Compila,
inicia los dos proyectos en segundo plano y abre el login cuando ambos responden.
Los registros quedan en `.local/demo-*.log`. Si un puerto está ocupado conserva
ese proceso; si corresponde a otro perfil, detén esa ejecución antes de reintentar.

Desde la raíz, ejecuta cada comando en una terminal diferente:

```powershell
dotnet run --project Afrisan.Api --launch-profile http
```

```powershell
dotnet run --project Afrisan.Web --launch-profile http
```

- Aplicación: http://localhost:5226/login
- Swagger: http://localhost:5141/swagger
- Estado: http://localhost:5141/health/ready

## Cuentas

Contraseña común de ejemplo: **DemoAfrisan2026!**

| Rol | Correo |
| --- | --- |
| Administrador | admin@afrisan.example |
| Jefatura | jefatura@afrisan.example |
| Bodega | bodega@afrisan.example |
| Técnico | tecnico@afrisan.example |

Los botones del login completan el formulario. Selecciona Bodega para recorrer
el despacho y retorno. Los accesos de cada rol siguen restringidos en la API.

## Datos y alcance

Se incluyen cuatro cilindros, tres refrigerantes, tres órdenes y movimientos
de ejemplo. Hay un cilindro despachado, uno con historial cerrado, otro disponible
y otro sin tara para mostrar la validación. Los datos se cargan en `DemoStore.cs`.

El adaptador `DemoEndpoints.cs` ofrece los contratos actuales de usuarios,
cilindros, refrigerantes, órdenes, despacho, retorno, historial y balance físico.
Cada movimiento conserva el usuario de la sesión. Las fotos se representan por
rutas; este modo no implementa almacenamiento de imágenes.

Intervenciones en terreno, sincronización offline, auditoría automática,
integraciones y reportes ambientales completos siguen pendientes. Los factores
GWP de ejemplo quedan vacíos para no presentar valores ambientales inventados.

Al reiniciar la API se pierden todos los cambios y se recargan los ejemplos.
La firma de los tokens también cambia; recarga la Web para iniciar sesión otra vez.

## Volver a la base real

Configura conexión y secretos según [Database/README.md](../Database/README.md).
Inicia la API con:

```powershell
dotnet run --project Afrisan.Api --launch-profile http -- --Demo:Enabled=false
```

Para inicializar explícitamente el esquema real:

```powershell
dotnet run --project Afrisan.Api --launch-profile http -- --Demo:Enabled=false --initialize-database
```

El modo real requiere su propio administrador y no admite las cuentas demo.
