
# Afrisan.Api

API ASP.NET Core 8 con PostgreSQL/Npgsql, Identity y JWT. En desarrollo escucha
en `http://localhost:5141`; Swagger está en `/swagger` y la comprobación de base
en `/health/ready`.

Actualmente Development usa datos simulados en memoria, sin conexión a la base.
Las cuentas, instrucciones y límites están en [Demo/README.md](Demo/README.md).
Para utilizar PostgreSQL, establece `Demo:Enabled=false` y configura los secretos.

El modelo, las decisiones del MER y los pasos de migración se explican en
[Database/README.md](Database/README.md). Las claves de conexión, JWT y administrador
se configuran con secretos locales o variables de entorno.
