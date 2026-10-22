using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Afrisan.Api.DTOs;
using Afrisan.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Afrisan.Api.Demo;

// Adaptador de simulación para los contratos que consume la Web actual.
// No sustituye las pruebas de los controladores ni las restricciones de PostgreSQL.
public static class DemoEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly string[] Lectores = ["Administrador", "Jefatura", "Bodega"];
    private static readonly string[] Bodega = ["Administrador", "Bodega"];

    public static void MapDemoEndpoints(this WebApplication app)
    {
        app.MapGet("/api/entorno", (DemoStore s) => Locked(s, () => Ok(new
        {
            demo = true, instanciaId = s.InstanciaId,
            mensaje = "Datos simulados. Los cambios se pierden al reiniciar la API.",
            password = DemoStore.Password,
            cuentas = s.Usuarios.Take(4).Select(u => new { email = u.Cuenta.Email, nombre = u.Cuenta.NombreCompleto, rol = u.Rol }).ToArray()
        }))).AllowAnonymous().WithTags("Demostración");
        app.MapGet("/health/ready", () => Results.Ok(new { estado = "Listo", modo = "Demo", persistencia = "Memoria" }))
            .AllowAnonymous();

        var api = app.MapGroup("/api").RequireAuthorization().AddEndpointFilter<DemoValidationFilter>().WithTags("Demostración");
        api.MapPost("/Auth/login", Login).AllowAnonymous();
        api.MapGet("/Auth/perfil", (ClaimsPrincipal p) => Results.Ok(new
        {
            id = p.FindFirstValue(ClaimTypes.NameIdentifier), nombreCompleto = p.Identity?.Name,
            email = p.FindFirstValue(ClaimTypes.Email), roles = p.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray()
        }));
        api.MapPost("/Usuarios", CrearUsuario).RequireAuthorization(p => p.RequireRole("Administrador"));

        api.MapGet("/Cilindros", (DemoStore s) => Locked(s, () => Ok(s.Cilindros.OrderBy(c => c.Id))))
            .RequireAuthorization(p => p.RequireRole(Lectores));
        api.MapGet("/Cilindros/{id:int}", (int id, DemoStore s) => Locked(s, () =>
            s.Cilindros.Find(c => c.Id == id) is { } c ? Ok(c) : Missing("cilindro")));
        api.MapGet("/Cilindros/qr/{codigoQr}", (string codigoQr, DemoStore s) => Locked(s, () =>
            s.Cilindros.Find(c => c.CodigoQr.Equals(codigoQr.Trim(), StringComparison.OrdinalIgnoreCase)) is { } c ? Ok(c) : Missing("cilindro")));
        api.MapPost("/Cilindros", CrearCilindro).RequireAuthorization(p => p.RequireRole(Bodega));
        api.MapPut("/Cilindros/{id:int}", EditarCilindro).RequireAuthorization(p => p.RequireRole(Bodega));
        api.MapDelete("/Cilindros/{id:int}", EliminarCilindro).RequireAuthorization(p => p.RequireRole(Bodega));

        api.MapGet("/GasRefrigerantes", (DemoStore s) => Locked(s, () => Ok(s.Gases.OrderBy(g => g.Id))))
            .RequireAuthorization(p => p.RequireRole(Lectores));
        api.MapGet("/GasRefrigerantes/{id:int}", (int id, DemoStore s) => Locked(s, () =>
            s.Gases.Find(g => g.Id == id) is { } g ? Ok(g) : Missing("refrigerante")))
            .RequireAuthorization(p => p.RequireRole(Lectores));
        api.MapPost("/GasRefrigerantes", (GasRefrigeranteDto dto, DemoStore s) => GuardarGas(null, dto, s))
            .RequireAuthorization(p => p.RequireRole("Administrador"));
        api.MapPut("/GasRefrigerantes/{id:int}", (int id, GasRefrigeranteDto dto, DemoStore s) => GuardarGas(id, dto, s))
            .RequireAuthorization(p => p.RequireRole("Administrador"));
        api.MapDelete("/GasRefrigerantes/{id:int}", EliminarGas).RequireAuthorization(p => p.RequireRole("Administrador"));

        api.MapGet("/OrdenesTrabajo", (DemoStore s) => Locked(s, () => Ok(s.Ordenes.OrderByDescending(o => o.Id))))
            .RequireAuthorization(p => p.RequireRole(Lectores));
        api.MapGet("/OrdenesTrabajo/activas", (DemoStore s) => Locked(s, () => Ok(s.Ordenes.Where(o => o.Activa).OrderBy(o => o.Codigo))))
            .RequireAuthorization(p => p.RequireRole(Lectores));
        api.MapGet("/OrdenesTrabajo/{id:int}", (int id, DemoStore s) => Locked(s, () =>
            s.Ordenes.Find(o => o.Id == id) is { } o ? Ok(o) : Missing("OT")));
        api.MapPost("/OrdenesTrabajo", CrearOrden).RequireAuthorization(p => p.RequireRole(Bodega));
        api.MapPut("/OrdenesTrabajo/{id:int}/desactivar", DesactivarOrden).RequireAuthorization(p => p.RequireRole(Bodega));

        api.MapPost("/MovimientosInventario/despacho/{cilindroId:int}", Despachar).RequireAuthorization(p => p.RequireRole(Bodega));
        api.MapPost("/MovimientosInventario/retorno/{cilindroId:int}", Retornar).RequireAuthorization(p => p.RequireRole(Bodega));
        api.MapGet("/MovimientosInventario/historial/{cilindroId:int}", Historial);
        api.MapGet("/MovimientosInventario/balance/{cilindroId:int}", Balance).RequireAuthorization(p => p.RequireRole(Lectores));
    }

    private static IResult Login(LoginDto dto, DemoStore s, IConfiguration config) => Locked(s, () =>
    {
        var u = s.Usuarios.Find(u => string.Equals(u.Cuenta.Email, dto.Email?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (u is null || !u.Cuenta.Activo) return Results.Unauthorized();
        if (u.BloqueadoHasta > DateTime.UtcNow) return Results.Json(new { mensaje = "Cuenta demo bloqueada temporalmente." }, statusCode: 401);
        if (s.Hasher.VerifyHashedPassword(u.Cuenta, u.Cuenta.PasswordHash!, dto.Password ?? "") == PasswordVerificationResult.Failed)
        {
            if (++u.IntentosFallidos >= 5) u.BloqueadoHasta = DateTime.UtcNow.AddMinutes(15);
            return Results.Json(new { mensaje = "Correo o contraseña incorrectos." }, statusCode: 401);
        }
        u.IntentosFallidos = 0;
        u.BloqueadoHasta = null;
        var expiry = DateTime.UtcNow.AddHours(2);
        Claim[] claims = [new(ClaimTypes.NameIdentifier, u.Cuenta.Id), new(ClaimTypes.Name, u.Cuenta.NombreCompleto),
            new(ClaimTypes.Email, u.Cuenta.Email!), new(ClaimTypes.Role, u.Rol), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())];
        var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], claims, expires: expiry,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256));
        return Ok(new { mensaje = "Sesión de demostración iniciada.", token = new JwtSecurityTokenHandler().WriteToken(token),
            expiracion = expiry, usuario = new { id = u.Cuenta.Id, nombreCompleto = u.Cuenta.NombreCompleto, email = u.Cuenta.Email, roles = new[] { u.Rol } } });
    });

    private static IResult CrearUsuario(CrearUsuarioDto dto, DemoStore s) => Locked(s, () =>
    {
        var rol = DemoStore.Roles.FirstOrDefault(r => r.Equals(dto.Rol.Trim(), StringComparison.OrdinalIgnoreCase));
        if (rol is null) return Bad("El rol no es válido.");
        if (s.Usuarios.Any(u => string.Equals(u.Cuenta.Email, dto.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict(new { mensaje = "Ya existe una cuenta con ese correo." });
        if (!dto.Password.Any(char.IsUpper) || !dto.Password.Any(char.IsLower) || !dto.Password.Any(char.IsDigit))
            return Bad("La contraseña necesita mayúscula, minúscula y número.");
        var u = s.AddUser(Guid.NewGuid().ToString(), dto.Email.Trim().ToLowerInvariant(), dto.NombreCompleto.Trim(), rol, dto.Password);
        return Created($"/api/Usuarios/{u.Cuenta.Id}", new { id = u.Cuenta.Id, nombreCompleto = u.Cuenta.NombreCompleto,
            email = u.Cuenta.Email, activo = true, rolId = u.Cuenta.RolId, rol });
    });

    private static IResult CrearCilindro(CrearCilindroDto dto, DemoStore s) => Locked(s, () =>
    {
        var gas = s.Gases.Find(g => g.Id == dto.GasRefrigeranteId && g.Activo);
        if (gas is null) return Bad("Selecciona un gas activo.");
        if (!dto.CapacidadKg.HasValue || !dto.TaraKg.HasValue || !dto.PesoActualKg.HasValue) return Bad("Faltan los pesos.");
        if (s.NextQr > 9999) return Results.Conflict(new { mensaje = "Se alcanzó el límite del formato QR." });
        var c = new Cilindro { Id = s.NextCilindroId++, CodigoQr = $"CIL-{s.NextQr++:D4}", GasRefrigeranteId = gas.Id,
            GasRefrigerante = gas, TipoRefrigerante = gas.Codigo, CapacidadKg = dto.CapacidadKg.Value, TaraKg = dto.TaraKg.Value,
            PesoActualKg = dto.PesoActualKg.Value, Estado = "Disponible", FechaRegistro = DateTime.UtcNow };
        s.Cilindros.Add(c);
        return Created($"/api/Cilindros/{c.Id}", c);
    });

    private static IResult EditarCilindro(int id, CilindroDto dto, DemoStore s) => Locked(s, () =>
    {
        var c = s.Cilindros.Find(c => c.Id == id);
        if (c is null) return Missing("cilindro");
        if (dto.CodigoQr != c.CodigoQr) return Bad("El código QR no se puede modificar.");
        if (!string.Equals(dto.Estado, c.Estado, StringComparison.OrdinalIgnoreCase) || dto.PesoActualKg != c.PesoActualKg)
            return Bad("El estado y peso se cambian mediante despacho o retorno.");
        var gas = s.Gases.Find(g => g.Id == dto.GasRefrigeranteId && g.Activo);
        if (gas is null) return Bad("Selecciona un gas activo.");
        if (dto.TaraKg is not > 0 || c.PesoActualKg < dto.TaraKg || c.PesoActualKg > dto.TaraKg + dto.CapacidadKg)
            return Bad("Revisa tara, capacidad y peso bruto.");
        if (s.Movimientos.Any(m => m.CilindroId == id) && (c.GasRefrigeranteId != gas.Id || c.CapacidadKg != dto.CapacidadKg ||
            (c.TaraKg.HasValue && c.TaraKg != dto.TaraKg))) return Results.Conflict(new { mensaje = "Los datos técnicos tienen historial y no pueden cambiarse." });
        c.GasRefrigeranteId = gas.Id; c.GasRefrigerante = gas; c.TipoRefrigerante = gas.Codigo;
        c.CapacidadKg = dto.CapacidadKg; c.TaraKg = dto.TaraKg;
        return Ok(c);
    });

    private static IResult EliminarCilindro(int id, DemoStore s) => Locked(s, () =>
    {
        var c = s.Cilindros.Find(c => c.Id == id);
        if (c is null) return Missing("cilindro");
        if (s.Movimientos.Any(m => m.CilindroId == id)) return Results.Conflict(new { mensaje = "El cilindro tiene movimientos y no puede eliminarse." });
        s.Cilindros.Remove(c);
        return Ok(new { mensaje = "Cilindro eliminado de la demostración." });
    });

    private static IResult GuardarGas(int? id, GasRefrigeranteDto dto, DemoStore s) => Locked(s, () =>
    {
        if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre)) return Bad("Nombre y código son obligatorios.");
        if (s.Gases.Any(g => g.Id != id && g.Codigo.Equals(dto.Codigo.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Bad("Ya existe un refrigerante con ese código.");
        var gas = id.HasValue ? s.Gases.Find(g => g.Id == id) : new GasRefrigerante { Id = s.NextGasId++ };
        if (gas is null) return Missing("refrigerante");
        gas.Nombre = dto.Nombre.Trim(); gas.Codigo = dto.Codigo.Trim(); gas.Descripcion = dto.Descripcion?.Trim() ?? "";
        gas.Activo = dto.Activo; gas.FactorGwp = dto.FactorGwp; gas.FuenteGwp = dto.FuenteGwp?.Trim(); gas.MetodologiaGwp = dto.MetodologiaGwp?.Trim();
        if (!id.HasValue) s.Gases.Add(gas);
        return id.HasValue ? Ok(gas) : Created($"/api/GasRefrigerantes/{gas.Id}", gas);
    });

    private static IResult EliminarGas(int id, DemoStore s) => Locked(s, () =>
    {
        var gas = s.Gases.Find(g => g.Id == id);
        if (gas is null) return Missing("refrigerante");
        if (s.Cilindros.Any(c => c.GasRefrigeranteId == id)) return Results.Conflict(new { mensaje = "El refrigerante está asociado a cilindros." });
        s.Gases.Remove(gas);
        return Ok(new { mensaje = "Refrigerante eliminado de la demostración." });
    });

    private static IResult CrearOrden(CrearOrdenTrabajoDto dto, DemoStore s) => Locked(s, () =>
    {
        var id = s.NextOtId++;
        var orden = new OrdenTrabajo { Id = id, Codigo = $"OT-{id:D6}", Descripcion = dto.Descripcion?.Trim() };
        s.Ordenes.Add(orden);
        return Created($"/api/OrdenesTrabajo/{id}", orden);
    });

    private static IResult DesactivarOrden(int id, DemoStore s) => Locked(s, () =>
    {
        var orden = s.Ordenes.Find(o => o.Id == id);
        if (orden is null) return Missing("OT");
        if (!orden.Activa) return Bad("La OT ya está inactiva.");
        if (s.Movimientos.Any(m => m.OrdenTrabajoId == id && m.CustodiaAbierta))
            return Results.Conflict(new { mensaje = "Hay cilindros pendientes de retorno en esta OT." });
        orden.Activa = false; orden.FechaCierre = DateTime.UtcNow;
        return Ok(new { mensaje = "OT desactivada en la demostración.", orden.Id, orden.Codigo, orden.Activa });
    });

    private static IResult Despachar(int cilindroId, DespachoCilindroDto dto, DemoStore s, ClaimsPrincipal user) => Locked(s, () =>
    {
        var c = s.Cilindros.Find(c => c.Id == cilindroId);
        if (c is null) return Missing("cilindro");
        if (c.Estado != "Disponible") return Bad("Solo se pueden despachar cilindros disponibles.");
        var orden = s.Ordenes.Find(o => o.Id == dto.OrdenTrabajoId && o.Activa);
        if (orden is null) return Bad("Selecciona una OT activa.");
        if (string.IsNullOrWhiteSpace(dto.Responsable)) return Bad("Indica quién retira el cilindro.");
        if (c.TaraKg is not > 0) return Bad("Registra la tara del cilindro antes del despacho.");
        if (!dto.PesoKg.HasValue || dto.PesoKg <= c.TaraKg || dto.PesoKg > c.TaraKg + c.CapacidadKg)
            return Bad("El peso de salida debe superar la tara y no superar tara + capacidad.");
        var m = new MovimientoInventario { Id = s.NextMovimientoId++, CilindroId = c.Id, OrdenTrabajoId = orden.Id,
            TipoMovimiento = "Despacho", PesoKg = dto.PesoKg.Value, UsuarioId = user.FindFirstValue(ClaimTypes.NameIdentifier),
            Responsable = dto.Responsable.Trim(), CustodiaAbierta = true, FotoPesajeRuta = dto.FotoPesajeRuta, Observaciones = dto.Observaciones };
        s.Movimientos.Add(m); c.Estado = "Despachado"; c.PesoActualKg = m.PesoKg;
        return Ok(new { mensaje = "Despacho simulado registrado.", cilindroId = c.Id, codigoQr = c.CodigoQr,
            ordenTrabajoId = orden.Id, ordenTrabajo = orden.Codigo, estado = c.Estado, taraKg = c.TaraKg,
            movimientoId = m.Id, pesoDespachoKg = m.PesoKg, responsable = m.Responsable, fechaMovimiento = m.FechaMovimiento });
    });

    private static IResult Retornar(int cilindroId, RetornoCilindroDto dto, DemoStore s, ClaimsPrincipal user) => Locked(s, () =>
    {
        var c = s.Cilindros.Find(c => c.Id == cilindroId);
        if (c is null) return Missing("cilindro");
        if (c.Estado != "Despachado") return Bad("Solo se pueden retornar cilindros despachados.");
        if (string.IsNullOrWhiteSpace(dto.Responsable)) return Bad("Indica quién devuelve el cilindro.");
        if (!dto.PesoKg.HasValue || dto.PesoKg < 0 || dto.PesoKg < c.TaraKg) return Bad("El peso de retorno no puede ser menor que la tara.");
        var salida = s.Movimientos.Find(m => m.CilindroId == c.Id && m.CustodiaAbierta);
        if (salida is null) return Results.Conflict(new { mensaje = "No hay una custodia abierta." });
        var m = new MovimientoInventario { Id = s.NextMovimientoId++, CilindroId = c.Id, OrdenTrabajoId = salida.OrdenTrabajoId,
            TipoMovimiento = "Retorno", DespachoId = salida.Id, PesoKg = dto.PesoKg.Value,
            UsuarioId = user.FindFirstValue(ClaimTypes.NameIdentifier), Responsable = dto.Responsable.Trim(),
            FotoPesajeRuta = dto.FotoPesajeRuta, Observaciones = dto.Observaciones };
        salida.CustodiaAbierta = false; s.Movimientos.Add(m); c.Estado = "Disponible"; c.PesoActualKg = m.PesoKg;
        return Ok(new { mensaje = "Retorno simulado registrado.", cilindroId = c.Id, codigoQr = c.CodigoQr, estado = c.Estado,
            movimientoId = m.Id, pesoRetornoKg = m.PesoKg, responsable = m.Responsable, fechaMovimiento = m.FechaMovimiento });
    });

    private static IResult Historial(int cilindroId, DemoStore s) => Locked(s, () =>
    {
        var c = s.Cilindros.Find(c => c.Id == cilindroId);
        if (c is null) return Missing("cilindro");
        var movimientos = s.Movimientos.Where(m => m.CilindroId == c.Id).OrderByDescending(m => m.FechaMovimiento).ThenByDescending(m => m.Id)
            .Select(m => new { m.Id, m.TipoMovimiento, m.PesoKg, m.Responsable, m.FechaMovimiento, m.Observaciones,
                m.UsuarioId, m.DespachoId, m.FotoPesajeRuta, ordenTrabajoId = m.OrdenTrabajoId,
                ordenTrabajo = s.Ordenes.Find(o => o.Id == m.OrdenTrabajoId)?.Codigo }).ToArray();
        return Ok(new { cilindroId = c.Id, codigoQr = c.CodigoQr, estadoActual = c.Estado, taraKg = c.TaraKg,
            totalMovimientos = movimientos.Length, movimientos });
    });

    private static IResult Balance(int cilindroId, DemoStore s) => Locked(s, () =>
    {
        var c = s.Cilindros.Find(c => c.Id == cilindroId);
        if (c is null) return Missing("cilindro");
        var retorno = s.Movimientos.Where(m => m.CilindroId == c.Id && m.TipoMovimiento == "Retorno")
            .OrderByDescending(m => m.FechaMovimiento).ThenByDescending(m => m.Id).FirstOrDefault();
        if (retorno is null) return Bad("El cilindro todavía no tiene un retorno registrado.");
        var despacho = s.Movimientos.Find(m => m.Id == retorno.DespachoId);
        if (despacho is null) return Bad("No se encontró el despacho asociado.");
        var diferencia = despacho.PesoKg - retorno.PesoKg;
        return Ok(new { cilindroId = c.Id, codigoQr = c.CodigoQr, taraKg = c.TaraKg,
            despachoId = despacho.Id, ordenTrabajoId = despacho.OrdenTrabajoId, pesoDespachoKg = despacho.PesoKg,
            fechaDespacho = despacho.FechaMovimiento, retornoId = retorno.Id, pesoRetornoKg = retorno.PesoKg,
            fechaRetorno = retorno.FechaMovimiento, diferenciaKg = diferencia, refrigeranteDespachoKg = despacho.PesoKg - c.TaraKg,
            balanceValido = diferencia >= 0, estadoBalance = diferencia >= 0
                ? "Diferencia física simulada. Conciliación con consumo declarado pendiente."
                : "Inconsistencia: el peso de retorno supera al de salida." });
    });

    private static IResult Locked(DemoStore s, Func<IResult> action) { lock (s.Gate) return action(); }
    // Capturar los resultados dentro del bloqueo evita serializar listas mientras otra petición las cambia.
    private static IResult Ok(object value) => Results.Json(JsonSerializer.SerializeToElement(value, JsonOptions));
    private static IResult Created(string location, object value) => Results.Created(location, JsonSerializer.SerializeToElement(value, JsonOptions));
    private static IResult Bad(string message) => Results.BadRequest(new { mensaje = message });
    private static IResult Missing(string entity) => Results.NotFound(new { mensaje = $"No se encontró el registro de {entity}." });
}
