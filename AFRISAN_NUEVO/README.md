# AFRISAN: nueva etapa de desarrollo

Responsable e integrante actual: **Dante Moya**. Inicio de esta organización: **19 de octubre de 2026**.

Esta carpeta concentra el trabajo nuevo y permite separar la revisión del negocio
de la aplicación anterior. Primero se revisa el proceso, después los datos y
finalmente se programa. No se considera aprobado un flujo solo por estar escrito
o representado en un script SQL.

## Organización

Material para la defensa del 22 de octubre: [informe, presentación y guion](05_ENTREGA_AVANCE_2026_10_22/README.md).

| Carpeta | Contenido |
| --- | --- |
| [01_NEGOCIO](01_NEGOCIO/README.md) | Contexto, acuerdos, procesos y asuntos por confirmar. |
| [02_BASE_DATOS](02_BASE_DATOS/README.md) | Scripts existentes y diccionario con PK, FK y tipos. |
| [03_CODIGO](03_CODIGO/README.md) | Aplicación nueva .NET 8: registro y consulta de inventario en memoria. |
| [04_SEGUIMIENTO](04_SEGUIMIENTO/DECISIONES_Y_PENDIENTES.md) | Decisiones, pendientes y próximos pasos del trabajo individual. |

## Por dónde empezar

Entrega del 20/10/2026: [Trabajo técnico](03_CODIGO/TRABAJO_TECNICO.md), con OT
asignadas, reemplazos/agregados, fotos de ambos pesajes, guardado local,
incidencias y revisión. La persistencia central sigue siendo temporal en memoria;
no se alteró la base de Supabase.

1. [Contexto y acuerdos](01_NEGOCIO/00_CONTEXTO_Y_ACUERDOS.md).
2. [Registro de refrigerantes y cilindros](01_NEGOCIO/01_REGISTRO_REFRIGERANTES_Y_CILINDROS.md): primer proceso a programar; faltan definiciones del ingreso y pesaje inicial.
3. [Consulta de inventario](01_NEGOCIO/02_CONSULTA_INVENTARIO.md): proceso descrito para revisión, sin aceptación final registrada.
4. [Procesos siguientes](01_NEGOCIO/03_PROCESOS_PENDIENTES.md): agenda de revisión, sin aprobación implícita.

## Tecnología y documentación de referencia

Se mantiene lo acordado más recientemente: **.NET 8, ASP.NET Core Web API,
Blazor WebAssembly, EF Core y PostgreSQL/Supabase**. La conversación sobre .NET 10
no fue una autorización de migración. La API forma parte de la arquitectura.

Las fuentes se conservan en sus ubicaciones originales:

- [Acta de constitución](../EVIDENCIAS_GRUPALES/Documentación%20del%20Proyecto/ACTUAL/00_Acta_de_Constitucion_AFRISAN_Corregida.docx).
- [Product Backlog](../EVIDENCIAS_GRUPALES/Documentación%20del%20Proyecto/ACTUAL/01_Product_Backlog_AFRISAN_Corregido.xlsx).
- [Arquitectura](../ARQUITECTURA/README.md).
- [Evidencias de EVA 2](../EVIDENCIAS_GRUPALES/EVA_2/README.md).

## Separación del proyecto anterior

El 25/10/2026, a petición de Dante, se quitaron de la copia local los proyectos
antiguos Afrisan.Api y Afrisan.Web y su iniciador Iniciar-Demo.ps1. Su código sigue
en el historial de Git; los documentos y las evidencias conservan su autoría.

AFRISAN.slnx y su perfil de inicio en la raíz ahora abren únicamente los tres
proyectos nuevos de 03_CODIGO. También se mantiene la solución
03_CODIGO/Afrisan.Nuevo.slnx (API 5181 / Web 5286).

La antigua carpeta BASE_DATOS_NUEVA se trasladó completa a 02_BASE_DATOS.
Los nombres de sus archivos se conservaron. No se ejecutó SQL en Supabase
durante esta reorganización.
