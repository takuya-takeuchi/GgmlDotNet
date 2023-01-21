class BuildTarget
{
   [string] $Platform
   [string] $Target
   [int]    $Architecture
   [string] $Postfix
   [string] $RID

   BuildTarget( [string]$Platform,
                [string]$Target,
                [int]   $Architecture,
                [string]$RID,
                [string]$Postfix = ""
              )
   {
      $this.Platform = $Platform
      $this.Target = $Target
      $this.Architecture = $Architecture
      $this.Postfix = $Postfix
      $this.RID = $RID
   }

   [string] $OperatingSystem
   [string] $Distribution
   [string] $DistributionVersion

   [string] $CudaVersion

   [string] $AndroidVersion
   [string] $AndroidNativeApiLevel
}

class Config
{

   $ConfigurationArray =
   @(
      "Debug",
      "Release",
      "RelWithDebInfo"
   )

   $TargetArray =
   @(
      "cpu",
      "vulkan",
      "arm"
   )

   $PlatformArray =
   @(
      "desktop",
      "android",
      "ios",
      "uwp"
   )

   $ArchitectureArray =
   @(
      32,
      64
   )

   $VisualStudio = "Visual Studio 17 2022"

   static $BuildLibraryWindowsHash =
   @{
      "GgmlDotNet.Native"     = "GgmlDotNetNative.dll";
   }

   static $BuildLibraryLinuxHash =
   @{
      "GgmlDotNet.Native"     = "libGgmlDotNetNative.so";
   }

   static $BuildLibraryOSXHash =
   @{
      "GgmlDotNet.Native"     = "libGgmlDotNetNative.dylib";
   }

   static $BuildLibraryIOSHash =
   @{
      "GgmlDotNet.Native"     = "libGgmlDotNetNative_merged.a";
   }

   [string]   $_Root
   [string]   $_Configuration
   [int]      $_Architecture
   [string]   $_Target
   [string]   $_Platform
   [string]   $_VulkanSDKDirectory
   [int]      $_CudaVersion
   [string]   $_AndroidABI
   [string]   $_AndroidNativeAPILevel
   [string]   $_OSXArchitectures

   #***************************************
   # Arguments
   #  %1: Root directory of GgmlDotNet
   #  %2: Build Configuration (Release/Debug)
   #  %3: Target (cpu/vulkan/arm)
   #  %4: Architecture (32/64)
   #  %5: Platform (desktop/android/ios/uwp)
   #  %6: Optional Argument
   #    Reserved
   #***************************************
   Config(  [string]$Root,
            [string]$Configuration,
            [string]$Target,
            [int]   $Architecture,
            [string]$Platform,
            [string]$Option
         )
   {
      if ($this.ConfigurationArray.Contains($Configuration) -eq $False)
      {
         $candidate = $this.ConfigurationArray -join "/"
         Write-Host "Error: Specify build configuration [${candidate}]" -ForegroundColor Red
         exit -1
      }

      if ($this.TargetArray.Contains($Target) -eq $False)
      {
         $candidate = $this.TargetArray -join "/"
         Write-Host "Error: Specify Target [${candidate}]" -ForegroundColor Red
         exit -1
      }

      if ($this.ArchitectureArray.Contains($Architecture) -eq $False)
      {
         $candidate = $this.ArchitectureArray -join "/"
         Write-Host "Error: Specify Architecture [${candidate}]" -ForegroundColor Red
         exit -1
      }

      if ($this.PlatformArray.Contains($Platform) -eq $False)
      {
         $candidate = $this.PlatformArray -join "/"
         Write-Host "Error: Specify Platform [${candidate}]" -ForegroundColor Red
         exit -1
      }

      switch ($Target)
      {
         "vulkan"
         {
            $this._VulkanSDKDirectory = $env:VULKAN_SDK
         }
      }

      $this._Root = $Root
      $this._Configuration = $Configuration
      $this._Architecture = $Architecture
      $this._Target = $Target
      $this._Platform = $Platform

      switch ($Platform)
      {
         "android"
         {
            $decoded = [Config]::Base64Decode($Option)
            $setting = ConvertFrom-Json $decoded
            $this._AndroidABI            = $setting.ANDROID_ABI
            $this._AndroidNativeAPILevel = $setting.ANDROID_NATIVE_API_LEVEL
         }
         "ios"
         {
            $this._OSXArchitectures = $Option
         }
      }
   }

   static [string] Base64Encode([string]$text)
   {
      $byte = ([System.Text.Encoding]::Default).GetBytes($text)
      return [Convert]::ToBase64String($byte)
   }

   static [string] Base64Decode([string]$base64)
   {
      $byte = [System.Convert]::FromBase64String($base64)
      return [System.Text.Encoding]::Default.GetString($byte)
   }

   static [hashtable] GetBinaryLibraryWindowsHash()
   {
      return [Config]::BuildLibraryWindowsHash
   }

   static [hashtable] GetBinaryLibraryOSXHash()
   {
      return [Config]::BuildLibraryOSXHash
   }

   static [hashtable] GetBinaryLibraryLinuxHash()
   {
      return [Config]::BuildLibraryLinuxHash
   }

   static [hashtable] GetBinaryLibraryIOSHash()
   {
      return [Config]::BuildLibraryIOSHash
   }

   [string] GetRootDir()
   {
      return $this._Root
   }

   [string] GetGgmlRootDir()
   {
      return   Join-Path $this.GetRootDir() src |
               Join-Path -ChildPath ggml
   }

   [string] GetToolchainDir()
   {
      return   Join-Path $this.GetRootDir() src |
               Join-Path -ChildPath toolchains
   }

   [string] GetProtobufRootDir()
   {
      return   Join-Path $this.GetRootDir() src |
               Join-Path -ChildPath protobuf
   }

   [string] GetNugetDir()
   {
      return   Join-Path $this.GetRootDir() nuget
   }

   [int] GetArchitecture()
   {
      return $this._Architecture
   }

   [string] GetConfigurationName()
   {
      return $this._Configuration
   }

   [string] GetAndroidABI()
   {
      return $this._AndroidABI
   }

   [string] GetAndroidNativeAPILevel()
   {
      return $this._AndroidNativeAPILevel
   }

