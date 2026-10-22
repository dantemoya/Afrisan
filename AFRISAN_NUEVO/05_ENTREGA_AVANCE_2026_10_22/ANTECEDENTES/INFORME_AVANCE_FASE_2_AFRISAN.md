# Informe de avance de AFRISAN Fase 2

Proyecto: Sistema Integral de Gestión y Trazabilidad de Refrigerantes AFRISAN
Integrante actual y responsable de continuidad: Dante Moya
Fecha de corte documental: 16 de octubre de 2026
Asignatura: Capstone PTY4614, según pauta proporcionada
Entrega: semana 9 según pauta; fecha exacta por confirmar
Único integrante actual: Dante Moya
Aplicación individual de la pauta: pendiente de confirmación docente

## Índice

- 1 Resumen del avance
- 2 Abstract
- 3 Objetivos y metodología del avance
- 4 Continuidad individual y ajustes
- 5 Desarrollo de ingeniería identificado
- 6 Evidencias y calidad
- 7 Monitoreo del plan de trabajo
- 8 Dificultades y ajustes a partir del monitoreo
- 9 Individual conclusions
- 10 Individual reflection
- 11 Fuentes y referencias
- 12 Anexo de operaciones principales
- 13 Anexo de validación propuesta

## 1 Resumen del avance

AFRISAN busca dar trazabilidad a cilindros de refrigerante y sus movimientos. El repositorio contiene una aplicación Blazor WebAssembly, una API ASP.NET Core y un modelo de persistencia para PostgreSQL. La inspección identifica código de inventario, órdenes de trabajo locales, despacho, retorno, historial, diferencia de pesos y autenticación por roles.

Dante Moya continúa como único integrante actual. Se corrige la documentación para distinguir implementación, validación y aceptación. Las funciones de terreno, trabajo offline, fotografía, semáforo, certificados y reportería siguen pendientes. No se declara cumplimiento total de historias ni se acredita funcionamiento en producción.

## 2 Abstract

AFRISAN aims to track refrigerant cylinders and inventory movements. The repository contains a Blazor WebAssembly client, an ASP.NET Core API, and a PostgreSQL persistence model. Source inspection identifies inventory management, local work orders, dispatch, returns, movement history, a basic weight comparison, and role-based authentication.

Dante Moya is the sole current student member responsible for continuing the project. This report distinguishes implemented code from runtime validation and user acceptance. Field interventions, offline synchronization, photographic evidence, leak alerts, disposal certificates, and environmental reporting remain pending. No full completion percentage or production readiness is claimed.

## 3 Objetivos y metodología del avance

El objetivo de este avance es documentar la base implementada, identificar sus límites y preparar la validación del recorrido de bodega. El alcance completo del proyecto se mantiene en las diez historias originales del Product Backlog. Cualquier modificación de objetivos o criterios de aceptación se debe acordar con el cliente y el docente.

- Describir el modelo real, las operaciones de la API y las vistas existentes.

- Relacionar HU-01, HU-02, HU-03 y HU-08 con su implementación y sus pendientes, sin declararlas aceptadas.

- Actualizar el plan y la responsabilidad actual para la continuidad individual.

- Diseñar casos de validación y registrar posteriormente resultados reproducibles.

La organización propuesta es incremental por historias de usuario: revisar alcance, planificar tareas, implementar o corregir, validar y registrar resultados. En esta entrega se acredita inspección de código y corrección documental. El historial Git permite identificar cambios anteriores. No se adjuntan actas que acrediten ceremonias Scrum ni resultados de pruebas ejecutadas.

## 4 Continuidad individual y ajustes

Dante Moya es el único integrante actual y responsable de continuidad, según lo informado por el estudiante. Esta asignación es prospectiva: no sustituye la autoría de los aportes previos. La aplicación individual de la pauta grupal debe acordarse con el docente; no se adjunta una autorización. Las duraciones y fechas de actividades pendientes deben estimarse con la capacidad actual.

