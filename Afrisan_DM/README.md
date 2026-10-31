# AFRISAN DM

Documentación actualizada de las Fases 1 y 2 del Proyecto APT, basada en el nuevo AFRISAN. Responsable actual: **Dante Moya**. Revisión documental en español: **29 de octubre de 2026**.

Verificación final y empaquetado: **29 de octubre de 2026**. El calendario mantiene la referencia informada el día 28 y los resultados técnicos conservan sus fechas originales.

El paquete contiene **10 documentos Word y 6 planillas Excel actualizados**, un diagrama de arquitectura y los antecedentes utilizados. Los originales del proyecto se conservan en sus carpetas anteriores.

## Por dónde comenzar

1. Revisar el [informe de avance de Fase 2](FASE_2/02_Informe_de_Avance_Fase_2_Afrisan_DM.docx).
2. Consultar el [plan de ocho columnas y lista de trabajo de Fase 2](FASE_2/01_Plan_de_Trabajo_y_Backlog_Fase_2_Afrisan_DM.xlsx).
3. Revisar el [acta actualizada](FASE_1/00_Acta_de_Constitucion_Afrisan_DM.docx) y la [lista priorizada de requisitos](FASE_1/01_Product_Backlog_Afrisan_DM.xlsx).
4. Consultar [evidencias y pendientes](FASE_2/05_Evidencias_y_Pendientes_Afrisan_DM.xlsx) antes de la entrega académica.

## Fase 1

| Archivo | Contenido actualizado |
| --- | --- |
| 00 Acta de Constitución | Problema, objetivos, alcance, reglas, RF, RNF, historias, riesgos y aprobación. |
| 01 Lista priorizada de requisitos | Diez historias con sus IDs originales, criterios revisados y control de cambios. |
| 02 Plan de actividades del ciclo | Actividades de continuidad individual y evidencia disponible. |
| 03 Plan de Trabajo e Hitos | Plan de siete columnas de la guía, competencias, actividades, recursos, duraciones, responsable y observaciones; hoja adicional de hitos. |
| 04 Carta Gantt | Calendario académico de 18 semanas con tres fases y fechas de referencia; continuidad individual de 22 sesiones y 66 horas estimadas, con fórmulas y dependencias. |
| 05 Informe Fase 1 | Definición del proyecto alineada con el proceso nuevo, resumen, conclusiones y reflexión en español. |
| 06 Plan de Comunicación | Responsables, destinatarios, mecanismos y comunicaciones propuestas. |
| 07 Pauta y Registro de Entrevista | Antecedente histórico, aclaraciones documentadas y preguntas pendientes. |
| 08 Matriz RACI | Responsabilidad de desarrollo y responsabilidad operativa diferenciadas. |
| 09 Revisión de Retrospectivas | Consolidación de los cuatro antecedentes con autoría, sin sustituir las reuniones históricas. |

## Fase 2

| Archivo | Contenido actualizado |
| --- | --- |
| 01 Plan de Trabajo y lista de trabajo | Ocho columnas de la guía, competencias, duraciones vinculadas a la Gantt, estados, observaciones y ajustes. |
| 02 Informe de Avance | Objetivos, metodología, desarrollo, pruebas, límites y próximos incrementos. |
| 03 Monitoreo y Ajustes | Dificultades, facilitadores, modificaciones y acciones pendientes. |
| 04 Informe de Cierre Documental | Desarrollo de los seis apartados de la guía del informe final con los resultados disponibles. El proyecto sigue en desarrollo. |
| 05 Evidencias y Pendientes | Registro de fuentes, alcance probado y validaciones necesarias. |

## Base del nuevo contexto

- El Autorizador crea y asigna la OT; el Técnico registra su retiro desde Bodega con peso y foto de salida.
- El trabajo de terreno contempla reemplazo físico o agregado de cilindros. Se diferencian el instalado y el retirado.
- Bodega recibe y registra su pesaje; después del término técnico y de resolver recepciones pendientes aprueba el trabajo.
- El Autorizador revisa posteriormente las fotografías y registros y realiza el cierre administrativo.
- El peso de terreno y el recibido por Bodega se conservan separados. Una diferencia no determina pérdida ni culpabilidad automáticamente.
- Se conserva .NET 8, Blazor WebAssembly y ASP.NET Core Web API, con PostgreSQL/Supabase como destino de persistencia.

