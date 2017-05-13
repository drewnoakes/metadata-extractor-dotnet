Push-Location $PSScriptRoot

$nuget_path = (join-path (join-path $env:LOCALAPPDATA "Nuget") "nuget.exe")
if ((test-path (join-path $env:LOCALAPPDATA "Nuget")) -eq $false) {
    New-Item -Path (join-path $env:LOCALAPPDATA "Nuget") -ItemType Directory | Out-Null
}

if ((test-path $nuget_path) -eq $false -or [System.Version](Get-Item $nuget_path).VersionInfo.FileVersion -lt [System.Version]"3.4.4") {
    try {
        Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget_path
    } catch [System.Net.WebException] {
        Write-Error "Could not access nuget.org, aborting."
        exit 1
    }
}

if ((test-path .\packages\vswhere\tools\vswhere.exe -ErrorAction SilentlyContinue) -eq $false) {
    $nuget_params = 'install', 'vswhere', '-ExcludeVersion', '-OutputDirectory', 'packages'
    &$nuget_path $nuget_params
}

$ids = 'Community', 'Professional', 'Enterprise', 'BuildTools' | foreach { 'Microsoft.VisualStudio.Product.' + $_ }
$instance = .\packages\vswhere\tools\vswhere.exe -latest -products $ids -requires 'Microsoft.Component.MSBuild' -format json `
          | convertfrom-json `
          | select-object -first 1

$msbuild = join-path $instance.installationPath 'MSBuild\15.0\Bin\MSBuild.exe'
if ((test-path $msbuild) -eq $false) {
    Write-Error "Could not find msbuild."
    exit 2
}

&$msbuild MetadataExtractor\MetadataExtractor.csproj /t:Restore,Build,Pack /p:Configuration=Release /p:PackageOutputPath=..\artifacts
&$msbuild MetadataExtractor\MetadataExtractor.csproj /t:Restore,Build,Pack /p:Configuration=Release /p:PackageOutputPath=..\artifacts /p:Signed=True

Pop-Location