   [string] GetArtifactDirectoryName()
   {
      $target = $this._Target
      $platform = $this._Platform
      $name = ""

      switch ($platform)
      {
         "desktop"
         {
            $name = $target
         }
         "android"
         {
            $name = $platform
         }
         "ios"
         {
            $name = $platform
         }
         "uwp"
         {
            $name = Join-Path $platform $target
         }
      }

      return $name
   }

   [string] GetOSName()
   {
      $os = ""

      if ($global:IsWindows)
      {
         $os = "win"
      }
      elseif ($global:IsMacOS)
      {
         if (![string]::IsNullOrEmpty($this._OSXArchitectures))
         {
            $os = "ios"
         }
         else
         {
            $os = "osx"
         }
      }
      elseif ($global:IsLinux)
      {
         $os = "linux"
      }
      else
      {
         Write-Host "Error: This plaform is not support" -ForegroundColor Red
         exit -1
      }

      return $os
   }

   [string] GetVulkanSDKDirectory()
   {
      return [string]$this._VulkanSDKDirectory
   }

   [string] GetArchitectureName()
   {
      $arch = ""
      $target = $this._Target
      $architecture = $this._Architecture

      if ($target -eq "arm")
      {
         if ($architecture -eq 32)
         {
            $arch = "arm"
         }
         elseif ($architecture -eq 64)
         {
            $arch = "arm64"
         }
      }
      else
      {
         if ($architecture -eq 32)
         {
            $arch = "x86"
         }
         elseif ($architecture -eq 64)
         {
            $arch = "x64"
         }
      }

      return $arch
   }

   [string] GetTarget()
   {
      return $this._Target
   }

   [string] GetPlatform()
   {
      return $this._Platform
   }

   [string] GetRootStoreDriectory()
   {
      return $env:CIBuildDir
   }