La pauta 2.5 exige grupos de dos a tres estudiantes y asigna indicadores al aporte de cada integrante y al trabajo en equipo. Se debe confirmar con el docente cómo se aplicarán a la continuidad individual. La asistencia de herramientas de IA no representa un segundo estudiante. Se conserva el aporte de integrantes anteriores como antecedente histórico.

Se conservan los diez identificadores HU-01 a HU-10, las prioridades, los puntos y los sprints de referencia del Product Backlog de Fase 1. HU-03 sigue siendo autenticación; no se crea HU-SEG. Los sprints indicados son la planificación de referencia, no una confirmación de calendario vigente. La siguiente iteración y sus fechas están por acordar.

El backlog original ubica HU-03 en Sprint 1. Los commits que implementaron autenticación llevan la etiqueta sprint-3; esto describe el historial de implementación y no demuestra un adelanto desde la planificación original. La reprogramación de funciones pendientes es una propuesta por confirmar, sin semanas ni fechas comprometidas en este informe.

## 5 Desarrollo de ingeniería identificado

La arquitectura está aprobada, según confirmación del estudiante recibida el 16 de octubre de 2026. El desarrollo continúa sobre la arquitectura seleccionada; los pendientes corresponden a implementación, integración, validación funcional y evidencias.

La presentación se implementa en Afrisan.Web con Blazor WebAssembly .NET 8. La API está en Afrisan.Api con ASP.NET Core .NET 8, controladores, DTOs y Entity Framework Core con Npgsql. Se configura PostgreSQL como proveedor. Supabase es la plataforma prevista en la documentación del proyecto; este informe no adjunta una consulta del servidor ni acredita versión, disponibilidad o despliegue en producción.

Flujo de comunicación: Afrisan.Web utiliza HttpClient para enviar JSON a Afrisan.Api; los controladores consultan o actualizan AfrisanDbContext. El código de negocio está principalmente en controladores. No hay un proyecto de modelos compartidos entre cliente y API que permita afirmar reutilización automática de DTOs.

| Entidad | Campos implementados | Límites relevantes |
| --- | --- | --- |
| Cilindro | Id, CodigoQr, TipoRefrigerante, CapacidadKg, TaraKg, PesoActualKg, Estado, FechaRegistro, GasRefrigeranteId. | Sin número de serie ni fecha de inspección en este modelo. |
| GasRefrigerante | Id, Nombre, Codigo, Descripcion, Activo; colección Cilindros. | Sin fórmula química, GWP, toxicidad o inflamabilidad. |
| OrdenTrabajo | Id, Codigo, Descripcion, Activa, FechaCreacion; colección Movimientos. | Sin cliente, dirección, equipo o estado de cierre administrativo. |
| MovimientoInventario | Id, CilindroId, OrdenTrabajoId opcional, TipoMovimiento, PesoKg, Responsable, FechaMovimiento, Observaciones. | Responsable es texto libre. No hay UsuarioId ni entidad de intervención. |
| Usuario | IdentityUser más NombreCompleto, Activo y FechaCreacion. | Identity permite cuentas y roles; el movimiento aún no se relaciona con esta entidad. |

AfrisanDbContext configura índices únicos para el código QR y el código de OT y relaciones de los movimientos con cilindro y OT. Se incluyen migraciones en el repositorio. Se debe comprobar su aplicación sobre la base utilizada en la demostración; la existencia del archivo de migración no acredita su ejecución.

AuthController valida credenciales mediante Identity, controla cuenta activa e intentos fallidos y emite JWT con vigencia de ocho horas. La API valida firma, emisor, audiencia y expiración. Los roles implementados son Administrador, Jefatura, Bodega y Tecnico. El cliente conserva el token y usa JwtAuthenticationStateProvider para representar la sesión; la autorización efectiva corresponde a la API.

El despacho exige cilindro Disponible, OT local activa, responsable y peso válido. La tara debe estar registrada y ser positiva. Se valida: Peso bruto de salida > Tara; Peso bruto de salida <= Tara + Capacidad. La comprobación del estado no demuestra protección frente a dos solicitudes simultáneas: ese caso queda pendiente de validación.

