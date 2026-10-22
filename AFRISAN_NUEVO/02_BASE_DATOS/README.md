# Base propuesta para la aplicación nueva

Preparado el 19 de octubre de 2026 para Dante Moya.

## Archivo a ejecutar

### Base vacía, sin poblar tablas (versión solicitada más reciente)

Usa **03_Crear_Tablas_Sin_Datos.sql** si eliminaste los esquemas anteriores.
Ejecuta únicamente ese archivo, completo, en SQL Editor. Crea **afrisan** y
**afrisan_bi** con 16 tablas vacías, relaciones, validaciones, funciones,
triggers y vistas. No inserta roles, usuarios, catálogos ni datos simulados.
Los INSERT dentro de las funciones de auditoría se ejecutarán solamente ante
operaciones futuras; no cargan datos durante la instalación.

Si persiste un esquema afrisan/afrisan_bi, CREATE SCHEMA detendrá el script:
el archivo no borra ni reemplaza objetos existentes. La aplicación requerirá
aprovisionar roles y usuarios posteriormente para poder iniciar sesión.

Los archivos 01 y 02 se conservan como versiones anteriores. No son pasos
adicionales que deban ejecutarse después del 03.

**Si apareció `schema "afrisan" already exists`, usa en su lugar
`02_MER_Completo_Afrisan_Nuevo.sql`.** Es el script completo de instalación en
los esquemas separados **afrisan_nuevo** y **afrisan_bi_nuevo**. No borra ni
actualiza los esquemas anteriores. No ejecutes ambos archivos para una misma
aplicación: son alternativas de instalación, no pasos consecutivos.

El archivo 02 admite que esos esquemas nuevos existan vacíos; si contienen tablas,
vistas, secuencias o funciones, se detiene para no ocultar diferencias. Después
de ejecutarlo, selecciona **afrisan_nuevo** en el editor/visualizador de tablas.
Las instrucciones de unidades, autenticación y límites de este documento siguen
siendo aplicables sustituyendo el esquema por afrisan_nuevo y el de BI por
afrisan_bi_nuevo. El contexto de auditoría del archivo 02 se llama
`afrisan_nuevo.id_usuario`.

`01_Crear_Afrisan_Supabase.sql`: copia **todo** el contenido en SQL Editor de
Supabase y ejecútalo como propietario. Está envuelto en una transacción.

El archivo crea los esquemas **afrisan** y **afrisan_bi**. Las tablas manuales
existentes en public se conservan, sin copiar sus datos. Si alguno de los dos
esquemas ya existe, se detiene; no añade DROP ni omite diferencias silenciosamente.
Tras un error, si tu sesión conserva una transacción abortada, ejecuta ROLLBACK
antes de corregir el problema y volver a ejecutar el archivo completo.

En Table Editor/Schema Visualizer selecciona **afrisan**, no public, para ver
las nuevas tablas. No añadas estos esquemas a los expuestos por Data API todavía.

**No se ha ejecutado en tu proyecto Supabase ni se ha probado contra PostgreSQL.**
Es una propuesta de instalación para la aplicación nueva, no una confirmación de
despliegue ni una migración compatible con el modelo EF Core anterior.

## Nombres y unidades

Se mantienen Usuario, Rol, Cliente, Sucursales, Refrigerantes, Cilindros,
OrdenTrabajo, Movimientos y Costos con nombres cercanos a tu captura.
Los nombres llevan comillas y distinguen mayúsculas/minúsculas, por ejemplo:

```sql
select * from afrisan."Cilindros";
select * from afrisan."BalanceCilindro";
select * from afrisan."EstadoCilindros";
```

- `Salida = true`: despacho; `false`: retorno. El retorno lleva `Id_Despacho`.
- Los IDs de cada FK usan el mismo tipo que su PK.
- Pesos en gramos enteros: **10 kg = 10000 g**. La interfaz puede mostrar kg.
- `Capacidadgr` es masa máxima de gas; `TaraGr` es masa del envase vacío.
- `PesoGrBruto` es lectura total de balanza. `PesoRegistradoGr` se deriva del
  último movimiento en EstadoCilindros; ContenidoEstimadoGr descuenta las cargas.
- `CilindroQR` conserva el código identificador, no el archivo de imagen.
- Las fotos son rutas de objetos en Storage privado, no bytea.
- Las fechas con hora son timestamptz. El agrupamiento mensual usa America/Santiago.
- Precios por gramo con ocho decimales: **30000 CLP/kg = 30 CLP/g**.
  La aplicación convierte los precios al mostrarlos o ingresarlos en kg.
- Costos pertenece a una intervención. PrecioTotal se calcula con gramos y
  tarifa aplicada, más PrecioCilindro solo si hay un cargo de envase acordado.
- Una OT pertenece a una sucursal en esta propuesta; si el cliente maneja OT que
  abarcan varias sucursales, se debe modificar esa relación antes de usarla.

## Cobertura de los requisitos

Fuente funcional: `EVIDENCIAS_GRUPALES/Documentación del Proyecto/ACTUAL/01_Product_Backlog_AFRISAN_Corregido.xlsx`,
hojas Historias de Usuario y Criterios de Aceptación. No se inventan valores GWP,
precios, causas de falla, cuentas de usuario ni credenciales.

