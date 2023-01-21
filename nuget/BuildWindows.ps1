Param()

# import class and function
$ScriptPath = $PSScriptRoot
$GgmlDotNet = Split-Path $ScriptPath -Parent
$ScriptPath = Join-Path $GgmlDotNet "nuget" | `
              Join-Path -ChildPath "BuildUtils.ps1"
import-module $ScriptPath -function *

$OperatingSystem="win"

# Store current directory
$Current = Get-Location

$BuildSourceHash = [Config]::GetBinaryLibraryWindowsHash()

$VulkanSDKDir = $env:VULKAN_SDK
if ([string]::IsNullOrEmpty($VulkanSDKDir))
{
   Write-Host "Environmental Value 'VULKAN_SDK' is not defined." -ForegroundColor Yellow
}

if ($VulkanSDKDir -And !(Test-Path $VulkanSDKDir))
{
   Write-Host "Environmental Value 'VULKAN_SDK' does not exist." -ForegroundColor Yellow
}

$BuildTargets = @()
$BuildTargets += [BuildTarget]::new("desktop", "cpu",    64, "$OperatingSystem-x64", "" )
# $BuildTargets += [BuildTarget]::new("desktop", "cpu",    32, "$OperatingSystem-x86", "" )

foreach ($BuildTarget in $BuildTargets)
{
   $BuildTarget.OperatingSystem = ${OperatingSystem}
   
   $ret = [Config]::Build($GgmlDotNet, $False, $BuildSourceHash, $BuildTarget)
   if ($ret -eq $False)
   {
      Set-Location -Path $Current
      exit -1
   }
}

# Move to Root directory 
Set-Location -Path $Current