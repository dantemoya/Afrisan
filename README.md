# AFRISAN-APT
Repositorio oficial del proyecto "Sistema Integral de Gestión y Trazabilidad de Refrigerantes" para la empresa AFRISAN para la Fase 1 de APT, con evidencias individuales, grupales, documentación, arquitectura y planificación.

## Trabajo nuevo: comenzar aquí

Respaldo previo al formateo del 24/10/2026: [guía de restauración y estado del proyecto](AFRISAN_NUEVO/06_RESPALDO_ANTES_FORMATEO_2026_10_24/README.md).

Desde el 19 de octubre de 2026, la revisión del negocio y el desarrollo nuevo se
organizan en **[AFRISAN_NUEVO](AFRISAN_NUEVO/README.md)**, a cargo de Dante Moya.
Incluye procesos, base de datos y seguimiento. La aplicación nueva de registro
y consulta en memoria está en [AFRISAN_NUEVO/03_CODIGO](AFRISAN_NUEVO/03_CODIGO/README.md);
los proyectos de la raíz corresponden a la implementación anterior.

Los scripts que estaban en BASE_DATOS_NUEVA ahora están en
[AFRISAN_NUEVO/02_BASE_DATOS](AFRISAN_NUEVO/02_BASE_DATOS/README.md).

## Integrante actual y continuidad

Desde la actualización documental del 16 de octubre de 2026, Dante Moya es el único integrante actual y responsable de continuidad del proyecto, según lo informado por el estudiante. La aplicación individual de la pauta de evaluación grupal debe confirmarse con el docente.

El informe de avance, el plan de trabajo y el backlog de Fase 2 se encuentran en [EVIDENCIAS_GRUPALES/EVA_2](EVIDENCIAS_GRUPALES/EVA_2/README.md). Las contribuciones y evidencias de integrantes anteriores se conservan como antecedentes históricos con su autoría original.

La arquitectura está aprobada, según confirmación del estudiante recibida el 16 de octubre de 2026. El trabajo actual se concentra en completar el flujo de bodega, validar su funcionamiento y reunir evidencias de EVA 2.

## Ejecutar la implementación anterior con datos simulados

En desarrollo está activado el modo demostración: no requiere PostgreSQL,
Supabase ni secretos de conexión.

En dos terminales, desde esta carpeta:

```powershell
dotnet run --project Afrisan.Api --launch-profile http
```

```powershell
dotnet run --project Afrisan.Web --launch-profile http
```

Abre http://localhost:5226/login y selecciona una cuenta de demostración.
Los cambios se conservan en memoria hasta reiniciar la API.