| Historia | Estructura y reglas incluidas |
| --- | --- |
| HU-01 / HU-02 | QR único, referencia de OT externa, múltiples cilindros por OT, tara, capacidad, pesajes, técnico custodio y usuario registrador. |
| HU-03 | Un rol por usuario; hash de contraseña, cuenta activa y campos de bloqueo. Sin tabla de permisos. |
| HU-04 | UUID único de operación en movimientos e intervenciones; dispositivo, creación local y recepción. |
| HU-05 | Intervención por equipo, sucursal, causa, técnico, cilindro y despacho exacto; validación de OT y gas compatibles. |
| HU-06 | Foto y peso restante exigidos para cerrar intervención. Campos para revisor y fecha de revisión. |
| HU-07 | Verde menor a 5%, amarillo entre 5% y 15%, rojo mayor a 15% o nueva carga en menos de 30 días. Rojo exige plan. Vista mensual de sucursales con más de tres críticas. |
| HU-08 | Un retorno por despacho; consumo declarado y descalce físico; conforme solo si valor absoluto menor a 200 g. Bloqueo de cierre de OT con pendientes o descalces. |
| HU-09 | Certificado con folio, entrega, documento y lote de cilindros clasificados como chatarra. |
| HU-10 | Tarifas por vigencia, costos aplicados, GWP con fuente/metodología, emisiones estimadas y vistas para BI. |

Auditoria es una decisión de implementación para respaldar cambios: no se
presenta como una historia de usuario adicional. Se registran INSERT/UPDATE/DELETE
con datos anteriores/nuevos y se excluyen hashes y VersionSeguridad.

## Autenticación y acceso

La propuesta continúa con **autenticación desde la API**, como indica HU-03.
Usuario no se vincula automáticamente a Supabase Auth; si se decide usar ese
servicio, hay que cambiar este diseño antes de integrarlo.

`Clave_Usuario` recibe únicamente un hash creado por la API con un mecanismo
como PasswordHasher. No insertar una contraseña escrita directamente en SQL.
El primer administrador se aprovisiona desde un proceso de backend.

Todas las tablas tienen RLS activado y no hay políticas ni permisos para anon o
authenticated. Un esquema personalizado tampoco se publica automáticamente en
Data API. La aplicación nueva accederá desde su API, usando una identidad
PostgreSQL de servidor configurada por separado. Nunca poner esas credenciales
en Blazor WebAssembly, JavaScript ni el repositorio.

La API debe comprobar rol, usuario activo y sesión; obtener el Id_Usuario del
token verificado; y establecer el actor de auditoría con una consulta parametrizada
en la misma transacción:

```sql
select set_config('afrisan.id_usuario', :id_usuario_autenticado::text, true);
```

Las escrituras hechas directamente en SQL Editor sin ese contexto conservan el
usuario de PostgreSQL y un Id_Usuario nulo. El propietario puede administrar
triggers y datos: la auditoría no es un mecanismo contra el propietario de BD.

### Power BI

El esquema afrisan_bi ofrece ConsumoIntervenciones, Conciliacion y
SucursalesCriticasMes. No incluye claves ni fotografías privadas. Usa una
conexión PostgreSQL de solo lectura dedicada y otorga acceso únicamente a esas
vistas, cuando se defina su identidad. Por ejemplo, sustituyendo el nombre real:

```sql
-- Ejemplo, NO forma parte del script de instalación:
-- GRANT USAGE ON SCHEMA afrisan_bi TO lector_bi_real;
-- GRANT SELECT ON ALL TABLES IN SCHEMA afrisan_bi TO lector_bi_real;
```

Estas vistas usan los permisos de su propietario sobre las tablas subyacentes:
se diseñan para un lector BI confiable que puede ver todas las sucursales.
No otorgar acceso al esquema afrisan ni a sus tablas a ese lector. Si se necesita
segmentación por cliente, diseñar las vistas/políticas antes de conceder acceso.
No sumar importes de monedas distintas ni convertir emisiones desconocidas en
cero; las vistas distinguen falta de valoración y estimación.

## Responsabilidades pendientes de la aplicación

La instalación prepara la persistencia y algunas reglas de integridad. Sigue
siendo necesario implementar:

- API, validación de sesiones/roles, emisión y revocación de tokens y bloqueo.
- Aprovisionamiento seguro de usuarios y permisos del usuario técnico de BD.
- Importación/integración Defontana y validación contra el ERP.
- Captura de fotos, bucket privado y comprobación real de existencia/acceso
  del archivo: una ruta no acredita por sí misma una fotografía válida.
- Persistencia local offline, sincronización, reintentos y conflictos. Un UUID
  repetido debe devolver la operación existente si su contenido coincide.
- Reconciliación de semáforos cuando llegan cargas antiguas offline: el trigger
  considera las cargas anteriores ya presentes, no recalcula trabajos futuros
  recibidos antes. No se permite insertar cargas después del retorno sin un
  procedimiento de regularización; sincronizar antes del cierre de custodia.
- Selección de tarifa vigente por fecha/moneda y copiado de su precio a Costos.
  Los triggers no eligen tarifas ni inventan factores GWP.
- Metodología para estimar masa emitida: refrigerante utilizado no equivale
  automáticamente a refrigerante emitido. Sin estimación/factor, CO2e queda NULL.
- Resolución de descalces y rectificaciones auditadas. La propuesta bloquea
  edición/borrado de pesajes y edición de trabajos cerrados. No existe todavía
  un procedimiento de ajuste: un descalce se mantiene visible y bloquea cierre.
- Definición adicional de «sobreconsumo recurrente mensual» de sucursal.
- Interfaz de alertas, revisión, reportes y configuración de Power BI.

No se incluye una tabla Permisos ni se modifica la aplicación actual.
