# AFRISAN nuevo · Primera entrega de inventario

## Esquema corregido para futura persistencia (25 de octubre)

Se preparó [04_Crear_Afrisan_Contexto_Actual.sql](../02_BASE_DATOS/04_Crear_Afrisan_Contexto_Actual.sql)
para el flujo actual, con validación local del SQL. La [guía de base de datos](../02_BASE_DATOS/README.md)
explica instalación y correspondencia de entidades. **La API continúa en memoria**;
no se ejecutó este script en Supabase ni se implementó todavía el adaptador persistente.

## Flujo de terreno actualizado (25 de octubre)

El Técnico registra el retiro de Bodega con peso y foto obligatoria; el cilindro
queda En proceso de despacho. Después del cambio y la devolución, Bodega confirma
los envases y sus pesos y aprueba el trabajo. El Autorizador revisa fotografías y
registros después de esa aprobación y antes del cierre. La
[validación del flujo corregido](../04_SEGUIMIENTO/FLUJO_TERRENO_2026_10_25/README.md)
incluye 274 comprobaciones satisfactorias en tres casos y un recorrido adicional
completo por formularios del navegador. Esta actualización sustituye el despacho
por Bodega descrito en los antecedentes siguientes. Continúa en memoria.

## Validación de los tres roles (25 de octubre)

Se ejecutaron tres recorridos completos de Bodega → Autorizador → Técnico →
Bodega → Autorizador, con tres refrigerantes, nueve cilindros de valores distintos
y tres OT cerradas. La [evidencia y el guion repetible](../04_SEGUIMIENTO/VALIDACION_ROLES_2026_10_25/README.md)
registran 278 comprobaciones satisfactorias por HTTP y consultas en navegador
con las tres cuentas. Son datos e imágenes sintéticos en memoria; esta validación
no acredita todavía la integración con PostgreSQL/Supabase.

## Actualización del 22 de octubre

Se incorporó selección de cilindros mediante listas en el módulo del Autorizador,
lectura de imágenes QR y cámara, descarga PNG compatible, descarte de borradores
y correcciones de custodia y versiones. La [validación de esta entrega](../04_SEGUIMIENTO/VALIDACION_QR_Y_SELECCION_2026_10_22/README.md)
registra 17 comprobaciones HTTP satisfactorias y pruebas de navegador de selección,
creación de OT, lectura de imagen QR, imagen inválida y descarga. La cámara física
no se probó. Esta actualización prevalece sobre los pendientes históricos de QR
por cámara y ausencia de pruebas indicados más abajo. La persistencia central
sigue siendo temporal en memoria.

## Actualización: procesos del técnico (20 de octubre de 2026)

Se añadió el módulo de OT asignadas, reemplazos/agregados, fotos múltiples por
pesaje, incidencias, recepción e informe técnico. Consulta el recorrido, las
decisiones provisionales y los límites en **[TRABAJO_TECNICO.md](TRABAJO_TECNICO.md)**.
Las secciones siguientes describen la primera entrega de inventario y se
conservan como antecedente; la actualización técnica prevalece sobre las listas
anteriores que indicaban OT/cambios como no implementados.

Responsable actual: Dante Moya. Desarrollo iniciado el 19 de octubre de 2026.

Aplicación independiente de los proyectos anteriores, con **.NET 8, ASP.NET Core
Web API y Blazor WebAssembly**. Usa contratos compartidos e implementa PN-01 y
PN-02 como versión de revisión. La persistencia EF Core/PostgreSQL definida en
arquitectura aún no está conectada en esta solución.

## Abrir en Visual Studio

1. Abre **Afrisan.Nuevo.slnx**, dentro de esta carpeta.
2. Selecciona **AFRISAN NUEVO - AMBOS** junto al botón de inicio y presiona F5.
3. Se inicia la API en http://localhost:5181 y la Web abre http://localhost:5286/acceso.

Si el perfil compartido no aparece, configura varios proyectos de inicio en las
propiedades de la solución: Afrisan.Nuevo.Api y Afrisan.Nuevo.Web con acción
Iniciar y perfil http. Contratos es una biblioteca y no se inicia.

Para iniciar con PowerShell:

```powershell
.\Iniciar-Revision.ps1
```

O usa dos terminales desde esta carpeta:

```powershell
dotnet run --project Afrisan.Nuevo.Api --launch-profile http
```

```powershell
dotnet run --project Afrisan.Nuevo.Web --launch-profile http
```

No ejecutar el launcher y F5 simultáneamente: los procesos usan los mismos puertos.
Los puertos de la aplicación anterior (5141/5226) no son los de esta entrega.

## Modo de revisión y datos

**Solo Development. Inventario y catálogo vacíos al arrancar.** Lo que registres
queda en memoria de la API y desaparece al reiniciarla. Reiniciar la Web no
borra datos si la API sigue encendida. No hay conexión, migraciones ni inserciones
en Supabase. La firma de sesiones cambia al reiniciar API; recarga la Web para
volver a entrar. No se admite iniciar esta versión en Production.

