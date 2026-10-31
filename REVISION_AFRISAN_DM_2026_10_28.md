# Revisión de cumplimiento de AFRISAN DM

Fecha de revisión: 28 de octubre de 2026. Documentación revisada: corte del 27 de octubre de 2026.

**Actualización posterior:** los puntos 1 a 5 del resumen comunicado a Dante se corrigieron en la versión documental del 28/10/2026. Véase el [control de correcciones](Afrisan_DM/CONTROL_CORRECCIONES_2026_10_28.md). El análisis que sigue conserva los hallazgos de la versión anterior; no describe por sí solo el estado del paquete revisado. La fecha final de entrega o defensa y la adaptación de modalidad siguen pendientes de confirmación.

## Dictamen

**El paquete está alineado con el contexto principal del nuevo AFRISAN y cumple parcialmente las pautas académicas. Todavía requiere correcciones para presentarlo como una entrega completa.** Es una base de avance con evidencia técnica identificable. El documento preparado con estructura de informe final sigue siendo provisional y no demuestra cierre del proyecto.

La revisión anterior de preparación verificó formato, archivos y coherencia general. Esta revisión adicional de las exigencias académicas detecta omisiones de contenido y planificación que deben corregirse. La existencia de los documentos no equivale a cumplir todos los campos solicitados.

Se inspeccionaron los 10 Word y las 6 planillas Excel actuales, las guías y pauta originales, los acuerdos de negocio, la configuración actual de la API y los resultados conservados de pruebas. Se comprobó que las 53 copias de fuentes coinciden con sus originales mediante SHA-256 y que el ZIP contiene la misma versión de los archivos. No se ejecutaron nuevas pruebas funcionales ni se consultó Supabase. Los documentos y el ZIP evaluados no fueron modificados durante esta auditoría.

## Resultado por ámbito

| Ámbito | Resultado | Fundamento |
| --- | --- | --- |
| Contexto principal del caso | Alineado | Retiro por Técnico, reemplazo físico, recepción y aprobación de Bodega, revisión posterior del Autorizador y cierre. |
| Fase 1 | Cumplimiento parcial | Problema, objetivos, pertinencia, metodología y recursos están presentes. Falta el plan con las siete columnas de la guía y la temporalidad académica de las tres fases. |
| Fase 2 de avance | Cumplimiento parcial | Hay estructura, abstract bilingüe, ajustes, evidencia y ocho columnas. Faltan reflexión terminada, duraciones y mejor citación; hay un estado inconsistente. |
| Informe final de Fase 2 | Provisional | Incluye los apartados de la guía, pero deja resultados finales, reflexión y validaciones por completar. |
| Evidencias y entrega por Git | Pendiente para este paquete | `git status` muestra `?? Afrisan_DM/` y `?? Afrisan_DM.zip`; `git ls-files -- Afrisan_DM Afrisan_DM.zip` no devuelve archivos. |

Estos resultados son una revisión documental, no una calificación docente ni una estimación numérica de la nota.

## Hallazgos prioritarios

### 1 Plan de Fase 1 incompleto respecto de la guía

**Exigencia:** la sección 7 de la [guía de Fase 1](<EVIDENCIAS_GRUPALES/1.5_GuiaEstudiante_Fase 1_Definicion Proyecto APT (1).docx>) incluye siete columnas: competencia o unidad, nombre de actividad, descripción de actividad, recursos, duración, responsable y observaciones.

**Observado:** el [Plan de Trabajo e Hitos](Afrisan_DM/FASE_1/03_Plan_de_Trabajo_e_Hitos_Afrisan_DM.xlsx), hoja `Hitos`, A5:E5, presenta cinco columnas de hitos. El [Informe Fase 1](Afrisan_DM/FASE_1/05_Informe_Fase_1_Afrisan_DM.docx), sección 11, resume actividades y evidencias en una tabla de cuatro columnas. El plan de Fase 2 tiene otra estructura y no sustituye automáticamente el plan solicitado para Fase 1.

**Impacto:** la propuesta explica el trabajo, pero no entrega el instrumento completo que solicita la guía.

