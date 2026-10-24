# Compara los archivos del commit restaurado con el manifiesto del respaldo.
# No modifica Git, no ejecuta la aplicación y no requiere paquetes adicionales.
$ErrorActionPreference = 'Stop'
$carpetaRespaldo = $PSScriptRoot
$raizProyecto = [IO.Path]::GetFullPath((Join-Path $carpetaRespaldo '../..'))
$manifiesto = Get-Content -LiteralPath (Join-Path $carpetaRespaldo 'MANIFIESTO_GIT.json') -Raw | ConvertFrom-Json
$archivosGit = @{}
Push-Location $raizProyecto
try {
    $entradasGit = @(git -c core.quotePath=false ls-tree -r --full-tree HEAD)
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo leer el commit de Git.' }
    foreach ($entrada in $entradasGit) {
        if ($entrada -notmatch '^(\d+) blob ([0-9a-f]+)\t(.+)$') {
            throw "Entrada inesperada de Git: $entrada"
        }
        $archivosGit[$Matches[3]] = "$($Matches[1]) $($Matches[2])"
    }
    $rutaManifiesto = 'AFRISAN_NUEVO/06_RESPALDO_ANTES_FORMATEO_2026_10_24/MANIFIESTO_GIT.json'
    foreach ($archivo in $manifiesto.Archivos) {
        $esperado = "$($archivo.Modo) $($archivo.ObjetoGit)"
        if (-not $archivosGit.ContainsKey($archivo.Ruta) -or $archivosGit[$archivo.Ruta] -ne $esperado) {
            throw "Falta un archivo o su contenido versionado es distinto: $($archivo.Ruta)"
        }
    }
    $rutasEsperadas = @{}
    foreach ($archivo in $manifiesto.Archivos) { $rutasEsperadas[$archivo.Ruta] = $true }
    $adicionales = @($archivosGit.Keys | Where-Object { $_ -ne $rutaManifiesto -and -not $rutasEsperadas.ContainsKey($_) })
    if ($adicionales.Count -gt 0) { throw 'El commit contiene archivos adicionales: comprueba que estés en la etiqueta del respaldo.' }
    Write-Host "Respaldo verificado: $($manifiesto.Archivos.Count) archivos coinciden con el manifiesto de Git."
} finally {
    Pop-Location
}
