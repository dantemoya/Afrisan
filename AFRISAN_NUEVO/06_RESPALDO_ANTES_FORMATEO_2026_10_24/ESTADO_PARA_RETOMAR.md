# Estado del trabajo al preparar el respaldo

Fecha: 24 de octubre de 2026. Responsable humano actual: Dante Moya.

## Implementación de referencia

`../03_CODIGO/Afrisan.Nuevo.slnx`, ASP.NET Core y Blazor WebAssembly con destino .NET 8.
La persistencia central y las fotografías del servidor siguen en memoria. La aplicación nueva no está conectada a Supabase ni a Power BI.

Se conservan las correcciones recientes de ubicación inicial e inventario de bodega, custodia por OT, recepción y devolución sin uso, selección de cilindros del Autorizador, lectura de imagen QR, descarga del QR, descarte de borradores y control de versiones.

Las 17 comprobaciones HTTP y las evidencias de navegador del 22/10 están en `../04_SEGUIMIENTO/VALIDACION_QR_Y_SELECCION_2026_10_22`. La cámara física quedó sin probar. La última compilación observada el 22/10 terminó con cero errores y cero advertencias; el respaldo no ejecuta una nueva batería de pruebas.

## Decisión de negocio que guía la continuación

La OT de una sucursal solicita kilos por refrigerante, tiene un técnico asignado y registra reemplazos físicos o agregados de cilindros. El instalado permanece en la sucursal y el retirado vuelve a bodega. Se necesitan fotos de los pesajes de ambos envases en un reemplazo, con varias imágenes por operación.

Los límites son semanales y mensuales por sucursal. Bodega y Jefatura los definen; una modificación de Jefatura protege la edición hasta que Jefatura la habilite otra vez. Los excesos se registran y revisan, sin atribuir culpabilidad automáticamente. Debe precisarse qué kilos computan y qué calendario se aplica.

Las reglas detalladas están en `../01_NEGOCIO/07_OT_DEFINICION_DANTE.md`, `09_PROCESOS_TECNICO.md` y `10_PROCESOS_AUTORIZADOR.md`.

## Base de datos: revisión del 24/10/2026

El listado de tablas compartido por Dante conserva Rol, Usuario, Cliente, Sucursales, Refrigerantes, Cilindros, Equipos, CausasFalla, OrdenTrabajo, Movimientos, Intervencion, PreciosRefrigerantes, Costos, CertificadosDisposicion, CertificadosCilindros y Auditoria.

Ese modelo requiere conciliación con la OT actual antes de conectar la aplicación:

1. Técnico asignado en OrdenTrabajo y detalle por refrigerante con solicitado y autorizado.
2. Operación física con tipo reemplazo/agregado y relación entre cilindro retirado e instalado.
3. Transferencias, ubicación y custodia que permitan dejar instalado un cilindro y recibir otro, sin exigir el retorno de todos los nuevos instalados.
4. Pesajes históricos y varias evidencias por pesaje/cilindro.
5. Límites semanales/mensuales, vigencia, historial y protección de edición de Jefatura.
6. Incidencias, revisión de excesos y trazabilidad de autorizaciones.
7. Admisión del rol Autorizador y revisión de estados, FK, triggers y condiciones de cierre del SQL anterior.

El backlog previo describe inyección de gas y devolución de los mismos envases. Principalmente HU-05, HU-06 y HU-08 requieren reconciliarse con el reemplazo físico, junto con las historias de Autorizador y límites. Equipos, fallas y semáforo por capacidad deben conservarse o ajustarse según el alcance acordado; no se eliminan por suposición.

No se consultó ni modificó la base de Supabase durante esa revisión. El listado de columnas no confirma las FK, índices, triggers ni políticas desplegadas. Los scripts antiguos no deben ejecutarse sobre datos reales como si fueran una migración compatible con el flujo actual.

## Próximos avances

- Acordar el diseño de datos y actualizar los criterios de aceptación afectados.
- Implementar persistencia central y almacenamiento privado de fotografías.
- Completar límites, incidencias y correcciones con reglas acordadas.
- Preparar consultas y módulo de Power BI con cantidades y fórmulas definidas.
- Retomar las pruebas pendientes de cámara, trabajo offline completo y despliegue público.

El informe y la presentación del avance están en `../05_ENTREGA_AVANCE_2026_10_22`. El análisis anterior de incongruencias está en `../04_SEGUIMIENTO/REVISION_2026_10_20`.