**Corrección:** añadir el plan de siete columnas con actividades del nuevo contexto y Dante como responsable actual. Diferenciar actividades históricas de la continuidad individual y referenciarlo expresamente desde el informe.

### 2 La Carta Gantt no ubica el proyecto en el período académico

**Exigencia:** la sección 8 de la guía de Fase 1 pide organizar las actividades considerando el período asignado y mantener la temporalidad académica de las tres fases.

**Observado:** la [Carta Gantt](Afrisan_DM/FASE_1/04_Carta_Gantt_Afrisan_DM.xlsx) contiene una secuencia propuesta de 22 sesiones y 66 horas. No tiene fechas o semanas académicas ni distribución explícita entre Fases 1, 2 y 3. En la hoja `Hitos` del plan de Fase 1, C9:C12, los hitos futuros dicen `Fecha por acordar`.

**Impacto:** es posible entender el orden propuesto de las tareas, pero no comprobar su viabilidad dentro del plazo del curso ni comparar ejecución con una línea temporal vigente.

**Corrección:** incorporar las semanas o fechas académicas confirmadas, las tres fases, las actividades ya realizadas y los hitos pendientes. Mantener cualquier estimación provisional identificada hasta acordar el calendario; no inventar una fecha docente.

### 3 La reflexión de Fase 2 sigue siendo un borrador

**Exigencia:** la pauta 2.5 pide conclusiones individuales y reflexión en inglés. La guía 2.6 pide explicar el aporte del proyecto a los intereses profesionales y las proyecciones laborales.

**Observado:** en el [Informe de Avance](Afrisan_DM/FASE_2/02_Informe_de_Avance_Fase_2_Afrisan_DM.docx), sección 10, aparecen `Proposed wording for Dante Moya to review and personalise before submission` y `For my final reflection, I should explain...`. El [informe con estructura final](Afrisan_DM/FASE_2/04_Informe_de_Cierre_Documental_Fase_2_Afrisan_DM.docx), sección 10, también indica que la reflexión y las preferencias deben confirmarse o completarse.

**Impacto:** el apartado está en el idioma pedido, pero parte de su contenido son instrucciones para redactarlo, no la respuesta personal terminada.

**Corrección:** sustituir esas instrucciones por una reflexión real de Dante: decisiones que cambió, dificultades que resolvió, aprendizajes concretos y proyección profesional. Conservar las conclusiones técnicas respaldadas. No atribuir vivencias o preferencias que Dante no haya confirmado.

### 4 El plan de ocho columnas carece de duraciones

**Exigencia:** la guía 2.4 solicita la duración de cada actividad, junto a recursos, responsable, observaciones, estado y ajustes.

**Observado:** el [Plan de Trabajo y Backlog de Fase 2](Afrisan_DM/FASE_2/01_Plan_de_Trabajo_y_Backlog_Fase_2_Afrisan_DM.xlsx), hoja `Plan de Trabajo 8 Columnas`, D6:D16, utiliza fechas de corte, `Por estimar` o referencias a sesiones sin duración total. D8 identifica la fecha del diseño SQL completado, no el tiempo que tomó. La Gantt contiene estimaciones, pero no existe una correspondencia explícita que complete estas celdas.

**Impacto:** las ocho columnas están presentes, pero la columna de duración no satisface completamente su función. Dificulta monitorear capacidad y desviaciones.

**Corrección:** registrar duraciones estimadas justificadas para lo pendiente y tiempo real cuando exista evidencia para lo ejecutado. Si no se registró el tiempo real, indicarlo sin inventarlo y separar ese dato de la fecha de ejecución. Vincular las actividades y estimaciones entre el plan y la Gantt.

### 5 Un ensayo tiene estados contradictorios

**Observado:** en el plan de Fase 2, B11 es `Ensayar trabajo local y cámara` y G11 indica `En curso`. En [Evidencias y Pendientes](Afrisan_DM/FASE_2/05_Evidencias_y_Pendientes_Afrisan_DM.xlsx), hoja `Pendientes`, registro P03, el ensayo sin red y recuperación figura como `No ejecutado` y `No iniciado`.

**Impacto:** el evaluador puede confundir la implementación de IndexedDB con la ejecución de una prueba física sin red.