   [string] GetStoreDriectory([string]$CMakefileDir)
   {
      $DirectoryName = Split-Path $CMakefileDir -leaf
      $buildDir = $this.GetRootStoreDriectory()
      if (!(Test-Path($buildDir)))
      {
         return $CMakefileDir
      }

      return Join-Path $buildDir "GgmlDotNet" | `
             Join-Path -ChildPath $DirectoryName
   }

   [bool] HasStoreDriectory()
   {
      $buildDir = $this.GetRootStoreDriectory()
      return Test-Path($buildDir)
   }

   [string] GetBuildDirectoryName([string]$os="")
   {
      if (![string]::IsNullOrEmpty($os))
      {
         $osname = $os
      }
      elseif (![string]::IsNullOrEmpty($env:TARGETRID))
      {
         $osname = $env:TARGETRID
      }
      else
      {
         $osname = $this.GetOSName()
      }

      $target = $this._Target
      $platform = $this._Platform
      $architecture = $this.GetArchitectureName()

      switch ($platform)
      {
         "android"
         {
            $architecture = $this._AndroidABI
         }
         "ios"
         {
            $architecture = $this._OSXArchitectures
         }
      }

      if ($this._Configuration -eq "Debug" -or $this._Configuration -eq "RelWithDebInfo")
      {
         return "build_${osname}_${platform}_${target}_${architecture}_d"
      }
      else
      {
         return "build_${osname}_${platform}_${target}_${architecture}"
      }
   }

   [string] GetVisualStudio()
   {
      return $this.VisualStudio
   }

   [string] GetVisualStudioArchitecture()
   {
      $architecture = $this._Architecture
      $target = $this._Target

      if ($target -eq "arm")
      {
         if ($architecture -eq 32)
         {
            return "ARM"
         }
         elseif ($architecture -eq 64)
         {
            return "ARM64"
         }
      }
      else
      {
         if ($architecture -eq 32)
         {
            return "Win32"
         }
         elseif ($architecture -eq 64)
         {
            return "x64"
         }
      }

      Write-Host "${architecture} and ${target} do not support" -ForegroundColor Red
      exit -1
   }

   [string] GetAVXINSTRUCTIONS()
   {
      return "ON"
   }

   [string] GetSSE4INSTRUCTIONS()
   {
      return "ON"
   }

   [string] GetSSE2INSTRUCTIONS()
   {
      return "OFF"
   }

   [string] GetToolchainFile()
   {
      $architecture = $this._Architecture
      $target = $this._Target
      $toolchainDir = $this.GetToolchainDir()
      $toolchain = Join-Path $toolchainDir "empty.cmake"

      if ($global:IsLinux)
      {
         if ($target -eq "arm")
         {
            if ($architecture -eq 64)
            {
               $toolchain = Join-Path $toolchainDir "aarch64-linux-gnu.toolchain.cmake"
            }
         }
      }
      else
      {
         $Platform = $this._Platform
         switch ($Platform)
         {
            "ios"
            {
               $osxArchitectures = $this.GetOSXArchitectures()
               $toolchain = Join-Path $toolchainDir "${osxArchitectures}-ios.cmake"
            }
         }
      }

      return $toolchain
   }

   [string] GetDeveloperDir()
   {
      return $env:DEVELOPER_DIR
   }

   [string] GetOSXArchitectures()
   {
      return $this._OSXArchitectures
   }

   [string] GetIOSSDK([string]$osxArchitectures, [string]$developerDir)
   {
      switch ($osxArchitectures)
      {
         "arm64e"
         {
            return "${developerDir}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk"
         }
         "arm64"
         {
            return "${developerDir}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk"
         }
         "arm"
         {
            return "${developerDir}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk"
         }
         "armv7"
         {
            return "${developerDir}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk"
         }
         "armv7s"
         {
            return "${developerDir}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk"
         }
         "i386"
         {
            return "${developerDir}/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator.sdk"
         }
         "x86_64"
         {
            return "${developerDir}/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator.sdk"
         }
      }
      return $this._OSXArchitectures
   }

   static [bool] Build([string]$root, [bool]$docker, [hashtable]$buildHashTable, [BuildTarget]$buildTarget)
   {
      $current = $PSScriptRoot

      $platform              = $buildTarget.Platform
      $target                = $buildTarget.Target
      $architecture          = $buildTarget.Architecture
      $postfix               = $buildTarget.Postfix
      $rid                   = $buildTarget.RID
      $operatingSystem       = $buildTarget.OperatingSystem
      $distribution          = $buildTarget.Distribution
      $distributionVersion   = $buildTarget.DistributionVersion
      $cudaVersion           = $buildTarget.CudaVersion
      $androidVersion        = $buildTarget.AndroidVersion
      $androidNativeApiLevel = $buildTarget.AndroidNativeApiLevel
      $configuration         = "Release"

      $option = ""

      $sourceRoot = Join-Path $root src

      if ($docker -eq $True)
      {
         $dockerDir = Join-Path $root docker

         Set-Location -Path $dockerDir

         $dockerFileDir = Join-Path $dockerDir build  | `
                          Join-Path -ChildPath $distribution | `
                          Join-Path -ChildPath $distributionVersion

         if ($platform -eq "android")
         {
            $setting =
            @{
               'ANDROID_ABI' = $rid;
               'ANDROID_NATIVE_API_LEVEL' = $androidNativeApiLevel
            }
            $option = [Config]::Base64Encode((ConvertTo-Json -Compress $setting))

            $dockername = "ggmldotnet/build/$distribution/$distributionVersion/android/$androidVersion"
            $imagename  = "ggmldotnet/devel/$distribution/$distributionVersion/android/$androidVersion"
         }
         else
         {
            if ($target -ne "cuda")
            {
               $option = ""

               $dockername = "ggmldotnet/build/$distribution/$distributionVersion/$Target" + $postfix
               $imagename  = "ggmldotnet/devel/$distribution/$distributionVersion/$Target" + $postfix
            }
            else
            {
               $option = $cudaVersion

               $cudaVersion = ($cudaVersion / 10).ToString("0.0")
               $dockername = "ggmldotnet/build/$distribution/$distributionVersion/$Target/$cudaVersion"
               $imagename  = "ggmldotnet/devel/$distribution/$distributionVersion/$Target/$cudaVersion"
            }
         }

         $config = [Config]::new($root, $configuration, $target, $architecture, $platform, $option)
         $libraryDir = Join-Path "artifacts" $config.GetArtifactDirectoryName()
         $build = $config.GetBuildDirectoryName($operatingSystem)

         Write-Host "Start 'docker build -t $dockername $dockerFileDir --build-arg IMAGE_NAME=""$imagename""'" -ForegroundColor Green
         docker build --network host --force-rm=true -t $dockername $dockerFileDir --build-arg IMAGE_NAME="$imagename" | Write-Host

         if ($lastexitcode -ne 0)
         {
            Write-Host "Failed to docker build: $lastexitcode" -ForegroundColor Red
            return $False
         }

         if ($platform -eq "desktop")
         {
            if ($target -eq "arm")
            {
               Write-Host "Start 'docker run --rm --privileged multiarch/qemu-user-static --reset -p yes'" -ForegroundColor Green
               docker run --rm --privileged multiarch/qemu-user-static --reset -p yes
            }
         }

         # Build binary
         foreach ($key in $buildHashTable.keys)
         {
            Write-Host "Start 'docker run --rm -v ""$($root):/opt/data/GgmlDotNet"" -e LOCAL_UID=$(id -u $env:USER) -e LOCAL_GID=$(id -g $env:USER) -t $dockername'" -ForegroundColor Green
            docker run --rm --network host `
                        -v "$($root):/opt/data/GgmlDotNet" `
                        -e "LOCAL_UID=$(id -u $env:USER)" `
                        -e "LOCAL_GID=$(id -g $env:USER)" `
                        -t "$dockername" $key $target $architecture $platform $option | Write-Host

            if ($lastexitcode -ne 0)
            {
               Write-Host "Failed to docker run: $lastexitcode" -ForegroundColor Red
               return $False
            }
         }

         # Copy output binary
         foreach ($key in $buildHashTable.keys)
         {
            $srcDir = Join-Path $sourceRoot $key
            $dll = $buildHashTable[$key]
            $dstDir = Join-Path $current $libraryDir

            CopyToArtifact -srcDir $srcDir -build $build -libraryName $dll -dstDir $dstDir -rid $rid
         }
      }
      else
      {
         if ($platform -eq "ios")
         {
            $option = $rid
         }

         $config = [Config]::new($root, $configuration, $target, $architecture, $platform, $option)
         $libraryDir = Join-Path "artifacts" $config.GetArtifactDirectoryName()
         $build = $config.GetBuildDirectoryName($operatingSystem)

         foreach ($key in $buildHashTable.keys)
         {
            $srcDir = Join-Path $sourceRoot $key

            # Move to build target directory
            Set-Location -Path $srcDir

            $arc = $config.GetArchitectureName()
            Write-Host "Build $key [$arc] for $target" -ForegroundColor Green
            Build -Config $config

            if ($lastexitcode -ne 0)
            {
               return $False
            }
         }

         # Copy output binary
         foreach ($key in $buildHashTable.keys)
         {
            $srcDir = Join-Path $sourceRoot $key
            $dll = $buildHashTable[$key]
            $dstDir = Join-Path $current $libraryDir

            if ($global:IsWindows)
            {
               # CopyToArtifact -configuration "Release" -srcDir $srcDir -build $build -libraryName $dll -dstDir $dstDir -rid $rid
               CopyToArtifact -srcDir $srcDir -build $build -libraryName $dll -dstDir $dstDir -rid $rid
            }
            else
            {
               CopyToArtifact -srcDir $srcDir -build $build -libraryName $dll -dstDir $dstDir -rid $rid
            }
         }
      }

      return $True
   }

}

class ThirdPartyBuilder
{

   [Config]   $_Config

   ThirdPartyBuilder( [Config]$Config )
   {
      $this._Config = $Config
   }

   [string] BuildGgml()
   {
      $ret = ""
      $current = Get-Location

      try
      {
         $Configuration = $this._Config.GetConfigurationName()

         $Platform = $this._Config.GetPlatform()

         switch ($Platform)
         {
            "desktop"
            {
               Write-Host "Start Build ggml" -ForegroundColor Green
               
               $ggmlDir = $this._Config.GetGgmlRootDir()

               $ggmlTarget = Join-Path $current ggml
               New-Item $ggmlTarget -Force -ItemType Directory
               Set-Location $ggmlTarget
               $current2 = Get-Location
               $installDir = Join-Path $current2 install
               $ret = $installDir

               if ($global:IsWindows)
               {
                  $vs = $this._Config.GetVisualStudio()
                  $vsarch = $this._Config.GetVisualStudioArchitecture()

                  Write-Host "   cmake -G `"${vs}`" -A $vsarch -T host=x64 `
         -D CMAKE_BUILD_TYPE=$Configuration `
         -D BUILD_SHARED_LIBS=OFF `
         -D CMAKE_INSTALL_PREFIX=`"${installDir}`" `
         -D GGML_BUILD_EXAMPLES:BOOL=OFF `
         -D GGML_BUILD_TESTS:BOOL=OFF `
         $ggmlDir" -ForegroundColor Yellow
                  cmake -G "${vs}" -A $vsarch -T host=x64 `
                        -D BUILD_SHARED_LIBS=OFF `
                        -D CMAKE_INSTALL_PREFIX="${installDir}" `
                        -D GGML_BUILD_EXAMPLES:BOOL=OFF `
                        -D GGML_BUILD_TESTS:BOOL=OFF `
                        $ggmlDir
                  Write-Host "   cmake --build . --config ${Configuration} --target install" -ForegroundColor Yellow
                  cmake --build . --config $Configuration --target install
               }
               elseif ($global:IsMacOS)
               {
                  $toolchain = $this._Config.GetToolchainFile()

                  # build vulkan variables
                  $Vulkan_INCLUDE_DIR = Join-Path $env:VULKAN_SDK MoltenVK | `
                                        Join-Path -Childpath include
                  $Vulkan_LIBRARY = Join-Path $env:VULKAN_SDK MoltenVK | `
                                    Join-Path -Childpath dylib | `
                                    Join-Path -Childpath macOS | `
                                    Join-Path -Childpath libMoltenVK.dylib

                  Write-Host "   cmake -D CMAKE_BUILD_TYPE=$Configuration `
         -D BUILD_SHARED_LIBS=OFF `
         -D CMAKE_INSTALL_PREFIX=`"${installDir}`" `
         -D CMAKE_TOOLCHAIN_FILE=`"${toolchain}`" `
         -D GGML_BUILD_EXAMPLES:BOOL=OFF `
         $ggmlDir" -ForegroundColor Yellow
                  cmake -D CMAKE_BUILD_TYPE=$Configuration `
                        -D BUILD_SHARED_LIBS=OFF `
                        -D CMAKE_INSTALL_PREFIX="${installDir}" `
                        -D CMAKE_TOOLCHAIN_FILE="${toolchain}" `
                        -D GGML_BUILD_EXAMPLES:BOOL=OFF `
                        $ggmlDir
                  Write-Host "   cmake --build . --config ${Configuration} --target install" -ForegroundColor Yellow
                  cmake --build . --config $Configuration --target install
               }
               else
               {
                  $toolchain = $this._Config.GetToolchainFile()

                  Write-Host "   cmake -D CMAKE_BUILD_TYPE=$Configuration `
         -D BUILD_SHARED_LIBS=OFF `
         -D CMAKE_INSTALL_PREFIX=`"${installDir}`" `
         -D CMAKE_TOOLCHAIN_FILE=`"${toolchain}`" `
         -D GGML_BUILD_EXAMPLES:BOOL=OFF `
         $ggmlDir" -ForegroundColor Yellow
                  cmake -D CMAKE_BUILD_TYPE=$Configuration `
                        -D BUILD_SHARED_LIBS=OFF `
                        -D CMAKE_INSTALL_PREFIX="${installDir}" `
                        -D CMAKE_TOOLCHAIN_FILE="${toolchain}" `
                        -D GGML_BUILD_EXAMPLES:BOOL=OFF `
                        $ggmlDir
                  Write-Host "   cmake --build . --config ${Configuration} --target install" -ForegroundColor Yellow
                  cmake --build . --config $Configuration --target install
               }
            }
            "ios"
            {
               Write-Host "Start Build ggml" -ForegroundColor Green

               $ggmlDir = $this._Config.GetGgmlRootDir()
               $ggmlTarget = Join-Path $current ggml
               New-Item $ggmlTarget -Force -ItemType Directory
               Set-Location $ggmlTarget
               $current2 = Get-Location
               $installDir = Join-Path $current2 install
               $ret = $installDir

               $developerDir = $this._Config.GetDeveloperDir()
               $osxArchitectures = $this._Config.GetOSXArchitectures()
               $toolchain = $this._Config.GetToolchainFile()

               $OSX_SYSROOT = $this._Config.GetIOSSDK($osxArchitectures, $developerDir)

               $targetPlatform = ""
               switch ($osxArchitectures)
               {
                  "arm64e"
                  {
                     $targetPlatform = "ios-arm64"
                  }
                  "arm64"
                  {
                     $targetPlatform = "ios-arm64"
                  }
                  "arm"
                  {
                     $targetPlatform = ""
                  }
                  "armv7"
                  {
                     $targetPlatform = ""
                  }
                  "armv7s"
                  {
                     $targetPlatform = ""
                  }
                  "i386"
                  {
                     $targetPlatform = ""
                  }
                  "x86_64"
                  {
                     $targetPlatform = "ios-arm64_x86_64-simulator"
                  }
               }

               # use libc++ rather than libstdc++
               # * Fix for PThread library not in path
               #     -D CMAKE_THREAD_LIBS_INIT="-lpthread" `
               #     -D CMAKE_HAVE_THREADS_LIBRARY=1 `
               #     -D CMAKE_USE_WIN32_THREADS_INIT=0 `
               #     -D CMAKE_USE_PTHREADS_INIT=1 `
               # * omp.h is missing so remove the following arguments
               #     -D OpenMP_C_FLAGS=`"-Xclang -fopenmp`" `
               #     -D OpenMP_CXX_FLAGS=`"-Xclang -fopenmp`" `
               #     -D OpenMP_C_LIB_NAMES=`"libomp`" `
               #     -D OpenMP_CXX_LIB_NAMES=`"libomp`" `
               #     -D OpenMP_libomp_LIBRARY=`"${developerDir}/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk/usr/lib/libomp.a`" `
               Write-Host "   cmake -D CMAKE_BUILD_TYPE=$Configuration `
         -D CMAKE_CXX_FLAGS=`"-std=c++11 -stdlib=libc++ -static`" `
         -D CMAKE_EXE_LINKER_FLAGS=`"-std=c++11 -stdlib=libc++ -static`" `
         -D CMAKE_SYSTEM_NAME=iOS `
         -D BUILD_SHARED_LIBS=OFF `
         -D CMAKE_OSX_ARCHITECTURES=${osxArchitectures} `
         -D CMAKE_OSX_SYSROOT=${OSX_SYSROOT} `
         -D CMAKE_TOOLCHAIN_FILE=`"${toolchain}`" `
         -D GGML_BUILD_EXAMPLES:BOOL=OFF `
         -D GGML_BUILD_TESTS=OFF `
         $ggmlDir" -ForegroundColor Yellow
               cmake -D CMAKE_BUILD_TYPE=$Configuration `
                     -D CMAKE_CXX_FLAGS="-std=c++11 -stdlib=libc++ -static" `
                     -D CMAKE_EXE_LINKER_FLAGS="-std=c++11 -stdlib=libc++ -static" `
                     -D CMAKE_SYSTEM_NAME=iOS `
                     -D BUILD_SHARED_LIBS=OFF `
                     -D CMAKE_OSX_ARCHITECTURES=${osxArchitectures} `
                     -D CMAKE_OSX_SYSROOT=${OSX_SYSROOT} `
                     -D CMAKE_TOOLCHAIN_FILE="${toolchain}" `
                     -D GGML_BUILD_EXAMPLES:BOOL=OFF `
                     -D GGML_BUILD_TESTS=OFF `
                     $ggmlDir

               Write-Host "   cmake --build . --config ${Configuration} --target install" -ForegroundColor Yellow
               cmake --build . --config $Configuration --target install
            }
            "uwp"
            {
               Write-Host "Start Build ggml" -ForegroundColor Green

               $ggmlDir = $this._Config.GetGgmlRootDir()
               $ggmlTarget = Join-Path $current ggml
               New-Item $ggmlTarget -Force -ItemType Directory
               Set-Location $ggmlTarget
               $current2 = Get-Location
               $installDir = Join-Path $current2 install
               $ret = $installDir

               if ($global:IsWindows)
               {
                  Write-Host "   cmake -G `"NMake Makefiles`" `
         -D CMAKE_BUILD_TYPE=$Configuration `
         -D BUILD_SHARED_LIBS=OFF `
         -D CMAKE_INSTALL_PREFIX=`"${installDir}`" `
         -D GGML_BUILD_EXAMPLES:BOOL=OFF `
         $ggmlDir" -ForegroundColor Yellow
                  cmake -G "NMake Makefiles" `
                        -D CMAKE_BUILD_TYPE=$Configuration `
                        -D BUILD_SHARED_LIBS=OFF `
                        -D CMAKE_INSTALL_PREFIX="${installDir}" `
                        -D GGML_BUILD_EXAMPLES:BOOL=OFF `
                        $ggmlDir
                  Write-Host "   cmake --build . --config ${Configuration} --target install" -ForegroundColor Yellow
                  cmake --build . --config $Configuration --target install
               }
            }
            "android"
            {
               Write-Host "Start Build ggml" -ForegroundColor Green

               $ggmlDir = $this._Config.GetGgmlRootDir()
               $ggmlTarget = Join-Path $current ggml
               New-Item $ggmlTarget -Force -ItemType Directory
               Set-Location $ggmlTarget
               $current2 = Get-Location
               $installDir = Join-Path $current2 install
               $ret = $installDir

               $level = $this._Config.GetAndroidNativeAPILevel()
               $abi = $this._Config.GetAndroidABI()

               Write-Host "   cmake -D CMAKE_TOOLCHAIN_FILE=${env:ANDROID_NDK}/build/cmake/android.toolchain.cmake `
            -D CMAKE_BUILD_TYPE=$Configuration `
            -D CMAKE_INSTALL_PREFIX=`"${installDir}`" `
            -D ANDROID_ABI=$abi `
            -D ANDROID_PLATFORM=android-$level `
            -D GGML_BUILD_EXAMPLES:BOOL=OFF `
            -D GGML_BUILD_TESTS=OFF `
            $ggmlDir" -ForegroundColor Yellow
               cmake -D CMAKE_TOOLCHAIN_FILE="${env:ANDROID_NDK}/build/cmake/android.toolchain.cmake" `
                     -D CMAKE_BUILD_TYPE=$Configuration `
                     -D CMAKE_INSTALL_PREFIX="${installDir}" `
                     -D ANDROID_ABI=$abi `
                     -D ANDROID_PLATFORM=android-$level `
                     -D GGML_BUILD_EXAMPLES:BOOL=OFF `
                     -D GGML_BUILD_TESTS=OFF `
                     $ggmlDir

               Write-Host "   cmake --build . --config ${Configuration} --target install" -ForegroundColor Yellow
               cmake --build . --config $Configuration --target install
            }
         }
      }
      finally
      {
         Set-Location $current
         Write-Host "End Build ggml" -ForegroundColor Green
      }

      return $ret
   }
}

