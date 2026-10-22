# Módulo de gráficos con Power BI

Fecha: 20 de octubre de 2026.

## Alcance confirmado

Dante recordó expresamente que la aplicación debe incluir un módulo de gráficos
para Power BI. Se incorpora al alcance de la nueva aplicación; sigue pendiente
de implementación y no se sustituye por un listado de datos.

## Contenido para revisar

Propuesta de indicadores, sujeta a definir fórmulas y permisos:

- Inventario de cilindros por refrigerante, estado y ubicación.
- Kilos solicitados, autorizados e instalados por sucursal, refrigerante y período.
- Comparación con límites semanales y mensuales, una vez acordado su cómputo.
- OT por estado, sucursal y técnico.
- Cilindros retirados pendientes de recepción y diferencias registradas.
- Costos históricos según las cantidades y precios aplicados a cada operación.
- Indicadores ambientales de CO₂e, cuando se defina la masa y el factor GWP
  utilizados. Los kilos de cilindros reemplazados no se consideran automáticamente
  emisiones reales.

Filtros propuestos: fechas, sucursal, refrigerante, técnico y estado de OT.

## Datos y acceso pendientes

Definir quién consulta cada informe y con qué alcance por rol. No se asigna
automáticamente acceso exclusivo al Autorizador ni a toda la organización.

Preparar datos de consulta para BI a partir de los registros operativos,
conservando el historial de autorizaciones, precios y límites aplicados. La
integración no requiere exponer claves de usuario ni fotografías de pesaje.

Antes de implementar, acordar la forma de consulta de Power BI, presentación
de informes dentro o fuera de la aplicación, actualización y requisitos de
acceso/licencias. No se declara configurado ningún servicio de Power BI.

## Estado actual

El prototipo usa datos temporales en memoria. No tiene conexión a Power BI ni
persistencia central en Supabase; todavía no puede ofrecer informes históricos
duraderos. Este documento registra el alcance, sin ejecutar cambios en la base.
