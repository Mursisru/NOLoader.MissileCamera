param(
    [string]$GameRoot = "C:\Program Files (x86)\Steam\steamapps\common\Nuclear Option",
    [string]$Configuration = "DEV_SDK",
    [string]$PatchToolConfiguration = "",
    [switch]$SkipPatchTool
)

$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$EngineRoot = Join-Path (Split-Path -Parent $RepoRoot) "NOLoader_Engine"

if (-not (Test-Path $EngineRoot)) {
    Write-Error "NOLoader_Engine not found at $EngineRoot (expected sibling of NOLoader.MissileCamera repo)."
}

if (Get-Process -Name "NuclearOption" -ErrorAction SilentlyContinue) {
    Write-Error "Close Nuclear Option before deploy (PatchTool needs Managed DLLs unlocked)."
}

$project = Join-Path $RepoRoot "NOLoader.MissileCamera.csproj"
dotnet build $project -c $Configuration --verbosity minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$dll = Join-Path $RepoRoot "bin\$Configuration\net48\NOLoader.MissileCamera.dll"
$modConfigProject = Join-Path $EngineRoot "DEV.SDK\shared\NOLoader.ModConfig\NOLoader.ModConfig.csproj"
$modConfigDll = Join-Path $EngineRoot "DEV.SDK\shared\NOLoader.ModConfig\bin\$Configuration\net48\NOLoader.ModConfig.dll"
if (-not (Test-Path $modConfigDll)) {
    dotnet build $modConfigProject -c $Configuration --verbosity minimal
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

if (-not (Test-Path $dll)) {
    Write-Error "Build output missing: $dll"
}

$modRoot = Join-Path $GameRoot "NOLoader\mods\MissileCamera"
New-Item -ItemType Directory -Path $modRoot -Force | Out-Null

$dllDst = Join-Path $modRoot "NOLoader.MissileCamera.dll"
Copy-Item -Force $dll $dllDst
Copy-Item -Force $modConfigDll (Join-Path $modRoot "NOLoader.ModConfig.dll")
Copy-Item -Force (Join-Path $RepoRoot "mod.json") (Join-Path $modRoot "mod.json")

Get-Item $dllDst | Format-List FullName, Length, LastWriteTime

$iniSrc = Join-Path $RepoRoot "mod_config.ini"
$iniDst = Join-Path $modRoot "mod_config.ini"
Copy-Item -Force $iniSrc $iniDst
Write-Host "Synced mod_config.ini"

if (-not $SkipPatchTool) {
    $patchCfg = if ($PatchToolConfiguration) { $PatchToolConfiguration } else { $Configuration }
    Write-Host "Applying mod IL patches via PatchTool ($patchCfg)..."
    dotnet run --project (Join-Path $EngineRoot "src\NOLoader.PatchTool\NOLoader.PatchTool.csproj") -c $patchCfg -- $GameRoot
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Write-Host "NOLoader.MissileCamera deployed to $modRoot"
