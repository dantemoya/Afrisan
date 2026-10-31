[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string]$Servidor = 'aws-0-us-east-1.pooler.supabase.com',
    [ValidateRange(1, 65535)]
    [int]$Puerto = 5432,
    [ValidateNotNullOrEmpty()]
    [string]$BaseDatos = 'postgres',
    [ValidateNotNullOrEmpty()]
    [string]$Usuario = 'postgres.sugsgvrqxtzeaddkvzii',
    [string]$CertificadoRaiz
)

$ErrorActionPreference = 'Stop'
$proyectoApi = Join-Path $PSScriptRoot 'Afrisan.Nuevo.Api\Afrisan.Nuevo.Api.csproj'
if ($CertificadoRaiz) {
    $CertificadoRaiz = (Resolve-Path -LiteralPath $CertificadoRaiz).ProviderPath
}

# Compilar antes de pedir la contraseña; no inicia ni reinicia la API existente.
& dotnet build $proyectoApi --nologo -m:1 -p:UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar la API. No se guardaron credenciales.' }

Write-Host "Conexión de diagnóstico: ${Servidor}:${Puerto}, base ${BaseDatos}, usuario ${Usuario}."
Write-Host 'La contraseña se guardará en User Secrets de tu perfil local, fuera del repositorio.'
Write-Host 'User Secrets es una configuración de desarrollo y no cifra el archivo local.'
$claveSegura = Read-Host 'Contraseña de la base de datos (entrada oculta)' -AsSecureString
if ($claveSegura.Length -eq 0) {
    $claveSegura.Dispose()
    throw 'La contraseña no puede estar vacía.'
}

$punteroClave = [IntPtr]::Zero
$cadena = New-Object System.Data.Common.DbConnectionStringBuilder
$jsonSecreto = $null
try {
    $cadena['Host'] = $Servidor
    $cadena['Port'] = $Puerto
    $cadena['Database'] = $BaseDatos
    $cadena['Username'] = $Usuario
    $cadena['SSL Mode'] = 'VerifyFull'
    if ($CertificadoRaiz) { $cadena['Root Certificate'] = $CertificadoRaiz }
    $punteroClave = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($claveSegura)
    $cadena['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($punteroClave)
    $jsonSecreto = @{ 'ConnectionStrings:Supabase' = $cadena.ConnectionString } | ConvertTo-Json -Compress
    # stdin evita incluir la contraseña en argumentos de procesos o historial.
    $jsonSecreto | & dotnet user-secrets set --project $proyectoApi | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo guardar la configuración local.' }
}
finally {
    if ($punteroClave -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($punteroClave) }
    $claveSegura.Dispose()
    $cadena.Clear()
    $jsonSecreto = $null
}

& dotnet run --project $proyectoApi --no-build --launch-profile http -- --verificar-supabase
if ($LASTEXITCODE -ne 0) {
    throw 'La configuración quedó guardada, pero la comprobación no fue satisfactoria. Revisa el diagnóstico anterior.'
}