function ConfigCPU([Config]$Config)
{
   $Builder = [ThirdPartyBuilder]::new($Config)

   # Build ggml
   $installGgmlDir = $Builder.BuildGgml()

   # To inclue src/layer
   $ggmlDir = $Config.GetGgmlRootDir()

   # Build GgmlDotNet.Native
   Write-Host "Start Build GgmlDotNet.Native" -ForegroundColor Green
   if ($global:IsWindows)
   {
      $vs = $Config.GetVisualStudio()
      $vsarch = $Config.GetVisualStudioArchitecture()

      Write-Host "   cmake -G `"${vs}`" -A $vsarch -T host=x64 `
         -D CMAKE_BUILD_TYPE=$Configuration `
         -D ggml_DIR=`"${installGgmlDir}`" `
         -D ggml_SRC_DIR=`"${ggmlDir}`" `
         .." -ForegroundColor Yellow
      cmake -G "${vs}" -A $vsarch -T host=x64 `
            -D CMAKE_BUILD_TYPE=$Configuration `
            -D ggml_DIR="${installGgmlDir}" `
            -D ggml_SRC_DIR="${ggmlDir}" `
            ..
   }
   else
   {
      Write-Host "   cmake -D ggml_DIR=`"${installGgmlDir}`" `
         -D ggml_SRC_DIR=`"${ggmlDir}`" `
         .." -ForegroundColor Yellow
      cmake -D ggml_DIR="${installGgmlDir}" `
            -D ggml_SRC_DIR="${ggmlDir}" `
            ..
   }
}

function ConfigARM([Config]$Config)
{
   $Builder = [ThirdPartyBuilder]::new($Config)

   # Build ggml
   $installGgmlDir = $Builder.BuildGgml()

   # To inclue src/layer
   $ggmlDir = $Config.GetGgmlRootDir()

   # Build GgmlDotNet.Native
   Write-Host "Start Build GgmlDotNet.Native" -ForegroundColor Green
   if ($IsWindows)
   {
      Write-Host "   cmake -G `"NMake Makefiles`" `
         -D CMAKE_BUILD_TYPE=$Configuration `
         -D BUILD_SHARED_LIBS=ON `
         -D ggml_DIR=`"${installGgmlDir}`" `
         -D ggml_SRC_DIR=`"${ggmlDir}`" `
         .." -ForegroundColor Yellow
      cmake -G "NMake Makefiles" `
            -D CMAKE_BUILD_TYPE=$Configuration `
            -D BUILD_SHARED_LIBS=ON `
            -D ggml_DIR="${installGgmlDir}" `
            -D ggml_SRC_DIR="${ggmlDir}" `
            ..
   }
   else
   {
      $toolchain = $Config.GetToolchainFile()

      Write-Host "   cmake -D BUILD_SHARED_LIBS=ON `
         -D CMAKE_TOOLCHAIN_FILE=`"${toolchain}`" `
         -D ggml_DIR=`"${installGgmlDir}`" `
         -D ggml_SRC_DIR=`"${ggmlDir}`" `
         .." -ForegroundColor Yellow
      cmake -D BUILD_SHARED_LIBS=ON `
            -D CMAKE_TOOLCHAIN_FILE="${toolchain}" `
            -D ggml_DIR="${installGgmlDir}" `
            -D ggml_SRC_DIR="${ggmlDir}" `
            ..
   }
}

