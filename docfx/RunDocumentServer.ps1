$Current = $PSScriptRoot

$Root = Split-Path $Current -Parent
$SourceRoot = Join-Path $Root src
$GgmlDotNetRoot = Join-Path $SourceRoot GgmlDotNet
$DocumentDir = Join-Path $GgmlDotNetRoot docfx
$Json = Join-Path $Current docfx.json

docfx "${Json}" --serve
Set-Location $Current