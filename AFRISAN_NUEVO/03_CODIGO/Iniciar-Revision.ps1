# Ejecutar desde PowerShell o abrir la solución y usar el perfil AMBOS en Visual Studio.
$ErrorActionPreference = 'Stop'
$raizRevision = $PSScriptRoot
$logsRevision = Join-Path $raizRevision '.local'
New-Item -ItemType Directory -Path $logsRevision -Force | Out-Null
foreach ($puertoRevision in @(5181, 5286)) {
    if (Get-NetTCPConnection -State Listen -LocalPort $puertoRevision -ErrorAction SilentlyContinue) {
        throw "El puerto $puertoRevision ya está ocupado. Detén la ejecución anterior desde Visual Studio o su terminal antes de iniciar otra."
    }
}
Push-Location $raizRevision
try {
    dotnet build Afrisan.Nuevo.slnx --nologo -m:1 -p:UseSharedCompilation=false
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar la aplicación nueva.' }
} finally { Pop-Location }

foreach ($servidorRevision in @(@{Proyecto='Afrisan.Nuevo.Api';Log='api'}, @{Proyecto='Afrisan.Nuevo.Web';Log='web'})) {
    Start-Process -FilePath dotnet -WorkingDirectory $raizRevision -WindowStyle Hidden `
        -ArgumentList "run --project $($servidorRevision.Proyecto) --no-build --no-restore --launch-profile http" `
        -RedirectStandardOutput (Join-Path $logsRevision "$($servidorRevision.Log).log") `
        -RedirectStandardError (Join-Path $logsRevision "$($servidorRevision.Log)-error.log")
}
Add-Type -AssemblyName System.Net.Http
$handlerRevision = [System.Net.Http.HttpClientHandler]::new()
$handlerRevision.UseProxy = $false
$clienteRevision = [System.Net.Http.HttpClient]::new($handlerRevision)
$clienteRevision.Timeout = [TimeSpan]::FromSeconds(2)
$listoRevision = $false
try {
    for ($intentoRevision=0; $intentoRevision -lt 15; $intentoRevision++) {
        try {
            $entornoRevision = $clienteRevision.GetStringAsync('http://localhost:5181/api/v1/entorno').GetAwaiter().GetResult() | ConvertFrom-Json
            $paginaRevision = $clienteRevision.GetAsync('http://localhost:5286/acceso').GetAwaiter().GetResult()
            try { $listoRevision = $entornoRevision.modo -eq 'Memoria' -and $paginaRevision.IsSuccessStatusCode } finally { $paginaRevision.Dispose() }
            if ($listoRevision) { break }
        } catch { }
        Start-Sleep -Seconds 1
    }
} finally { $clienteRevision.Dispose() }
if (-not $listoRevision) { throw 'No respondieron ambos proyectos. Revisa los registros de .local.' }
Write-Host 'AFRISAN nuevo: http://localhost:5286/acceso'
Write-Host 'Swagger: http://localhost:5181/swagger'
Start-Process 'http://localhost:5286/acceso'
