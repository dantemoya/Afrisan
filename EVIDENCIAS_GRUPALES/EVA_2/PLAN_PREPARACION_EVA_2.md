# Preparación de EVA 2 de AFRISAN

Fecha de revisión: 16 de octubre de 2026.

Único integrante actual y responsable de continuidad: Dante Moya. La aplicación individual de la pauta grupal debe confirmarse con el docente. Las contribuciones previas conservan su autoría.

- [x] Arquitectura aprobada, según confirmación del estudiante recibida el 16 de octubre de 2026. Continuar implementación y validación funcional sobre esta base.

Este documento organiza la preparación del informe de avance. Distingue las exigencias de los archivos entregados de las recomendaciones para AFRISAN. Las tareas sin marcar están pendientes de ejecución o de acreditación; no constituyen una evaluación de funcionamiento del sistema.

## Documentos que regulan la entrega

- `2.4_GuiaEstudiante_Fase 2_DesarrolloProyecto APT.docx`: contenidos de avance, monitoreo del plan y ajustes.
- `2.5_APT122_Sumativa Avance Fase2.pdf`: instrucciones, formato y rúbrica de evaluación del informe de avance.
- `2.6_GuiaEstudiante_Fase 2_Informe Final Proyecto APT.docx`: contenidos del informe final. Es un documento distinto del informe de avance; confirmar con el docente si solicita incorporarlo en esta misma entrega.

La pauta 2.5 indica entrega en semana 9 y ponderación de 20% sobre la nota final. No establece una fecha de calendario, un porcentaje mínimo de funcionalidades ni una cantidad de historias de usuario que deban estar terminadas. El avance se contrasta con la planificación y los ajustes justificados.

## Condiciones que se deben confirmar

- [ ] Confirmar fecha y hora de entrega de la semana 9 en Aula Virtual o con el docente.
- [ ] Acordar con el docente la continuidad individual: la pauta indica grupos de 2 a 3 estudiantes y asigna 25% a aporte individual y trabajo en equipo. El apoyo de IA no constituye un segundo estudiante.
- [ ] Confirmar cómo se aplicarán los indicadores 7 y 8 a la nueva composición del equipo.
- [ ] Confirmar nombres, estructura de carpetas, formato de archivo y medio de entrega exigidos por el docente. Los tres archivos no especifican nombres completos para cada evidencia.
- [ ] Confirmar si se exige presentación, demostración y/o material adicional, y sus tiempos. La rúbrica evalúa dominio al exponer, pero no fija una duración ni exige expresamente diapositivas o video.
- [ ] Confirmar qué indicadores disciplinarios seleccionará el docente: pruebas, gestión del proyecto, modelos de datos, construcción, integración e implantación.

## Rúbrica del informe de avance

| Indicador | Peso | Qué preparar |
| --- | ---: | --- |
| 1. Ajustes justificados | 10% | Dificultades, facilitadores, retroalimentación y decisiones de ajuste. |
| 2. Metodología aplicada | 10% | Evidencias de cómo se trabajó y cómo permitió cumplir los objetivos del avance. |
| 3. Evidencias de documentación, programación y almacenamiento de datos | 25% | Productos comprobables, coherentes con la planificación y con calidad técnica. |
| 4. Lenguaje técnico preciso | 5% | Explicaciones correctas del funcionamiento, arquitectura, datos y límites del avance. |
| 5. Redacción, ortografía y referencias | 5% | Revisión del texto completo y citas de las fuentes utilizadas. |
| 6. Documentación y evidencias organizadas en Git | 20% | Archivos requeridos, nombres y estructura acordados, y evidencias versionadas. |
| 7. Aporte de cada estudiante y equidad | 15% | Trazabilidad real de contribuciones y acuerdo docente por continuidad individual. |
| 8. Trabajo en equipo y dominio del tema | 10% | Explicación y participación demostrables; aplicación acordada con el docente. |

## Contenido del informe de avance

- [ ] Portada e índice.
- [ ] Resumen en español y abstract en inglés.
- [ ] Resumen de actividades ejecutadas y objetivos específicos alcanzados, explicando cómo se alcanzaron.
- [ ] Objetivos actualizados cuando hayan cambiado. Si siguen vigentes, indicar su relación con el avance presentado.
- [ ] Metodología aplicada y su pertinencia. Si cambió, documentar el ajuste y su justificación.
- [ ] Desarrollo de ingeniería: componentes construidos, integración, datos, decisiones técnicas y resultados observados.
- [ ] Evidencias seleccionadas y explicación de qué avance demuestra cada una y cómo se resguardó su calidad.
- [ ] Plan de trabajo actualizado con las ocho columnas de la guía 2.4: competencia o unidad de competencia, actividades, recursos, duración, responsable, observaciones, estado de avance y ajustes.
- [ ] Utilizar los estados indicados por la guía: En curso, Con retraso, No iniciado, Completado o Ajustada.
- [ ] Factores facilitadores y dificultades; acciones ya tomadas y acciones previstas.
- [ ] Actividades ajustadas o eliminadas y justificación. Si no se ajustó el plan, justificar su continuidad.
- [ ] Actividades no iniciadas o retrasadas, motivos y estrategia para avanzar.
- [ ] Conclusiones individuales solo en inglés.
- [ ] Reflexión individual solo en inglés.
- [ ] Bibliografía o fuentes cuando corresponda y anexos de evidencia cuando corresponda.

