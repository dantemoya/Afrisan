using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace Afrisan.Web.Services;

public sealed class EntornoService(HttpClient http, IJSRuntime js)
{
    public EntornoApp? Info { get; private set; }

    public async Task InicializarAsync()
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Info = await http.GetFromJsonAsync<EntornoApp>("api/entorno", timeout.Token);
            var anterior = await js.InvokeAsync<string?>("localStorage.getItem", "afrisan_demo_instance");
            if (Info?.Demo == true)
            {
                // La clave JWT demo cambia al reiniciar. Se limpia la sesión anterior
                // antes de montar la UI para evitar mostrar acceso con un token inválido.
                if (anterior != Info.InstanciaId)
                    await js.InvokeVoidAsync("localStorage.removeItem", "afrisan_token");
                await js.InvokeVoidAsync("localStorage.setItem", "afrisan_demo_instance", Info.InstanciaId);
            }
            else if (Info is not null && anterior is not null)
            {
                await js.InvokeVoidAsync("localStorage.removeItem", "afrisan_token");
                await js.InvokeVoidAsync("localStorage.removeItem", "afrisan_demo_instance");
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JSException)
        {
            // La pantalla de login mantiene su mensaje de API no disponible.
            Info = null;
        }
    }
}

public sealed class EntornoApp
{
    public bool Demo { get; set; }
    public string? InstanciaId { get; set; }
    public string? Password { get; set; }
    public List<CuentaDemo> Cuentas { get; set; } = [];
}

public sealed class CuentaDemo
{
    public string Email { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Rol { get; set; } = "";
}
