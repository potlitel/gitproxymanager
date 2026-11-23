<#
.SYNOPSIS
Genera el instalador MSI de Git Proxy Manager para Windows.

.DESCRIPTION
1. Publica la aplicacion Avalonia (framework-dependent, win-x64).
2. Usa heat.exe (WiX v5) para recolectar todos los archivos publicados en
   installer/Components.wxs (ComponentGroup "PublishedFiles").
3. Compila installer/GitProxyManager.Installer.wixproj con WiX v5.

Requiere: .NET 10 SDK y WiX Toolset 5 (dotnet tool install -g wix),
o el MSI installer de WiX con heat.exe en el PATH.
#>
[CmdletBinding()]
param(
    [string] $Configuration = 'Release',
    [string] $Runtime = 'win-x64',
    [switch] $SelfContained
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$publishDir = Join-Path $root 'installer\publish'
$installerDir = Join-Path $root 'installer'
$components = Join-Path $installerDir 'Components.wxs'
$project = Join-Path $root 'GitProxyManager.csproj'
$wixproj = Join-Path $installerDir 'GitProxyManager.Installer.wixproj'

Write-Host "==> Publicando $Runtime ($Configuration)..." -ForegroundColor Cyan
$selfContainedArg = if ($SelfContained) { 'true' } else { 'false' }
dotnet publish $project -c $Configuration -r $Runtime --self-contained $selfContainedArg -o $publishDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish fallo con codigo $LASTEXITCODE" }

Write-Host '==> Recolectando archivos publicados (heat)...' -ForegroundColor Cyan
$heat = Get-Command heat.exe -ErrorAction SilentlyContinue
if ($null -eq $heat) {
    throw 'No se encontro heat.exe. Instala WiX Toolset 5 y agregalo al PATH.'
}

Push-Location $installerDir
try {
    & $heat.Source dir 'publish' `
        -cg PublishedFiles `
        -dr INSTALLFOLDER `
        -srd -sfrag -ag -suid -sreg `
        -o $components
    if ($LASTEXITCODE -ne 0) { throw "heat.exe fallo con codigo $LASTEXITCODE" }

    Write-Host '==> Compilando MSI (wix build)...' -ForegroundColor Cyan
    $wix = Get-Command wix.exe -ErrorAction SilentlyContinue
    if ($null -ne $wix) {
        & $wix.Source build $wixproj -o (Join-Path $root 'artifacts\GitProxyManager-v1.1.0-Setup.msi')
        if ($LASTEXITCODE -ne 0) { throw "wix build fallo con codigo $LASTEXITCODE" }
    } else {
        dotnet build $wixproj -c Release
        if ($LASTEXITCODE -ne 0) { throw "dotnet build del instalador fallo con codigo $LASTEXITCODE" }
    }
}
finally {
    Pop-Location
}

Write-Host 'Listo. El MSI se encuentra en installer\bin\Release o artifacts\.' -ForegroundColor Green