Formato de la pauta: Arial, Verdana o Calibri; tamaño 11 o 12; interlineado 1,0 o 1,5 según corresponda; páginas numeradas; lenguaje técnico preciso; ortografía y referencias revisadas. La pauta no identifica un estilo de citación concreto.

## Evidencias recomendadas para AFRISAN

Estos elementos son una propuesta de preparación para cubrir la rúbrica, no una lista adicional de requisitos impuesta por los tres archivos.

- [ ] Actualizar Product Backlog, Sprint Backlog y plan de trabajo según el avance real. Resolver la diferencia entre los estados documentales y el código existente.
- [ ] Reprogramar actividades, responsables y fechas por la salida de integrantes. Mantener las contribuciones históricas reales y registrar la transición desde su fecha efectiva.
- [ ] Registrar los acuerdos y prioridades de la reunión con el cliente y la retroalimentación del docente.
- [ ] Preparar un inventario de evidencias: identificador, objetivo/HU, descripción, fecha, archivo o ruta, resultado, autor y commit cuando corresponda.
- [ ] Mostrar la versión actual de la arquitectura y del modelo de datos, coherentes con las entidades y migraciones implementadas.
- [ ] Documentar cómo se ejecutan la web y la API y cómo se prepara un entorno de demostración con datos de prueba.
- [ ] Capturar pantallas del sistema funcionando y resultados de operaciones de API, con datos de prueba y sin mostrar contraseñas o claves.
- [ ] Mostrar datos persistidos después de registrar y consultar operaciones, no solo formularios completados.
- [ ] Diseñar y ejecutar casos de validación relevantes; guardar resultado esperado, resultado observado, fecha, evidencia, defectos y correcciones realizadas. La pauta permite que el docente seleccione indicadores de diseño y ejecución de pruebas; no exige un framework específico ni exclusivamente pruebas automatizadas.
- [ ] Registrar contribuciones con autoría real y preparar la explicación del trabajo que cada estudiante efectivamente realizó.
- [ ] Guardar y versionar en Git el informe y las evidencias conforme al acuerdo de entrega. Al comenzar esta revisión, los tres archivos nuevos todavía estaban sin seguimiento en Git.

## Alcance técnico recomendado para el avance

La revisión previa del repositorio encontró código para cilindros, gases, órdenes de trabajo, despacho, retorno, historial, balance básico y acceso por roles. Esto acredita implementación en código; falta acreditar su ejecución y aceptación con evidencias.

Propongo concentrar la demostración en el recorrido: iniciar sesión, identificar cilindro, seleccionar OT activa, registrar peso y despacho, registrar retorno, consultar historial y revisar el resultado del balance. Este alcance deberá coincidir con los objetivos, los compromisos con el cliente y el plan actualizado.

- [ ] Confirmar el método de lectura QR requerido y demostrarlo: consulta por código, lector físico o cámara, según el alcance acordado.
- [ ] Confirmar cómo se obtendrán las OT de Defontana. No presentar las órdenes locales existentes como integración ERP terminada.
- [ ] Completar la relación entre el retorno, el despacho y la OT para dar trazabilidad al ciclo.
- [ ] Definir y aplicar las reglas de pesos y las excepciones de retorno.
- [ ] Distinguir el balance básico existente de la conciliación completa: HU-08 requiere comparar salida menos retorno con kilos declarados en terreno, aplicar tolerancia y gestionar descalces y cierre. Si faltan esos datos o acciones, mantener HU-08 como parcial o acordar formalmente su ajuste.
- [ ] Definir si las pantallas de gestión de OT y usuarios son necesarias para la demostración acordada y completarlas si corresponde.
- [ ] Verificar los accesos permitidos y denegados de cada rol.
- [ ] Probar casos como OT inactiva, cilindro ya despachado, peso menor o igual a tara, peso sobre capacidad, retorno de cilindro no despachado y falta de autorización.
- [ ] Mantener las funciones aún no implementadas con su estado real: intervenciones en terreno, offline y sincronización, fotografías, semáforo, certificados Regener y reportería. No declararlas completadas.

## Orden de ejecución propuesto

1. Confirmar modalidad individual, plazo y criterios de aplicación con el docente.
2. Registrar acuerdos del cliente y reprogramar el alcance de la entrega.
3. Actualizar el plan, responsables, backlog y estados del avance.
4. Completar y comprobar el recorrido funcional elegido; corregir los defectos encontrados.
5. Reunir evidencias de documentación, software y almacenamiento de datos.
6. Redactar el informe con los contenidos de 2.4 y el formato exigido por 2.5.
7. Revisar idiomas, referencias, consistencia entre afirmaciones y evidencias y organización en Git.
8. Preparar la explicación y demostración; verificar el paquete de entrega contra los ocho indicadores.

## Informe final de la guía 2.6

Conservar para el cierre de la fase: nombre del proyecto, áreas de desempeño, competencias, relevancia del problema y aporte de valor, objetivos general y específicos, metodología y justificación, etapas y desarrollo, dificultades y facilitadores, ajustes, evidencias y resultados finales, intereses y proyecciones profesionales. Su fecha y ponderación no aparecen en esa guía. Confirmar su exigencia concreta con el docente.
