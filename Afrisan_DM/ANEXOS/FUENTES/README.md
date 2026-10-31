# AFRISAN-APT
Repositorio oficial del proyecto "Sistema Integral de Gestión y Trazabilidad de Refrigerantes" para la empresa AFRISAN para la Fase 1 de APT, con evidencias individuales, grupales, documentación, arquitectura y planificación.

## Trabajo nuevo: comenzar aquí

Respaldo previo al formateo del 24/10/2026: [guía de restauración y estado del proyecto](AFRISAN_NUEVO/06_RESPALDO_ANTES_FORMATEO_2026_10_24/README.md).

Desde el 19 de octubre de 2026, la revisión del negocio y el desarrollo nuevo se
organizan en **[AFRISAN_NUEVO](AFRISAN_NUEVO/README.md)**, a cargo de Dante Moya.
Incluye procesos, base de datos y seguimiento. La aplicación nueva de registro
y trabajo técnico en memoria está en [AFRISAN_NUEVO/03_CODIGO](AFRISAN_NUEVO/03_CODIGO/README.md).
La solución **[AFRISAN.slnx](AFRISAN.slnx)** de la raíz abre esos proyectos nuevos.

El 25/10/2026 se retiraron de la copia local los proyectos antiguos `Afrisan.Api`
y `Afrisan.Web`, junto con `Iniciar-Demo.ps1`. Su código permanece recuperable
en el historial de Git (por ejemplo, en el commit `e0f5c94`). La documentación,
las evidencias y el respaldo histórico se conservan.

Los scripts que estaban en BASE_DATOS_NUEVA ahora están en
[AFRISAN_NUEVO/02_BASE_DATOS](AFRISAN_NUEVO/02_BASE_DATOS/README.md).

## Integrante actual y continuidad

Desde la actualización documental del 16 de octubre de 2026, Dante Moya es el único integrante actual y responsable de continuidad del proyecto, según lo informado por el estudiante. La aplicación individual de la pauta de evaluación grupal debe confirmarse con el docente.

El informe de avance, el plan de trabajo y el backlog de Fase 2 se encuentran en [EVIDENCIAS_GRUPALES/EVA_2](EVIDENCIAS_GRUPALES/EVA_2/README.md). Las contribuciones y evidencias de integrantes anteriores se conservan como antecedentes históricos con su autoría original.

La arquitectura está aprobada, según confirmación del estudiante recibida el 16 de octubre de 2026. El trabajo actual se concentra en completar el flujo de bodega, validar su funcionamiento y reunir evidencias de EVA 2.

## Ejecutar la aplicación actual

Abre **AFRISAN.slnx**, selecciona el perfil **AFRISAN NUEVO - AMBOS** y ejecuta
con F5. También se puede abrir la solución de `AFRISAN_NUEVO/03_CODIGO`.

Para iniciar desde PowerShell en la raíz del repositorio:

```powershell
.\AFRISAN_NUEVO\03_CODIGO\Iniciar-Revision.ps1
```

Abre [AFRISAN](http://localhost:5286/acceso). La API usa el puerto 5181.
El inventario, los trabajos y las fotografías permanecen en memoria hasta reiniciar
la API. No requiere conexión a Supabase en este modo. No iniciar el script y F5
simultáneamente si los servidores ya están encendidos.

Último recorrido implementado: [retiro por el técnico, aprobación de Bodega y
revisión del Autorizador](AFRISAN_NUEVO/04_SEGUIMIENTO/FLUJO_TERRENO_2026_10_25/README.md).