El retorno exige estado Despachado, responsable y peso válido. Guarda un movimiento y cambia el cilindro a Disponible. Actualmente el movimiento de retorno no conserva OrdenTrabajoId ni un identificador explícito del despacho. El cálculo de balance empareja el último retorno con el despacho anterior por fecha e identificador.

Diferencia de peso (kg) = Peso bruto de despacho - Peso bruto de retorno. Esta comparación básica no implementa toda HU-08: faltan consumos declarados en intervenciones, tolerancia, gestión de descalces, comprobación de cilindros por OT y cierre de custodia. La regla RN-03 del backlog corresponde al semáforo de fuga, no se redefine como esta resta.

IdentificacionQr.razor genera QR mediante QRCoder en el proyecto web y permite buscar por código. Cilindros.razor contiene los formularios de inventario y movimientos. DetalleCilindro.razor consulta ficha, historial y balance según rol. No se identificó un lector óptico integrado en la aplicación ni integración con Defontana. El método de lectura externo o por cámara debe acordarse y demostrarse.

## 6 Evidencias y calidad

Las evidencias identificadas a continuación permiten inspeccionar la implementación y la planificación. No sustituyen los resultados de compilación, pruebas o aceptación que aún se deben adjuntar. No se afirma compilación sin advertencias, despliegue efectivo de migraciones ni certificación de calidad sin resultados verificables.

| ID | Evidencia identificada | Qué acredita y qué falta |
| --- | --- | --- |
| E-01 | Afrisan.Api/Models y Data/AfrisanDbContext.cs. | Estructura definida en código. Falta cotejar con base desplegada. |
| E-02 | Afrisan.Api/Migrations. | Migraciones disponibles. Su presencia no demuestra ejecución en Supabase. |
| E-03 | Controllers de Auth, Cilindros, Gases, OrdenesTrabajo, Usuarios y MovimientosInventario. | Operaciones y validaciones implementadas. Falta registrar ejecución. |
| E-04 | Afrisan.Web/Pages y Services. | Vistas y manejo de autenticación en código. Falta evidencia de uso del sistema. |
| E-05 | Commits c2a7bfd, 3ed71c2 y 4cfce18. | Avances históricos registrados con autor Tormenta-Yarek; no se reasigna su autoría a Dante. |
| E-06 | Documentos de EVIDENCIAS_GRUPALES/EVA_2. | Plan, backlog e informe corregidos. Su entrega en Git sigue pendiente hasta versionarlos. |
| E-07 | 04_MATRIZ_VALIDACION_PENDIENTE.md. | Diez casos diseñados como propuesta. No acredita ejecución ni aprobación. |

Para completar el avance se requiere registrar entorno y fecha de compilación, ejecutar los casos de validación acordados y adjuntar respuestas de API, capturas y consultas de persistencia con datos de prueba. Cada evidencia debe indicar qué objetivo o criterio demuestra. Deben registrarse defectos encontrados y correcciones; las credenciales no deben aparecer en las capturas.

## 7 Monitoreo del plan de trabajo

El plan siguiente usa las ocho columnas de la guía 2.4. Dante Moya es el responsable actual de las actividades. Las duraciones quedan por estimar con la capacidad disponible; no se inventan fechas históricas ni avances de validación.

