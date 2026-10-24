from pathlib import Path
p=Path('AFRISAN_NUEVO/03_CODIGO/Afrisan.Nuevo.Web/Pages/NuevaOt.razor')
s=p.read_text(encoding='utf-8-sig')
s=s.replace('@bind-Value="form.Sucursal" maxlength="200"','@bind-Value="form.Sucursal" @bind-Value:after="CambiarSucursal" list="sucursales-conocidas" maxlength="200"')
s=s.replace('<label for="ot-descripcion">','<datalist id="sucursales-conocidas">@foreach (var sucursal in cilindros.Where(c => c.Ubicacion == "Sucursal").Select(c => c.Sucursal).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase)) { <option value="@sucursal"></option> }</datalist>\n        <label for="ot-descripcion">')
start=s.index('        <label for="ot-retirar">');end=s.index('        <div class="form-actions">',start)
s=s[:start]+'''        <h2 class="mt">Cilindros a retirar de la sucursal</h2>
        <label class="check-label"><input type="checkbox" checked="@soloAgregados" @onchange="CambiarTipoPedido" /> Solo agregar cilindros, sin retiros</label>
        @if (!soloAgregados)
        {
            <label for="filtrar-retiros">Filtrar lista por refrigerante, serie o código</label><input id="filtrar-retiros" @bind="busqueda" @bind:event="oninput" placeholder="Opcional: filtra la lista" />
            @if (string.IsNullOrWhiteSpace(form.Sucursal)) { <p class="field-help">Primero elige o escribe la sucursal para consultar sus cilindros.</p> }
            else if (!Candidatos.Any()) { <p class="notice warning">No hay cilindros instalados que coincidan en esta sucursal. Bodega debe registrar sus existencias con ubicación inicial Sucursal. Los envases disponibles en Bodega se entregan después como reemplazos.</p> }
            else
            {
                <div class="table-wrap"><table><thead><tr><th>Retirar</th><th>Cilindro</th><th>Refrigerante</th><th>Contenido registrado</th><th>Disponibilidad</th></tr></thead><tbody>
                @foreach (var c in Candidatos)
                {
                    <tr @key="c.Id"><td><input type="checkbox" aria-label="@($"Retirar {c.Codigo}")" checked="@seleccionados.Contains(c.Codigo)" disabled="@Reservado(c.Codigo)" @onchange="e => Seleccionar(c.Codigo, e)" /></td><td><strong>@c.Codigo</strong><small class="cell-note">@(c.NumeroSerie ?? "Sin serie")</small></td><td>@c.RefrigeranteCodigo</td><td>@Formato.Gramos(c.ContenidoSegunPesajeGr)</td><td>@(Reservado(c.Codigo) ? "Reservado en otra OT" : "Disponible para retiro")</td></tr>
                }
                </tbody></table></div>
            }
            <p class="field-help">@seleccionados.Count seleccionados. Elige en el pedido el refrigerante de cada cilindro marcado. Los kilos solicitados y autorizados se registran por separado.</p>
        }
''' +s[end:]
s=s.replace('    private readonly Guid idOperacion', '''    private readonly HashSet<string> seleccionados = new(StringComparer.OrdinalIgnoreCase);
    private OtResponse[] ordenes = [];
    private string busqueda = "";
    private bool soloAgregados;
    private IEnumerable<CilindroResponse> Candidatos => cilindros.Where(c => c.Ubicacion == "Sucursal" && string.Equals(c.Sucursal, form.Sucursal.Trim(), StringComparison.OrdinalIgnoreCase)
        && ($"{c.Codigo} {c.NumeroSerie} {c.RefrigeranteCodigo}".Contains(busqueda.Trim(), StringComparison.OrdinalIgnoreCase))).OrderBy(c => c.RefrigeranteCodigo).ThenBy(c => c.Codigo);
    private bool Reservado(string codigo) => ordenes.Any(o => o.Estado != "Cerrada" && o.CilindrosARetirar.Contains(codigo));
    private void Seleccionar(string codigo, ChangeEventArgs e) { if (e.Value is true) seleccionados.Add(codigo); else seleccionados.Remove(codigo); }
    private void CambiarSucursal() { seleccionados.Clear(); busqueda = ""; }
    private void CambiarTipoPedido(ChangeEventArgs e) { soloAgregados = e.Value is true; seleccionados.Clear(); }
    private readonly Guid idOperacion''')
s=s.replace('cilindros = await Api.CilindrosAsync();','cilindros = await Api.CilindrosAsync(); ordenes = await Api.OrdenesAsync();')
s=s.replace('        ocupado = true; error = null;', '        if (ocupado) return;\n        ocupado = true; error = null;')
s=s.replace('            var ot = await Api.CrearOtAsync', '''            if (!soloAgregados && seleccionados.Count == 0) throw new InvalidOperationException("Selecciona al menos un cilindro de la lista o indica que el pedido es solo de agregados.");
            if (lineas.Any(l => l.RefrigeranteId <= 0 || l.Solicitados <= 0 || l.Autorizados <= 0) || lineas.Select(l => l.RefrigeranteId).Distinct().Count() != lineas.Count)
                throw new InvalidOperationException("Completa las cantidades y selecciona un refrigerante diferente en cada línea.");
            if (cilindros.Any(c => seleccionados.Contains(c.Codigo) && (!lineas.Any(l => l.RefrigeranteId == c.RefrigeranteId) || Reservado(c.Codigo))))
                throw new InvalidOperationException("Revisa los refrigerantes del pedido y los cilindros reservados antes de autorizar.");
            var ot = await Api.CrearOtAsync''')
s=s.replace("CilindrosARetirar = form.Retirar.Split([',','\\n',';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)", 'CilindrosARetirar = seleccionados.OrderBy(c => c).ToArray()')
s=s.replace('        public string Retirar { get; set; } = "";\n','')
p.write_text(s,encoding='utf-8')
print('Autorizador actualizado con selección por lista')