**Corrección:** separar las actividades `Implementar borradores locales` y `Ejecutar pruebas sin red y cámara`, con el estado y la evidencia que corresponden a cada una. Unificar la referencia de actividad en ambos archivos.

### 6 Las referencias permiten localizar archivos pero no completan la citación académica

**Exigencia:** IE5 de la pauta 2.5 evalúa redacción, ortografía y normas de citas y referencias.

**Observado:** las secciones finales de los informes enumeran nombres y rutas de archivos como `[1]`, `[2]`, etc. Las afirmaciones del cuerpo no remiten consistentemente a esos números. Faltan referencias presentadas con autor o institución, fecha y título cuando están disponibles.

**Impacto:** hay trazabilidad interna, pero no se acredita el cumplimiento completo de una norma de citación. La pauta inspeccionada no especifica que deba ser APA; no corresponde imponerla como exigencia confirmada.

**Corrección:** usar de manera consistente la norma indicada por el docente. Relacionar en el cuerpo las afirmaciones relevantes con sus fuentes, identificar autor/institución, título y fecha, y conservar la ruta del anexo como localizador adicional.

### 7 El paquete y el flujo actualizado siguen sin registrarse en Git

**Exigencia:** IE6 de la pauta 2.5, ponderado en 20 %, pide documentación y evidencias de avance en Git. IE7 solicita poder identificar aportes dentro del repositorio.

**Observado:** la revisión de Git encontró la carpeta y el ZIP sin seguimiento. El código del flujo nuevo tiene modificaciones locales; el acuerdo de negocio del 25 de octubre, la carpeta de pruebas del flujo corregido y el nuevo SQL también están pendientes de registro. El último commit local consultado fue `e0f5c94`, del 24 de octubre.

**Impacto:** tener el paquete en el disco no acredita su entrega por Git ni la relación de esta versión documental con el código y las pruebas que describe. La revisión local no certifica acceso del docente al repositorio remoto.

**Corrección:** preparar una entrega versionada coherente del código, documentación y evidencias correspondientes y comprobar su disponibilidad para evaluación. Conservar autoría histórica y evitar incluir archivos temporales, secretos o resultados ajenos a la entrega. Esta auditoría no registra ni publica cambios.

### 8 Aplicación individual de la pauta pendiente de acreditar

**Exigencia:** la pauta 2.5 describe equipos de dos o tres integrantes. IE7 e IE8 representan 15 % y 10 % y se refieren a aportes y trabajo en equipo.

**Observado:** los documentos explican que Dante continúa individualmente y conservan las contribuciones históricas. No se encontró en las fuentes inspeccionadas una constancia docente de cómo aplicar esos indicadores a la continuidad individual.

**Impacto:** el paquete puede describir correctamente la situación, pero no resolver por sí mismo el criterio de evaluación. La exposición oral tampoco puede acreditarse mediante una revisión de archivos.

**Corrección:** incorporar el acuerdo docente existente, si lo hay, o dejar registrada su definición cuando se obtenga. No añadir integrantes ficticios ni atribuir aprobación automática al profesor.

## Contraste con el contexto del caso

La [definición vigente del flujo](AFRISAN_NUEVO/01_NEGOCIO/12_FLUJO_TERRENO_DESPACHO_Y_REVISION.md) se refleja en el acta, los informes, el backlog y la RACI:

- El Autorizador prepara y asigna la OT. El Técnico registra su retiro con peso y fotografía.
- El reemplazo distingue cilindro retirado e instalado. Un agregado no exige inventar un retirado.
- Informar la devolución no equivale a recepción. Bodega recibe y conserva su pesaje; su aprobación requiere término técnico y pendientes resueltos.
- El Autorizador revisa después de Bodega y antes del cierre administrativo.
- Los pesos de terreno y recepción se conservan por separado. No se deduce consumo o emisión mediante una resta entre cilindros diferentes.
- Se mantiene la prioridad de Jefatura sobre límites; el cómputo de kilos y otros detalles todavía necesitan definición.
- Power BI y Defontana permanecen en el alcance pendiente. La arquitectura conserva .NET 8, Blazor WebAssembly y ASP.NET Core Web API.