| Competencia o unidades de competencias | Actividades | Recursos | Duración de la actividad | Responsable | Observaciones | Estado de avance | Ajustes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Construir modelos de datos para soportar los requerimientos de la organización de acuerdo a un diseño definido y escalable en el tiempo. | Revisar y validar el esquema relacional existente. | EF Core 8, Npgsql, modelos y migraciones. | Por estimar | Dante Moya | Hay modelos y migraciones en código. Su aplicación en la base de datos requiere evidencia de ejecución. | En curso | Describir solo las entidades reales. Preparar ampliaciones de terreno como trabajo pendiente. |
| Desarrollar una solución de software utilizando técnicas que permitan sistematizar el proceso de desarrollo y mantenimiento, asegurando el logro de los objetivos. | Consolidar la API de cilindros, gases, OT y movimientos. | .NET 8, controladores y DTOs. | Por estimar | Dante Moya | Se inspeccionaron reglas de estado y pesaje. No hay resultados de ejecución adjuntos a esta entrega. | En curso | Corregir defectos tras validar el recorrido funcional y la operación simultánea. |
| Desarrollar una solución de software utilizando técnicas que permitan sistematizar el proceso de desarrollo y mantenimiento, asegurando el logro de los objetivos. | Validar autenticación y permisos por rol (HU-03). | ASP.NET Core Identity, JWT, Blazor. | Por estimar | Dante Moya | Hay login, tokens y autorización. No se atribuye auditoría de usuario a movimientos que solo guardan responsable en texto. | En curso | Conservar HU-03 y la estimación original. Registrar pruebas y completar administración web si se acuerda. |
| Desarrollar una solución de software utilizando técnicas que permitan sistematizar el proceso de desarrollo y mantenimiento, asegurando el logro de los objetivos. | Completar el recorrido web de identificación y despacho. | Blazor WebAssembly, QRCoder, HttpClient. | Por estimar | Dante Moya | QR se genera en IdentificacionQr; el despacho está en Cilindros; DetalleCilindro consulta ficha e historial. | En curso | Confirmar método de lectura QR y origen de OT. Las OT actuales se crean localmente. |
| Desarrollar una solución de software utilizando técnicas que permitan sistematizar el proceso de desarrollo y mantenimiento, asegurando el logro de los objetivos. | Completar retorno y conciliación (HU-08). | Controlador de movimientos, modelo y web. | Por estimar | Dante Moya | Hay retorno y diferencia de pesos; no existe todavía conciliación con consumos de terreno. | En curso | Vincular retorno con despacho/OT. Definir consumo declarado, tolerancia, alertas y cierre. |
| Desarrollar una solución de software utilizando técnicas que permitan sistematizar el proceso de desarrollo y mantenimiento, asegurando el logro de los objetivos. | Diseñar intervenciones en terreno, evidencia y modo offline. | Cliente móvil o web por definir; API y datos a ampliar. | Por estimar | Dante Moya | HU-04 a HU-07 sin implementación identificada. El esquema actual no contiene equipos ni intervenciones. | No iniciado | Proponer nueva secuencia y estimación según capacidad individual, pendiente de acuerdo con cliente/docente. |
| Desarrollar una solución de software utilizando técnicas que permitan sistematizar el proceso de desarrollo y mantenimiento, asegurando el logro de los objetivos. | Diseñar certificados Regener y reportería ambiental. | Requisitos, modelo de datos y tecnología por definir. | Por estimar | Dante Moya | HU-09 y HU-10 pendientes. Confirmar documentos y datos disponibles antes de elegir integración. | No iniciado | Planificar después de acordar alcance y dependencias, sin comprometer semanas no confirmadas. |
| Realizar pruebas de certificación tanto de los productos como de los procesos utilizando buenas prácticas definidas por la industria. | Ejecutar y documentar validaciones del flujo y permisos. | Matriz de 10 casos propuesta; web, API y datos de prueba. | Por estimar | Dante Moya | Se propone una matriz en esta corrección. Los casos no están ejecutados ni existen resultados observados. | No iniciado | Registrar resultado esperado/observado, fecha, evidencia y correcciones antes de declarar conformidad. |
| Gestionar proyectos informáticos, ofreciendo alternativas para la toma de decisiones de acuerdo a los requerimientos de la organización. | Actualizar planificación y documentar continuidad individual. | Git, backlog, plan de trabajo y acuerdos. | Seguimiento continuo | Dante Moya | El estudiante informa que continúa como único integrante. Falta constancia docente para aplicar la pauta grupal. | En curso | Responsabilidad actual de Dante; conservar autoría histórica. Acordar fechas y criterios de evaluación. |

