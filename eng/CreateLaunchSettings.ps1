param($source, $target)

$settings = @{
        'profiles' = [ordered]@{}
};

$launchSettings = Join-Path $target "Properties" "launchSettings.json"
$args = Get-Content $source
$args[1] = '"' + $args[1] + '"'
$joinedArgs = $args | Join-String -Separator ' ';
$environmentVariablesToReplace = 'USERPROFILE', 'ProgramW6432'

foreach ($ev in $environmentVariablesToReplace)
{
    $variableValue = (Get-Item "env:$ev").Value
    $joinedArgs = $joinedArgs.Replace($variableValue, "`$($ev)")
}

$testProjectLocation = Get-Location

pushd $target
$relativePath = Resolve-Path -Relative $testProjectLocation
popd

$settings.profiles["csc"] = [ordered]@{
    'commandName'='Executable';
    'executablePath'='dotnet'
    'commandLineArgs'= $joinedArgs;
    'workingDirectory'="`$(MSBuildProjectDirectory)\$relativePath"
}

$settings | ConvertTo-Json | Out-File $launchSettings