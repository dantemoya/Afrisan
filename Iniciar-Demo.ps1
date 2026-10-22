# Inicia los dos proyectos en HTTP con datos simulados, sin configurar Supabase.
$ErrorActionPreference = 'Stop'
$raizProyecto = $PSScriptRoot
$carpetaLogs = Join-Path $raizProyecto '.local'
New-Item -ItemType Directory -Force -Path $carpetaLogs | Out-Null

# Compilar antes de iniciar evita que dos procesos compitan por los mismos archivos.
Push-Location $raizProyecto
try {
    dotnet build AFRISAN.slnx --nologo -m:1 -p:UseSharedCompilation=false
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar AFRISAN.' }
} finally {
    Pop-Location
}

$servidores = @(
    @{ Proyecto = 'Afrisan.Api'; Puerto = 5141; Log = 'demo-api'; Argumentos = ' -- --Demo:Enabled=true' },
    @{ Proyecto = 'Afrisan.Web'; Puerto = 5226; Log = 'demo-web'; Argumentos = '' }
)

foreach ($servidor in $servidores) {
    $ocupado = Get-NetTCPConnection -State Listen -LocalPort $servidor.Puerto -ErrorAction SilentlyContinue
    if ($ocupado) {
        Write-Host "Puerto $($servidor.Puerto) ocupado: se conserva el proceso existente."
        continue
    }
    Start-Process -FilePath 'dotnet' -WorkingDirectory $raizProyecto -WindowStyle Hidden `
        -ArgumentList "run --project $($servidor.Proyecto) --no-build --no-restore --launch-profile http$($servidor.Argumentos)" `
        -RedirectStandardOutput (Join-Path $carpetaLogs "$($servidor.Log).log") `
        -RedirectStandardError (Join-Path $carpetaLogs "$($servidor.Log)-error.log")
}

$handlerDemo = [System.Net.Http.HttpClientHandler]::new()
$handlerDemo.UseProxy = $false
$clienteDemo = [System.Net.Http.HttpClient]::new($handlerDemo)
$clienteDemo.Timeout = [TimeSpan]::FromSeconds(2)
try {
    $listo = $false
    for ($intento = 0; $intento -lt 15; $intento++) {
        try {
            $estado = $clienteDemo.GetStringAsync('http://localhost:5141/api/entorno').GetAwaiter().GetResult() | ConvertFrom-Json
            $web = $clienteDemo.GetAsync('http://localhost:5226/login').GetAwaiter().GetResult()
            try { $listo = $estado.demo -eq $true -and $web.IsSuccessStatusCode } finally { $web.Dispose() }
            if ($listo) { break }
        } catch { }
        Start-Sleep -Seconds 1
    }
    if (-not $listo) {
        throw 'No arrancaron ambos proyectos en modo demo. Revisa .local/demo-*-error.log y si Visual Studio ocupa los puertos con otro perfil.'
    }
} finally {
    $clienteDemo.Dispose()
}

Write-Host 'AFRISAN: http://localhost:5226/login'
Write-Host 'Swagger: http://localhost:5141/swagger'
Write-Host 'Cuenta: bodega@afrisan.example | Clave: DemoAfrisan2026!'
Start-Process 'http://localhost:5226/login'