function ConfigUWP([Config]$Config)
{
   if ($global:IsWindows)
   {
      $Builder = [ThirdPartyBuilder]::new($Config)

      # Build ggml
      $installGgmlDir = $Builder.BuildGgml()

      # To inclue src/layer
      $ggmlDir = $Config.GetGgmlRootDir()

      # Build GgmlDotNet.Native
      Write-Host "Start Build GgmlDotNet.Native" -ForegroundColor Green

      if ($Config.GetTarget() -eq "arm")
      {
         Write-Host "   cmake -G `"NMake Makefiles`" `
      -D CMAKE_BUILD_TYPE=$Configuration `
      -D CMAKE_SYSTEM_NAME=WindowsStore `
      -D CMAKE_SYSTEM_VERSION=10.0 `
      -D WINAPI_FAMILY=WINAPI_FAMILY_APP `
      -D _WINDLL=ON `
      -D _WIN32_UNIVERSAL_APP=ON `
      -D BUILD_SHARED_LIBS=ON `
      -D ggml_DIR=`"${installGgmlDir}`" `
      -D ggml_SRC_DIR=`"${ggmlDir}`" `
      -D NO_GUI_SUPPORT:BOOL=ON `
      .." -ForegroundColor Yellow
         cmake -G "NMake Makefiles" `
               -D CMAKE_BUILD_TYPE=$Configuration `
               -D CMAKE_SYSTEM_NAME=WindowsStore `
               -D CMAKE_SYSTEM_VERSION=10.0 `
               -D WINAPI_FAMILY=WINAPI_FAMILY_APP `
               -D _WINDLL=ON `
               -D _WIN32_UNIVERSAL_APP=ON `
               -D BUILD_SHARED_LIBS=ON `
               -D ggml_DIR="${installGgmlDir}" `
               -D ggml_SRC_DIR="${ggmlDir}" `
               -D NO_GUI_SUPPORT:BOOL=ON `
               ..
      }
      else
      {
         Write-Host "   cmake -G `"NMake Makefiles`" `
      -D CMAKE_BUILD_TYPE=$Configuration `
      -D CMAKE_SYSTEM_NAME=WindowsStore `
      -D CMAKE_SYSTEM_VERSION=10.0 `
      -D WINAPI_FAMILY=WINAPI_FAMILY_APP `
      -D _WINDLL=ON `
      -D _WIN32_UNIVERSAL_APP=ON `
      -D BUILD_SHARED_LIBS=ON `
      -D ggml_DIR=`"${installGgmlDir}`" `
      -D ggml_SRC_DIR=`"${ggmlDir}`" `
      -D NO_GUI_SUPPORT:BOOL=ON `
      .." -ForegroundColor Yellow
         cmake -G "NMake Makefiles" `
               -D CMAKE_BUILD_TYPE=$Configuration `
               -D CMAKE_SYSTEM_NAME=WindowsStore `
               -D CMAKE_SYSTEM_VERSION=10.0 `
               -D WINAPI_FAMILY=WINAPI_FAMILY_APP `
               -D _WINDLL=ON `
               -D _WIN32_UNIVERSAL_APP=ON `
               -D BUILD_SHARED_LIBS=ON `
               -D ggml_DIR="${installGgmlDir}" `
               -D ggml_SRC_DIR="${ggmlDir}" `
               -D NO_GUI_SUPPORT:BOOL=ON `
               ..
      }
   }
}

