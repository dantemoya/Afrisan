# Matriz de validación propuesta

Proyecto: Sistema Integral de Gestión y Trazabilidad de Refrigerantes AFRISAN
Integrante actual y responsable de continuidad: Dante Moya
Fecha de corte documental: 16 de octubre de 2026

Los diez casos siguientes se diseñan en esta corrección documental. Ninguno ha sido ejecutado para este informe. Al ejecutar, agregar fecha, entorno, datos de prueba, resultado observado, archivo de evidencia y referencia a defectos o correcciones.

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