## 8 Dificultades y ajustes a partir del monitoreo

| Criterio de monitoreo | Situación observada | Acción o propuesta |
| --- | --- | --- |
| Dificultades | Dante Moya informa que continúa como único integrante. La revisión detecta diferencias entre documentación y código, funciones de terreno pendientes y ausencia de resultados de validación adjuntos. | Actualizar responsables y alcance; solicitar acuerdo docente sobre evaluación individual; estimar trabajo restante y reunir evidencia. |
| Facilitadores | Arquitectura aprobada, según confirmación del estudiante recibida el 16 de octubre de 2026. El repositorio ya contiene API, web, modelos, migraciones y autenticación. | Continuar implementación y validación funcional sobre la arquitectura aprobada, sin atribuir despliegues o pruebas no demostrados. |
| Ajustes documentales realizados | Se recuperan HU-03, puntos y prioridades originales; se corrigen esquema, rutas y estados declarados. Se identifica a Dante como único integrante actual. | Mantener una única base coherente en Markdown, Word y Excel. Registrar los acuerdos de replanificación cuando se obtengan. |
| Actividades pendientes y reprogramación propuesta | HU-01, HU-02, HU-03 y HU-08 tienen código por validar o completar. HU-04 a HU-07, HU-09 y HU-10 no tienen implementación identificada. | Priorizar la validación del recorrido de bodega; luego acordar secuencia de terreno, offline, evidencias, semáforo y reportería. Fechas por confirmar. |

La siguiente secuencia es una propuesta de trabajo: confirmar alcance y modalidad de evaluación; estimar y actualizar el plan; validar identificación, OT, pesaje, despacho, retorno y permisos; corregir defectos; completar evidencias; y acordar luego el desarrollo de terreno, offline y reportería. La fecha exacta de entrega de semana 9 debe confirmarse en Aula Virtual o con el docente.

## 9 Individual conclusions

The current repository provides an implementation baseline for warehouse operations and access control. Source inspection supports the existence of code for inventory, local work orders, dispatch, returns, movement history, and role-based authentication. It does not establish runtime correctness, completed ERP integration, or user acceptance.

As the sole current student member, I am responsible for continuing the project, validating the implemented flow, and documenting the remaining work. Historical contributions retain their original attribution. The next delivery should link every completion claim to reproducible evidence and agreed acceptance criteria.

## 10 Individual reflection

Continuing AFRISAN individually requires me to review the inherited code, understand its limitations, and plan work according to my available capacity. I need to be able to explain the data model, authentication flow, business rules, and results of the validations I execute.

Preparing this report highlights the distinction between a feature present in source code and a feature demonstrated through testing and acceptance. My next task is to gather that evidence, record defects and corrections, and agree on a realistic delivery scope with the client and instructor. This reflection describes my current responsibilities and planned actions; it does not claim unrecorded test results or achievements.

## 11 Fuentes y referencias

Duoc UC. Guía 2.4 de Desarrollo Proyecto APT, Fase 2; instrumento 2.5 de Evaluación Sumativa de Avance Fase 2; guía 2.6 de Informe Final Proyecto APT. Documentos proporcionados en la raíz del repositorio. La pauta de avance indica 20% de ponderación y semana 9, sin fecha de calendario.

Product Backlog de referencia: EVIDENCIAS_GRUPALES/Documentación del Proyecto/ACTUAL/01_Product_Backlog_AFRISAN_Corregido.xlsx. Se conservan sus identificadores, puntos, prioridades y criterios de aceptación como base histórica.

Fuentes de implementación: Afrisan.Api/Models, Afrisan.Api/Data, Afrisan.Api/Controllers, Afrisan.Api/Migrations; Afrisan.Web/Pages, Afrisan.Web/Services y archivos de proyecto .csproj. Versión inspeccionada: commit c2a7bfd y archivos documentales locales de EVA 2.

