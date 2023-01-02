$Current = $PSScriptRoot

$Root = Split-Path $Current -Parent
$SourceRoot = Join-Path $Root src
$GgmlDotNetRoot = Join-Path $SourceRoot GgmlDotNet

docfx init -q -o docs
Set-Location $Current