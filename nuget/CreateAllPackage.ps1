$targets = @(
   "CPU"
)

$ScriptPath = $PSScriptRoot
$GgmlDotNet = Split-Path $ScriptPath -Parent

$source = Join-Path $GgmlDotNet src | `
          Join-Path -ChildPath GgmlDotNet
dotnet restore ${source}
dotnet build -c Release ${source}

foreach ($target in $targets)
{
   pwsh CreatePackage.ps1 $target
}