## 12 Anexo de operaciones principales

Las rutas siguientes corresponden al código actual. Los parámetros entre llaves deben sustituirse por identificadores o códigos existentes. La gestión de OT es local; no se presenta como consulta a Defontana.

| Método y ruta | Propósito | Permiso |
| --- | --- | --- |
| POST /api/Auth/login | Validar credenciales y emitir JWT. | Sin token previo |
| GET /api/Auth/perfil | Consultar claims del usuario autenticado. | Autenticado |
| GET /api/Cilindros | Listar inventario; sin paginación implementada. | Administrador, Jefatura, Bodega |
| GET /api/Cilindros/qr/{codigoQr} | Consultar cilindro por código. | Los cuatro roles |
| GET /api/GasRefrigerantes | Listar catálogo implementado de gases. | Administrador, Jefatura, Bodega |
| GET /api/OrdenesTrabajo | Listar OT locales, activas e inactivas. | Administrador, Jefatura, Bodega |
| GET /api/OrdenesTrabajo/activas | Listar solo OT locales activas. | Administrador, Jefatura, Bodega |
| POST /api/OrdenesTrabajo | Crear OT local con código generado por la API. | Administrador, Bodega |
| POST /api/MovimientosInventario/despacho/{cilindroId} | Registrar despacho y pesaje con OT activa. | Administrador, Bodega |
| POST /api/MovimientosInventario/retorno/{cilindroId} | Registrar retorno; estado final Disponible. | Administrador, Bodega |
| GET /api/MovimientosInventario/historial/{cilindroId} | Consultar movimientos. | Los cuatro roles |
| GET /api/MovimientosInventario/balance/{cilindroId} | Comparar último retorno con despacho anterior. | Administrador, Jefatura, Bodega |
| POST /api/Usuarios | Crear usuario y asignar rol. | Administrador |

## 13 Anexo de validación propuesta

Esta matriz se diseña en la corrección documental. Todos los casos están pendientes de ejecución. Al realizarlos se debe agregar fecha, entorno, datos de prueba, resultado observado y ruta de evidencia. CP-08 comprueba un riesgo que no se considera resuelto por la sola validación de estado.

| Caso | Operación y condición | Resultado esperado | Resultado observado | Estado |
| --- | --- | --- | --- | --- |
| CP-01 | Login con cuenta activa y credenciales válidas. | Token y acceso según rol. | Sin registro | Pendiente de ejecución |
| CP-02 | Login con contraseña incorrecta. | Rechazo sin emitir token. | Sin registro | Pendiente de ejecución |
| CP-03 | Acceder a recurso protegido con token ausente o expirado. | Rechazo por falta de autenticación. | Sin registro | Pendiente de ejecución |
| CP-04 | Técnico intenta despachar un cilindro. | Operación denegada por rol. | Sin registro | Pendiente de ejecución |
| CP-05 | Bodega despacha cilindro disponible con OT activa y peso válido. | Un movimiento persistido y estado Despachado. | Sin registro | Pendiente de ejecución |
| CP-06 | Despacho con OT inexistente o inactiva. | Rechazo sin crear movimiento. | Sin registro | Pendiente de ejecución |
| CP-07 | Despacho con peso menor o igual a tara, o mayor a tara más capacidad. | Rechazo de cada variante sin alterar datos. | Sin registro | Pendiente de ejecución |
| CP-08 | Dos solicitudes de despacho simultáneas del mismo cilindro. | Solo un despacho aceptado; analizar y corregir si ambas se guardan. | Sin registro | Pendiente de ejecución |
| CP-09 | Retorno de cilindro despachado con peso válido; repetir con estado no despachado. | Primer retorno persistido y estado Disponible; segunda operación rechazada. | Sin registro | Pendiente de ejecución |
| CP-10 | Consultar historial y balance tras despacho y retorno; repetir sin retorno. | Datos coinciden con los registros; sin retorno no se informa balance cerrado. | Sin registro | Pendiente de ejecución |
