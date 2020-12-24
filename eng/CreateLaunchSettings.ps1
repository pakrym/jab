param($source, $launchSettings)

$settings = @{
        'profiles' = [ordered]@{}
};

$args = Get-Content $source
$args[1] = '"' + $args[1] + '"'

$settings.profiles["csc"] = [ordered]@{
    'commandName'='Executable';
    'executablePath'='dotnet'
    'commandLineArgs'= $args | Join-String -Separator ' ';
    'workingDirectory'=Get-Location
}

Write-Host "dotnet " ($args | Join-String -Separator ' ')
$settings | ConvertTo-Json | Out-File $launchSettings