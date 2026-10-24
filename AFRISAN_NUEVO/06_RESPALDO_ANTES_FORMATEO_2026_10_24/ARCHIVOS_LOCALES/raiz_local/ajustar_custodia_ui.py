from pathlib import Path
root=Path('AFRISAN_NUEVO/03_CODIGO')
changes={}
def edit(rel,old,new):
 p=root/rel;s=changes.get(p,p.read_text(encoding='utf-8-sig'));assert old in s,(rel,old[:70]);changes[p]=s.replace(old,new)
edit('Afrisan.Nuevo.Api/TrabajoEndpoints.cs','Guid foto, TrabajoStore store, ClaimsPrincipal user) => TypedResults.Ok(store.QuitarFoto(id, operacion, foto, user))','Guid foto, int version, TrabajoStore store, ClaimsPrincipal user) => TypedResults.Ok(store.QuitarFoto(id, operacion, foto, user, version))')
edit('Afrisan.Nuevo.Api/TrabajoEndpoints.cs','Guid operacion, TrabajoStore store, ClaimsPrincipal user) => TypedResults.Ok(store.Confirmar(id, operacion, user))','Guid operacion, int version, TrabajoStore store, ClaimsPrincipal user) => TypedResults.Ok(store.Confirmar(id, operacion, user, version))')
edit('Afrisan.Nuevo.Api/TrabajoEndpoints.cs','        grupo.MapPost("/{id:guid}/termino-tecnico",','        grupo.MapPost("/{id:guid}/operaciones/{operacion:guid}/descartar", (Guid id, Guid operacion, int version, TrabajoStore store, ClaimsPrincipal user) => TypedResults.Ok(store.Descartar(id, operacion, version, user)))\n            .RequireAuthorization(p => p.RequireRole("Tecnico"));\n        grupo.MapPost("/{id:guid}/termino-tecnico",')
edit('Afrisan.Nuevo.Web/Services/TrabajoClient.cs','ConfirmarOperacionAsync(Guid id, Guid operacion) =>','ConfirmarOperacionAsync(Guid id, Guid operacion, int version) =>')
edit('Afrisan.Nuevo.Web/Services/TrabajoClient.cs','/operaciones/{operacion}/confirmar");','/operaciones/{operacion}/confirmar?version={version}");')
edit('Afrisan.Nuevo.Web/Services/TrabajoClient.cs','QuitarFotoAsync(Guid id, Guid operacion, Guid foto)','QuitarFotoAsync(Guid id, Guid operacion, Guid foto, int version)')
edit('Afrisan.Nuevo.Web/Services/TrabajoClient.cs','/fotos/{foto}");','/fotos/{foto}?version={version}");')
edit('Afrisan.Nuevo.Web/Services/TrabajoClient.cs','    public Task<OtResponse> CerrarOtAsync','    public Task<OtResponse> DescartarOperacionAsync(Guid id, Guid operacion, int version) => PostSinCuerpo<OtResponse>(OtRuta(id) + $"/operaciones/{operacion}/descartar?version={version}");\n    public Task<OtResponse> CerrarOtAsync')
edit('Afrisan.Nuevo.Web/Services/TrabajoLocalService.cs','    public ValueTask<BorradorLocal[]> BorradoresAsync() => js.InvokeAsync<BorradorLocal[]>("afrisanLocal.listar", "borrador", Usuario);','''    public async ValueTask<BorradorLocal[]> BorradoresAsync() => (await js.InvokeAsync<BorradorLocal[]>("afrisanLocal.listar", "borrador", Usuario)).Where(b => !b.Descartado).ToArray();
    public async Task DescartarAsync(Guid otId, Guid operacionId)
    {
        if (!await sincronizacion.WaitAsync(0)) throw new InvalidOperationException("Espera a que termine la sincronización antes de descartar.");
        try
        {
            var local = (await BorradoresAsync()).FirstOrDefault(b => b.OtId == otId && b.Datos.Id == operacionId);
            var ot = await api.OrdenAsync(otId); // Requiere conexión: no ocultar un borrador remoto por error de red.
            var op = ot.Operaciones.FirstOrDefault(o => o.Id == operacionId);
            if (op is not null) ot = await api.DescartarOperacionAsync(otId, operacionId, local?.Datos.Version ?? op.Version);
            if (local is not null) await GuardarLocalAsync(local with { Descartado = true, Pendiente = false, ConfirmarAlSincronizar = false });
            await CacheAsync(ot);
        }
        finally { sincronizacion.Release(); }
    }''')