## Resultados y alcance

Las evidencias del **25 de octubre** registran **274 comprobaciones satisfactorias del flujo vigente en memoria**, tres OT con datos sintéticos y un recorrido adicional desde el navegador. El diseño SQL de **31 tablas** tiene **144 comprobaciones locales en PGlite**. Son validaciones distintas y no se suman como escenarios independientes ni cobertura de pruebas unitarias.

La aplicación continúa usando datos centrales en memoria. La validación del SQL no acredita conexión de la API, instalación en Supabase, almacenamiento privado de archivos, usuarios reales, concurrencia real, cámara o funcionamiento sin red. Power BI y Defontana permanecen dentro del trabajo pendiente. Durante esta actualización documental no se repitieron pruebas de la aplicación ni se ejecutó SQL remoto.

## Calendario de referencia y duraciones

El 28 de octubre Dante informó que el curso está en la semana 12 y que comenzó un jueves. Con semanas de jueves a miércoles, se calcula el **6 de agosto de 2026** como inicio de la semana 1, el **22 al 28 de octubre** como semana 12 y el **3 al 9 de diciembre** como semana 18. Estas fechas son una referencia calculada a partir de esa información, no una confirmación docente de la entrega o defensa final.

La distribución Fase 1 en semanas 1–6, Fase 2 en semanas 7–16 y Fase 3 en semanas 17–18 es una propuesta de organización. Los hitos de evaluación formativa en semana 2 y avance en semana 9 proceden de las pautas. Las ventanas del calendario no acreditan ejecución ni cumplimiento de hitos.

Las actividades G01 a G09 suman **22 sesiones de 3 horas, equivalentes a 66 horas estimadas de continuidad**. No son horas ya trabajadas ni 22 días consecutivos. Las actividades históricas conservan sus estimaciones de referencia y señalan que el tiempo real no fue registrado. La implementación de borradores (H-LOCAL, En curso) se distingue del ensayo sin red y cámara (G05/P03, No iniciado).

## Reflexión y validaciones pendientes

Las conclusiones y reflexiones individuales en español están redactadas, vinculadas a los intereses profesionales y actividades documentadas, con citas a sus antecedentes. Se recomienda a Dante revisarlas antes de presentarlas como expresión personal. Las autoevaluaciones individuales históricas se conservan sin asignar notas ni respuestas nuevas.

La actualización no incorpora firmas nuevas ni aceptación del cliente. Falta registrar la adaptación docente de la pauta grupal a continuidad individual y confirmar la fecha de entrega o defensa. Las metas cuantitativas originales se conservan como metas pendientes de validación, sin declararlas cumplidas. La Carta Gantt explicita su calendario de referencia y las estimaciones propuestas.

El [control de correcciones](CONTROL_CORRECCIONES_2026_10_28.md) identifica los cinco puntos corregidos y su ubicación en los documentos.

## Anexos y antecedentes

El [anexo técnico](ANEXOS/01_Arquitectura_y_Modelo_de_Datos_Afrisan_DM.docx) describe arquitectura 4+1, modelo de datos, seguridad y persistencia. El [diagrama](ANEXOS/01_Arquitectura_Actual.png) distingue la aplicación actual de su integración prevista.

`ANEXOS/FUENTES` conserva las pautas originales, documentación de referencia, retrospectivas, evidencias individuales de Dante, acuerdos, fragmentos de código y resultados técnicos utilizados, manteniendo su estructura de carpetas y autoría. El [índice de fuentes](ANEXOS/Indice_de_Fuentes.json) incluye la correspondencia de rutas y las huellas SHA-256 de esas copias. Los archivos de esta carpeta son antecedentes; las versiones documentales actualizadas para revisión están en `FASE_1`, `FASE_2` y el anexo técnico.