function ConfigANDROID([Config]$Config)
{
   if (!${env:ANDROID_NDK_HOME})
   {
      Write-Host "Error: Specify ANDROID_NDK_HOME environmental value" -ForegroundColor Red
      exit -1
   }

   if ((Test-Path "${env:ANDROID_NDK_HOME}/build/cmake/android.toolchain.cmake") -eq $False)
   {
      Write-Host "Error: Specified Android NDK toolchain '${env:ANDROID_NDK_HOME}/build/cmake/android.toolchain.cmake' does not found" -ForegroundColor Red
      exit -1
   }

   $Builder = [ThirdPartyBuilder]::new($Config)

   # Build ggml
   $installGgmlDir = $Builder.BuildGgml()

   # To inclue src/layer
   $ggmlDir = $Config.GetGgmlRootDir()

   # # Build GgmlDotNet.Native
   Write-Host "Start Build GgmlDotNet.Native" -ForegroundColor Green

   $level = $Config.GetAndroidNativeAPILevel()
   $abi = $Config.GetAndroidABI()

   # https://github.com/Tencent/ggml/wiki/FAQ-ggml-throw-error#undefined-reference-to-__kmpc_xyz_xyz
   # $env:NDK_TOOLCHAIN_VERSION = 4.9
      Write-Host "   cmake -D CMAKE_TOOLCHAIN_FILE=${env:ANDROID_NDK}/build/cmake/android.toolchain.cmake `
   -D ANDROID_ABI=$abi `
   -D ANDROID_PLATFORM=android-$level `
   -D ANDROID_CPP_FEATURES:STRING=`"exceptions rtti`" `
   -D BUILD_SHARED_LIBS=ON `
   -D ggml_DIR=`"${installGgmlDir}`" `
   -D ggml_SRC_DIR=`"${ggmlDir}`" `
   .." -ForegroundColor Yellow
      cmake -D CMAKE_TOOLCHAIN_FILE=${env:ANDROID_NDK}/build/cmake/android.toolchain.cmake `
            -D ANDROID_ABI=$abi `
            -D ANDROID_PLATFORM=android-$level `
            -D ANDROID_CPP_FEATURES:STRING="exceptions rtti" `
            -D BUILD_SHARED_LIBS=ON `
            -D ggml_DIR="${installGgmlDir}" `
            -D ggml_SRC_DIR="${ggmlDir}" `
            ..
}