edit('Afrisan.Nuevo.Web/Services/TrabajoLocalService.cs','actual.OtId, actual.Datos.Id, eliminada)','actual.OtId, actual.Datos.Id, eliminada, actual.Datos.Version)')
edit('Afrisan.Nuevo.Web/Services/TrabajoLocalService.cs','actual.OtId, actual.Datos.Id, foto)','actual.OtId, actual.Datos.Id, foto with { Version = actual.Datos.Version })')
edit('Afrisan.Nuevo.Web/Services/TrabajoLocalService.cs','actual.OtId, actual.Datos.Id);','actual.OtId, actual.Datos.Id, actual.Datos.Version);')
edit('Afrisan.Nuevo.Web/Pages/DetalleOt.razor','@e.Fecha.ToLocalTime().ToString("dd/MM HH:mm")</small>','@e.Fecha.ToLocalTime().ToString("dd/MM HH:mm") · @e.Estado</small>')
edit('Afrisan.Nuevo.Web/Pages/DetalleOt.razor','<input id="recepcion-codigo" @bind="codigoRecepcion" />','<select id="recepcion-codigo" @bind="codigoRecepcion"><option value="">Selecciona un envase pendiente</option>@foreach (var codigo in RecepcionesPendientes) { <option value="@codigo">@codigo</option> }</select>')
edit('Afrisan.Nuevo.Web/Pages/DetalleOt.razor','    private bool EsTecnico =>','    private string[] RecepcionesPendientes => ot is null ? [] : ot.Operaciones.Where(o => o.Estado == "Registrada" && o.Tipo == "Reemplazo" && !o.RecibidoBodega).Select(o => o.CodigoRetirado!).Concat(ot.Entregas.Where(e => e.Estado == "ConTecnico").Select(e => e.Codigo)).Distinct().ToArray();\n    private bool EsTecnico =>')
edit('Afrisan.Nuevo.Web/Pages/OperacionTecnica.razor','@foreach (var entrega in ot.Entregas)','@foreach (var entrega in ot.Entregas.Where(e => e.Estado == "ConTecnico"))')
edit('Afrisan.Nuevo.Web/Pages/OperacionTecnica.razor','@foreach (var codigo in ot.CilindrosARetirar)','@foreach (var codigo in ot.CilindrosARetirar.Where(c => !ot.Operaciones.Any(o => o.Id != OperacionId && o.Estado == "Registrada" && o.CodigoRetirado == c)))')
edit('Afrisan.Nuevo.Web/Pages/OperacionTecnica.razor','<p class="fine-print">Los campos','<button type="button" class="btn subtle" disabled="@ocupado" @onclick="Descartar">Descartar este borrador</button>\n        <p class="fine-print">Los campos')
edit('Afrisan.Nuevo.Web/Pages/OperacionTecnica.razor.cs','    private Task GuardarYEnviar()','    private async Task Descartar()\n    {\n        if (ocupado) return; ocupado = true; error = null;\n        try { await Local.DescartarAsync(Id, OperacionId); Navigation.NavigateTo($"ordenes/{Id}"); }\n        catch (Exception ex) { error = Formato.Error(ex); }\n        finally { ocupado = false; }\n    }\n    private Task GuardarYEnviar()')
edit('Afrisan.Nuevo.Web/Pages/OperacionTecnica.razor.cs','    [Inject] private InventarioClient Api','    [Inject] private NavigationManager Navigation { get; set; } = default!;\n    [Inject] private InventarioClient Api')
edit('Afrisan.Nuevo.Web/Pages/OperacionTecnica.razor.cs','if (remoto?.Estado == "Registrada")','if (remoto?.Estado == "Descartada") throw new InvalidOperationException("Este borrador fue descartado. Vuelve a la OT para registrar otro trabajo.");\n            if (remoto?.Estado == "Registrada")')
edit('Afrisan.Nuevo.Web/Pages/DetalleCilindro.razor','Esta ficha todavía no representa ubicación en sucursal, OT ni custodia.','La ubicación y el último pesaje se actualizan al registrar entregas, instalaciones, retiros y recepciones.')
for p,s in changes.items():p.write_text(s,encoding='utf-8')
print('Flujos actualizados:',len(changes))