Las cinco cuentas públicas de revisión se generan en memoria, no en la tabla
Usuario. Contraseña común: **DemoAfrisan2026!**. Los botones del acceso completan
las credenciales; no realizan el login hasta pulsar Entrar.

| Rol | Correo |
| --- | --- |
| Bodega | bodega@afrisan.example |
| Autorizador | autorizador@afrisan.example |
| Jefatura | jefatura@afrisan.example |
| Técnico | tecnico@afrisan.example |
| Administrador | administrador@afrisan.example |

No corresponden a usuarios reales. Cinco intentos fallidos bloquean una cuenta
demo durante 15 minutos. Sesiones JWT de dos horas; API valida firma, emisor,
audiencia, vencimiento y rol. El cliente usa sessionStorage con clave propia.

## Recorrido implementado

1. Entrar como Bodega.
2. Abrir Refrigerantes y registrar código/nombre del tipo de gas.
3. Registrar un cilindro con un refrigerante, capacidad y tara en **gramos**.
4. Opcionalmente ingresar el peso bruto inicial. Si no existe lectura, queda
   PendientePesaje y no se calcula un contenido ficticio.
5. Consultar inventario por código/serie, refrigerante y estado.
6. Abrir la ficha, ver el peso y el contenido calculado, descargar el código QR.
7. Si faltaba lectura inicial, registrarla desde la ficha; queda con fecha,
   usuario y observación, separada de un despacho/retorno.

Los refrigerantes no tienen kilos de stock propios. Un tipo puede estar en
muchos cilindros, pero cada envase referencia un único tipo.

### Reglas incorporadas

- Código de refrigerante normalizado y único; nombre/código no vacíos.
- Número de serie opcional y sin duplicados, sin distinguir mayúsculas.
- Código de cilindro generado por API, estable y único en la instancia.
- Capacidad y tara positivas; lectura entre tara y tara más capacidad.
- Sin lectura: PendientePesaje. Peso igual a tara: Vacio. Mayor a tara: Disponible.
- Contenido según lectura = peso bruto menos tara. No se registra consumo ficticio.
- El primer pesaje no se puede reemplazar mediante el mismo endpoint.
- Las escrituras se serializan en memoria para evitar duplicados concurrentes.
- No existen botones de edición/borrado libre de pesos ni del gas del envase.

## Decisiones provisionales de esta entrega

Estos puntos NO se presentan como aprobación de negocio:

- Se permite el alta sin pesaje inicial, indicando expresamente que queda
  pendiente. Falta confirmar si la operación real permite ese ingreso.
- La foto inicial y el documento de recepción no se exigen ni se cargan todavía.
- El registro del catálogo está permitido a Bodega, Jefatura y Administrador.
- El alta/pesaje de cilindros está permitido a Bodega y Administrador.
- Inventario y catálogo: consulta para Bodega, Autorizador, Jefatura y Administrador.
- Técnico consulta una ficha por código y su historial, pero no lista todo el inventario.

La API aplica estas restricciones; la pantalla no es la única barrera. Esta
matriz provisional debe confirmarse para PN-01/PN-02; las asignaciones de OT y
jerarquía de límites no se generalizan automáticamente a todos los módulos.

## Qué aún no está implementado

- Conexión a Supabase, repositorios EF Core, persistencia y autenticación reales.
- Migración desde las tablas y cuentas del esquema anterior.
- OT, autorización, despacho, cambios físicos en sucursal, ubicación/custodia,
  límites semanales/mensuales y su jerarquía Bodega → Autorizador → Jefatura.
- Retorno, conciliación, rectificaciones, reportes, emisiones y certificados.
- Cámara QR, captura/Storage de fotografías, offline y sincronización.
- Auditoría general: esta entrega solo muestra trazas de alta y primer pesaje
  del cilindro. No reemplaza el proceso documental de auditoría de correcciones.
- Gestión de usuarios o contraseñas reales y revocación individual de tokens.

### Pendiente de alinear con la base SQL

El último SQL no representa pesajes iniciales por separado ni estos estados de
revisión (PendientePesaje/Vacio); además, su CHECK de roles no admite Autorizador.
No se ejecutaron ALTER ni se modificaron los scripts ya entregados para hacerlos
parecer compatibles. Confirmar el negocio y preparar una migración antes de
conectar este código a la base. InventarioStore es la persistencia temporal a
sustituir, no un modelo EF persistido.

## API y compilación

- Swagger: http://localhost:5181/swagger
- Estado: http://localhost:5181/health/ready
- Rutas: /api/v1/entorno, /sesiones, /refrigerantes, /cilindros,
  /cilindros/{codigo}, /cilindros/{codigo}/historial,
  /cilindros/{codigo}/pesaje-inicial, /cilindros/{codigo}/qr.

```powershell
dotnet build Afrisan.Nuevo.slnx -m:1 -p:UseSharedCompilation=false
```

Compilación realizada: **0 errores y 0 advertencias**. No se añadieron ni ejecutaron
pruebas automatizadas en esta entrega. El resultado de compilar no acredita
aceptación funcional del cliente ni compatibilidad del SQL en nube.
