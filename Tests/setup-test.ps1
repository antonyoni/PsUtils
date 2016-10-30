$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Split-Path $here -Parent
$modulePath = Join-Path $projectDir "bin\Debug\PsUtils.dll"

Import-Module $modulePath
