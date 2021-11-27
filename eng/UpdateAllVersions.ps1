param($NewVersion, $BasePath = "$PSScriptRoot/..")

$regexPrefix = "<PackageReference Include=`"Jab`" Version=`""
$regexToFind = "$regexPrefix([\w\d\.-]+)"

Get-ChildItem -Path $BasePath -File -Recurse | ForEach-Object {
    $content = $_ | Get-Content -Raw
    if ($content -match $regexToFind) {
        Write-Host "Replacing text in file '$($_.FullName)'"
        $_ | Set-Content -Value ($content -replace $matches[0].Trim(), "$regexPrefix$NewVersion") -Force -NoNewline
    }
}