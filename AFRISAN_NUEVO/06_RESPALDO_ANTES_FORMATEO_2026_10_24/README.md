# Respaldo antes del formateo — 24 de octubre de 2026

Responsable: Dante Moya — da.moyan@duocuc.cl.
Repositorio de destino: https://github.com/dantemoya/Afrisan.git.
La autorización para crear el commit y subir este respaldo fue recibida el 24/10/2026.

## Qué conserva el repositorio

- Aplicación nueva completa en `AFRISAN_NUEVO/03_CODIGO`, con las correcciones de inventario, custodia de cilindros, OT, selección por listas, QR y fotografías.
- Aplicación anterior, arquitectura, mockups, evidencias individuales y grupales, backlog, informes y presentación de avance.
- Procesos de negocio acordados y versiones de los scripts SQL. Los scripts guardados no equivalen a una copia de la base real de Supabase.
- Evidencias y comprobaciones del 22/10/2026 en `04_SEGUIMIENTO/VALIDACION_QR_Y_SELECCION_2026_10_22`.
- Copia de los archivos auxiliares de las carpetas `.local` en `ARCHIVOS_LOCALES`. Se conservan generadores de documentos, imágenes de las diapositivas, registros de ejecución, datos sintéticos y herramientas históricas. No son proyectos adicionales de la aplicación ni deben ejecutarse automáticamente.
- `INVENTARIO_ARCHIVOS_LOCALES.json` registra ruta original, ruta de respaldo, tamaño y SHA-256 de esas copias.
- `ENTORNO_DESARROLLO.txt` registra las versiones locales de Git y .NET disponibles al preparar el respaldo.
- El diagrama SVG de Supabase adjuntado anteriormente se conserva en `ARCHIVOS_LOCALES/diagrama_supabase`. Es un antecedente, no una exportación actual de la base.

El archivo `MANIFIESTO_GIT.json` enumera los archivos preparados para el commit y sus identificadores de contenido de Git. Se excluye a sí mismo para evitar una referencia circular. Los hashes son de objetos Git; los saltos de línea de Windows pueden hacer que un SHA-256 de un archivo restaurado sea diferente sin que cambie su contenido lógico.

Desde una copia restaurada en la etiqueta indicada se puede comprobar el contenido versionado con:

```powershell
.\AFRISAN_NUEVO\06_RESPALDO_ANTES_FORMATEO_2026_10_24\Verificar-Respaldo.ps1
```

La comprobación compara las rutas y objetos del commit con el manifiesto; no ejecuta pruebas de la aplicación.

## Qué requiere atención aparte

- `bin`, `obj`, `.vs` y preferencias `*.user` se regeneran. No forman parte del respaldo funcional.
- Credenciales, tokens, secretos de usuario de .NET, variables del sistema y sesiones de GitHub no se publican en el repositorio. Tras formatear habrá que autenticarse nuevamente y reconfigurar cualquier secreto real que se utilice.
- El prototipo nuevo conserva los registros y fotos del servidor en memoria: reiniciar la API ya los elimina. Este respaldo preserva el código y las evidencias guardadas en archivos, no esa memoria.
- Los borradores offline, fotos pendientes y sesiones guardadas en el navegador no son archivos del repositorio. Si hay trabajo pendiente en un navegador, debe conservarse antes de borrar su perfil o formatear. Este respaldo no exporta IndexedDB.
- No se exportó el contenido real de PostgreSQL/Supabase ni los archivos de sus buckets. El formateo del equipo no constituye una operación de borrado de esos servicios remotos.
- No es una copia del equipo completo: archivos personales y material fuera de la carpeta del proyecto requieren su respaldo correspondiente.

## Restaurar el proyecto

1. Instalar Git y Visual Studio con las herramientas de desarrollo web/ASP.NET y soporte para .NET 8. La solución usa `.slnx`; utilizar una instalación que pueda abrir ese formato. Las versiones observadas se guardan en `ENTORNO_DESARROLLO.txt`.
2. Iniciar sesión en GitHub con la cuenta que tiene acceso al repositorio privado. La autoría de Git y la cuenta con permiso para clonar son configuraciones distintas.
3. Clonar en una carpeta local y configurar la autoría institucional:

   ```powershell
   git clone https://github.com/dantemoya/Afrisan.git
   cd Afrisan
   git config --local user.name "Dante Moya"
   git config --local user.email "da.moyan@duocuc.cl"
   ```

4. Para recuperar exactamente este punto, usar la etiqueta del respaldo:

   ```powershell
   git switch -c codex/restauracion-formateo-2026-10-24 respaldo-antes-formateo-2026-10-24
   ```

5. Abrir `AFRISAN_NUEVO/03_CODIGO/Afrisan.Nuevo.slnx`. Restaurar los paquetes NuGet; también se puede ejecutar desde la raíz:

   ```powershell
   dotnet restore AFRISAN_NUEVO/03_CODIGO/Afrisan.Nuevo.slnx
   ```

6. En Visual Studio seleccionar **AFRISAN NUEVO - AMBOS** y presionar F5. La API usa `http://localhost:5181` y la web `http://localhost:5286/acceso`.
7. Consultar las cuentas públicas de demostración y el recorrido en `03_CODIGO/README.md`. La aplicación nueva arranca con inventario vacío y no necesita una conexión Supabase para el modo actual de revisión.
8. Detener con Shift+F5 antes de recompilar. No iniciar simultáneamente el launcher de PowerShell y la ejecución de Visual Studio: usan los mismos puertos y pueden bloquear las DLL.

La aplicación antigua está en la solución de la raíz y utiliza otros puertos. La continuidad del desarrollo corresponde a la solución de `AFRISAN_NUEVO/03_CODIGO`.

## Estado que debe retomarse

Ver `ESTADO_PARA_RETOMAR.md`. Este respaldo no introduce cambios funcionales ni acredita nuevas pruebas de la aplicación; conserva los cambios existentes y la evidencia previamente guardada.