La [API actual](AFRISAN_NUEVO/03_CODIGO/Afrisan.Nuevo.Api/Program.cs) confirma stores en memoria y cuentas de revisión. Es coherente que el paquete no afirme integración operativa con Supabase.

Las [evidencias del flujo corregido](AFRISAN_NUEVO/04_SEGUIMIENTO/FLUJO_TERRENO_2026_10_25/README.md) respaldan las 274 comprobaciones y el recorrido adicional de navegador. Las [evidencias SQL](AFRISAN_NUEVO/04_SEGUIMIENTO/VALIDACION_SQL_CONTEXTO_2026_10_25/README.md) respaldan 144 comprobaciones locales sobre 31 tablas. Se presentan correctamente como ensayos distintos, con datos sintéticos y limitaciones explícitas. La revisión no detectó una contradicción material en la secuencia operativa principal descrita.

## Lectura de los ocho indicadores de Fase 2

| Indicador | Peso | Evidencia documental y límite |
| --- | ---: | --- |
| IE1 Ajustes justificados | 10 % | Hay explicación del reemplazo físico, cambios de responsables y continuidad individual. No se acredita una cadena completa de retroalimentación docente y respuestas. |
| IE2 Aplicación de metodología | 10 % | Se describe y respalda un ciclo de reglas, implementación, pruebas y correcciones. La planificación necesita duraciones y referencia temporal para valorar objetivos del avance. |
| IE3 Evidencias de documentación, programación y datos | 25 % | Hay documentos, código referenciado, resultados y SQL local. La falta de persistencia integrada limita el avance demostrado, pero no permite declarar incumplimiento automático sin el alcance acordado para esta evaluación. |
| IE4 Lenguaje técnico | 5 % | En general preciso: distingue envases, pesajes, estados, almacenamiento temporal y validación local. |
| IE5 Redacción, citas y referencias | 5 % | Parcial por reflexión sin finalizar y citación interna insuficientemente desarrollada. |
| IE6 Estructura, nombres y Git | 20 % | Paquete organizado y estructura técnica presente; esta versión todavía no está registrada en Git. No se encontró una convención adicional de nombres que permita certificar todos los requisitos de nombrado. |
| IE7 Aportes de integrantes | 15 % | Se conserva autoría histórica, pero falta registrar esta entrega y acreditar la adaptación individual de la pauta. |
| IE8 Trabajo en equipo y exposición | 10 % | Requiere definición docente para modalidad individual y observación de la presentación; no verificable solo con estos archivos. |

Fuente: [Pauta de evaluación de avance de Fase 2](<2.5_APT122_Sumativa Avance Fase2.pdf>), páginas 1, 2 y 4 a 8. La tabla describe cobertura y límites, sin asignar los niveles ni la nota del docente.

## Condición del informe final

La [guía 2.6](<2.6_GuiaEstudiante_Fase 2_Informe Final Proyecto APT.docx>) pide relevancia, objetivos, metodología, desarrollo, evidencias e intereses y proyecciones profesionales. Los apartados aparecen en el documento preparado, pero algunos resultados y la reflexión personal permanecen abiertos. Por eso puede usarse como documento de trabajo para el cierre, no como prueba de que el proyecto haya terminado.

Persistencia, identidad real, fotos durables, pruebas sin red/cámara y Power BI son pendientes del proyecto declarados por los propios documentos. La pauta de avance no exige por sí sola que todos estén terminados en este corte. Para el cierre habrá que contrastar el alcance final acordado con los resultados reales y documentar cualquier ajuste aceptado.

## Orden de corrección recomendado

1. Completar el plan de siete columnas de Fase 1.
2. Vincular Gantt, hitos y plan de ocho columnas con duraciones y calendario académico confirmado.
3. Unificar los estados de actividades y separar implementación de pruebas pendientes.
4. Terminar la reflexión personal en inglés y mejorar citas y referencias.
5. Incorporar la definición docente de continuidad individual y relacionar las evidencias con los indicadores seleccionados.
6. Preparar y verificar la entrega por Git del conjunto coherente de código, documentos y pruebas.

Después de estas correcciones se puede reevaluar el paquete como entrega de avance. La terminación del sistema y la aceptación del informe final requieren evidencia adicional acorde con el alcance acordado.