function ConfigIOS([Config]$Config)
{
   if ($IsMacOS)
   {
      $Builder = [ThirdPartyBuilder]::new($Config)

      $osxArchitectures = $Config.GetOSXArchitectures()

      $targetPlatform = ""
      switch ($osxArchitectures)
      {
         "arm64e"
         {
            $targetPlatform = "ios-arm64"
         }
         "arm64"
         {
            $targetPlatform = "ios-arm64"
         }
         "arm"
         {
            $targetPlatform = ""
         }
         "armv7"
         {
            $targetPlatform = ""
         }
         "armv7s"
         {
            $targetPlatform = ""
         }
         "i386"
         {
            $targetPlatform = ""
         }
         "x86_64"
         {
            $targetPlatform = "ios-arm64_x86_64-simulator"
         }
      }

      # Build ggml
      $installGgmlDir = $Builder.BuildGgml()

      # To inclue src/layer
      $ggmlDir = $Config.GetGgmlRootDir()

      # # Build GgmlDotNet.Native
      Write-Host "Start Build GgmlDotNet.Native" -ForegroundColor Green

      $developerDir = $Config.GetDeveloperDir()
      $osxArchitectures = $Config.GetOSXArchitectures()
      $toolchain = $Config.GetToolchainFile()

      $OSX_SYSROOT = $Config.GetIOSSDK($osxArchitectures, $developerDir)

      # use libc++ rather than libstdc++
      # * Fix for PThread library not in path
      #     -D CMAKE_THREAD_LIBS_INIT="-lpthread" `
      #     -D CMAKE_HAVE_THREADS_LIBRARY=1 `
      #     -D CMAKE_USE_WIN32_THREADS_INIT=0 `
      #     -D CMAKE_USE_PTHREADS_INIT=1 `
      Write-Host "   cmake -D CMAKE_SYSTEM_NAME=iOS `
         -D CMAKE_OSX_ARCHITECTURES=${osxArchitectures} `
         -D CMAKE_OSX_SYSROOT=${OSX_SYSROOT} `
         -D CMAKE_TOOLCHAIN_FILE=`"${toolchain}`" `
         -D CMAKE_CXX_FLAGS=`"-std=c++11 -stdlib=libc++ -static`" `
         -D CMAKE_EXE_LINKER_FLAGS=`"-std=c++11 -stdlib=libc++ -static`" `
         -D ggml_DIR=`"${installGgmlDir}`" `
         -D ggml_SRC_DIR=`"${ggmlDir}`" `
         -D CMAKE_THREAD_LIBS_INIT=`"-lpthread`" `
         -D CMAKE_HAVE_THREADS_LIBRARY=1 `
         -D CMAKE_USE_WIN32_THREADS_INIT=0 `
         -D CMAKE_USE_PTHREADS_INIT=1 `
         .." -ForegroundColor Yellow
      cmake -D CMAKE_SYSTEM_NAME=iOS `
            -D CMAKE_OSX_ARCHITECTURES=${osxArchitectures} `
            -D CMAKE_OSX_SYSROOT=${OSX_SYSROOT} `
            -D CMAKE_TOOLCHAIN_FILE="${toolchain}" `
            -D CMAKE_CXX_FLAGS="-std=c++11 -stdlib=libc++ -static" `
            -D CMAKE_EXE_LINKER_FLAGS="-std=c++11 -stdlib=libc++ -static" `
            -D BUILD_SHARED_LIBS=OFF `
            -D ggml_DIR="${installGgmlDir}" `
            -D ggml_SRC_DIR="${ggmlDir}" `
            -D CMAKE_THREAD_LIBS_INIT=`"-lpthread`" `
            -D CMAKE_HAVE_THREADS_LIBRARY=1 `
            -D CMAKE_USE_WIN32_THREADS_INIT=0 `
            -D CMAKE_USE_PTHREADS_INIT=1 `
            ..
   }
   else
   {
      Write-Host "Error: This platform can not build iOS binary" -ForegroundColor Red
      exit -1
   }
}

function Build([Config]$Config)
{
   $Current = Get-Location

   Write-Host "git submodule update --init --recursive" -ForegroundColor Yellow
   git submodule update --init --recursive

   $Output = $Config.GetBuildDirectoryName("")
   if ((Test-Path $Output) -eq $False)
   {
      New-Item $Output -ItemType Directory
   }

   Set-Location -Path $Output

   $Target = $Config.GetTarget()
   $Platform = $Config.GetPlatform()

   switch ($Platform)
   {
      "desktop"
      {
         switch ($Target)
         {
            "cpu"
            {
               ConfigCPU $Config
            }
            "arm"
            {
               ConfigARM $Config
            }
         }
      }
      "android"
      {
         ConfigANDROID $Config
      }
      "ios"
      {
         ConfigIOS $Config
      }
      "uwp"
      {
         ConfigUWP $Config
      }
   }

   $cofiguration = $Config.GetConfigurationName()
   Write-Host "   cmake --build . --config ${cofiguration}" -ForegroundColor Yellow
   cmake --build . --config ${cofiguration}

   $Platform = $Config.GetPlatform()

   # # Post build
   # switch ($Platform)
   # {
   #    "ios"
   #    {
   #       $BuildTargets = @()
   #       $BuildTargets += New-Object PSObject -Property @{ Platform = "arm64";  StaticLib = "" }
   #       $BuildTargets += New-Object PSObject -Property @{ Platform = "x86_64";  }

   #       foreach($BuildTarget in $BuildTargets)
   #       {
   #          $platform = $BuildTarget.Platform
   #          $osxArchitectures = $Config.GetOSXArchitectures()

   #          if ($osxArchitectures -eq $platform )
   #          {
   #             Write-Host "Invoke libtool for ${platform}" -ForegroundColor Yellow

   #             switch ($platform)
   #             {
   #                "arm64e"
   #                {
   #                   $Vulkan_LIBRARY = Join-Path $env:VULKAN_SDK MoltenVK | `
   #                                     Join-Path -Childpath MoltenVK.xcframework | `
   #                                     Join-Path -Childpath "ios-arm64" | `
   #                                     Join-Path -Childpath libMoltenVK.a
   #                }
   #                "arm64"
   #                {
   #                   $Vulkan_LIBRARY = Join-Path $env:VULKAN_SDK MoltenVK | `
   #                                     Join-Path -Childpath MoltenVK.xcframework | `
   #                                     Join-Path -Childpath "ios-arm64" | `
   #                                     Join-Path -Childpath libMoltenVK.a
   #                }
   #                "arm"
   #                {
   #                   $targetPlatform = ""
   #                }
   #                "armv7"
   #                {
   #                   $targetPlatform = ""
   #                }
   #                "armv7s"
   #                {
   #                   $targetPlatform = ""
   #                }
   #                "i386"
   #                {
   #                   $targetPlatform = ""
   #                }
   #                "x86_64"
   #                {
   #                   $Vulkan_LIBRARY = Join-Path $env:VULKAN_SDK MoltenVK | `
   #                                     Join-Path -Childpath MoltenVK.xcframework | `
   #                                     Join-Path -Childpath "ios-arm64_x86_64-simulator" | `
   #                                     Join-Path -Childpath libMoltenVK.a
   #                }
   #             }

   #             if (Test-Path "libGgmlDotNetNative_merged.a")
   #             {
   #                Remove-Item "libGgmlDotNetNative_merged.a"
   #             }

   #             # https://github.com/abseil/abseil-cpp/issues/604
   #             if ($vulkan -eq $True)
   #             {
   #                libtool -o "libGgmlDotNetNative_merged.a" `
   #                           "libGgmlDotNetNative.a" `
   #                           "opencv/install/lib/libopencv_world.a" `
   #                           "opencv/install/share/OpenCV/3rdparty/lib/liblibpng.a" `
   #                           "opencv/install/share/OpenCV/3rdparty/lib/liblibjpeg.a" `
   #                           "opencv/install/share/OpenCV/3rdparty/lib/libzlib.a" `
   #                           "ggml/install/lib/libMachineIndependent.a" `
   #                           "ggml/install/lib/libOGLCompiler.a" `
   #                           "ggml/install/lib/libggml.a" `
   #                           "ggml/install/lib/libOSDependent.a" `
   #                           "ggml/install/lib/libGenericCodeGen.a" `
   #                           "ggml/install/lib/libSPIRV.a" `
   #                           "ggml/install/lib/libglslang.a" `
   #                           "${Vulkan_LIBRARY}"
   #             }
   #             else
   #             {
   #                libtool -o "libGgmlDotNetNative_merged.a" `
   #                           "libGgmlDotNetNative.a" `
   #                           "opencv/install/lib/libopencv_world.a" `
   #                           "opencv/install/share/OpenCV/3rdparty/lib/liblibpng.a" `
   #                           "opencv/install/share/OpenCV/3rdparty/lib/liblibjpeg.a" `
   #                           "opencv/install/share/OpenCV/3rdparty/lib/libzlib.a" `
   #                           "ggml/install/lib/libggml.a"
   #             }
   #          }
   #       }
   #    }
   # }

   # Move to Root directory
   Set-Location -Path $Current
}

function CopyToArtifact()
{
   Param([string]$srcDir, [string]$build, [string]$libraryName, [string]$dstDir, [string]$rid, [string]$configuration="")

   if ($configuration)
   {
      $binary = Join-Path ${srcDir} ${build}  | `
               Join-Path -ChildPath ${configuration} | `
               Join-Path -ChildPath ${libraryName}
   }
   else
   {
      $binary = Join-Path ${srcDir} ${build}  | `
               Join-Path -ChildPath ${libraryName}
   }

   $dstDir = Join-Path $dstDir runtimes | `
             Join-Path -ChildPath ${rid} | `
             Join-Path -ChildPath native

   $output = Join-Path $dstDir $libraryName

   if (!(Test-Path($binary)))
   {
      Write-Host "${binary} does not exist" -ForegroundColor Red
   }

   if (!(Test-Path($dstDir)))
   {
      Write-Host "${dstDir} does not exist" -ForegroundColor Red
   }

   Write-Host "Copy ${libraryName} to ${output}" -ForegroundColor Green
   Copy-Item ${binary} ${output}
}