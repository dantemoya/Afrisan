# Decisiones y seguimiento individual

Responsable: **Dante Moya**. Actualizado: **19 de octubre de 2026**.
Última actualización de acuerdos: **20 de octubre de 2026**.

## Decisiones registradas

| ID | Decisión | Origen |
| --- | --- | --- |
| D-01 | Concentrar el trabajo nuevo en AFRISAN_NUEVO. | Petición de Dante de separar y ordenar el proyecto. |
| D-02 | Entender el negocio antes de diseñar datos/pantallas. | Corrección expresa de Dante. |
| D-03 | Primer proceso a programar: refrigerantes y cilindros; un solo refrigerante por cilindro. | Definición expresa de Dante. |
| D-04 | Mantener tecnologías de arquitectura, acta y backlog, incluida .NET 8. | Última decisión tecnológica de Dante; no migrar a .NET 10. |
| D-05 | Documentar consulta de inventario para revisar su flujo. | Solicitud de Dante; no implica aceptación final de cada detalle. |
| D-06 | Instalación de estructura sin poblar tablas. | Solicitud expresa de Dante. |
| D-07 | No agregar tabla Permisos. | Acuerdo previo de esta conversación. |
| D-08 | Desarrollo inmediato limitado a PN-01 y PN-02; OT/despacho/retorno pendientes de revisión. | Instrucción posterior de Dante. |
| D-09 | OT como solicitud de sucursal, detalle por cilindros/refrigerantes y kilos, técnico obligatorio, cambios/agregados y control de kilos por sucursal. | Definición directa de Dante; significado físico del cambio y límites pendientes. |
| D-10 | El cambio de la OT es reemplazo físico de cilindros. Resuelve la duda de significado físico de D-09; no equivale automáticamente a recargar equipos. | Respuesta expresa posterior de Dante. |
| D-11 | Bodega define límites de kilos por sucursal; por ahora, jefatura también. Período y reacción al exceso pendientes. | Respuesta expresa posterior de Dante. |
| D-12 | Límites semanales y mensuales, definidos por bodega para cada sucursal. Resuelve los períodos pendientes de D-11; cómputo y calendario por precisar. | Última aclaración de Dante. |
| D-13 | Ante exceso, conservar registro e investigar información y autorizaciones de técnicos/bodegueros con jefatura. No determinar culpabilidad automáticamente. | Última aclaración de Dante; campos de incidencia/revisión propuestos, sin implementar. |
| D-14 | Bodega y jefatura definen límites con jerarquía: una modificación de jefatura bloquea la edición por bodega; solo jefatura puede modificar o habilitar otra vez esa edición. | Corrección expresa de Dante; completa D-11/D-12. |
| D-15 | Autorizador es un rol nuevo, distinto de Jefatura; responsable de procesos 1, 2, 7, 8 y 9 de la última lista. | Respuestas expresas de Dante. |
| D-16 | Proceso 4 asignado al técnico, corrigiendo la mención anterior de bodega. Bodega queda en 3, 6 y 8. | Corrección expresa de Dante. |
| D-17 | Jerarquía de menor a mayor: Bodega → Autorizador → Jefatura. Autorizador no puede invalidar el bloqueo de Jefatura. El mecanismo de bloqueo por cambios de Autorizador aún debe precisarse. | Última definición de Dante; no cambia automáticamente las tareas asignadas. |
| D-18 | Fotos obligatorias de los pesajes del cilindro retirado y del de reemplazo; varias fotos por operación, identificadas por envase/pesaje. | Instrucción expresa de Dante del 20/10/2026; todavía sin implementar. |
| D-19 | Programar procesos del técnico. Amplía el alcance inicial D-08; se añadió una entrega en memoria con OT de soporte y evidencias múltiples, sin modificar Supabase. | Solicitud expresa de Dante del 20/10/2026. |

## Estado real

- D-20 (20/10/2026): incluir un módulo de gráficos con Power BI en la nueva
  aplicación, por recordatorio expreso de Dante. Indicadores, acceso por rol y
  forma de integración pendientes; alcance en 01_NEGOCIO/11_MODULO_POWER_BI.md.

- Procesos PN-01 y PN-02 documentados con sus pendientes.
- PN-03 (OT/guía), PN-04 (despacho) y PN-05 (pendientes de devolución) añadidos
  a petición de Dante como propuestas de revisión; no se registró su aprobación.
- Scripts y diccionario trasladados a 02_BASE_DATOS sin cambiar su contenido SQL.
- Dante informó ejecución exitosa del script; el SVG exportado muestra tablas
  y relaciones. No se certificaron mediante consulta en vivo todos los triggers,
  políticas, columnas calculadas o vistas.
- Primera aplicación nueva iniciada en 03_CODIGO/Afrisan.Nuevo.slnx: contratos,
  API y Blazor WebAssembly .NET 8. Registro de gas/envase, pesaje inicial,
  consulta/filtros/ficha/QR con datos en memoria; inventario vacío al iniciar.
- Permisos de PN-01/PN-02 y alta sin pesaje son provisionales y están explicados
  en 03_CODIGO/README.md. Se añadió el trabajo técnico de D-19 con OT manual de
  soporte, entregas/recepciones por bodega, fotos múltiples y término/revisión.
  Los límites semanales/mensuales y su jerarquía de edición siguen sin programar.
- Compilación de esta entrega: cero errores y advertencias. Sin pruebas
  automatizadas añadidas/ejecutadas ni conexión a Supabase.
- 20/10/2026: se inició la actualización y se abrió el perfil Técnico en
  /ordenes. Cargó la lista vacía sin errores visibles. No se declara ejecutado
  el recorrido completo de reemplazo, fotografías, recepción, cierre u offline.
  Detalles y decisiones provisionales en 03_CODIGO/TRABAJO_TECNICO.md.
- La implementación y las evidencias anteriores permanecen en sus carpetas.
- Definición de OT guardada en 01_NEGOCIO/07_OT_DEFINICION_DANTE.md, sin
  programarla ni modificar el esquema sobre interpretaciones pendientes.

## Próximos pasos

1. Resolver ingreso y pesaje inicial de PN-01.
2. Revisar columnas, filtros, estados y significado de pesos de PN-02.
3. Ajustar el modelo a los procesos confirmados.
4. Crear solución nueva con el stack acordado y programar la primera parte.
5. Revisar despacho y los demás procesos por separado.

## Registro de esta reorganización

19/10/2026: se creó la estructura de trabajo nuevo y se trasladó
BASE_DATOS_NUEVA a AFRISAN_NUEVO/02_BASE_DATOS. No se borraron código fuente,
evidencias ni contribuciones históricas. No se ejecutó ningún script SQL remoto.
