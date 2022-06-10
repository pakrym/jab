param($NewVersion, $BasePath = "$PSScriptRoot/..")

function Replace-In-File($file, $prefix, $regex)
{
    $content = Get-Content $file -Raw
    if ($content -match $regex) {
        Write-Host "Replacing text in file '$file'"
        Set-Content $file -Value ($content -replace $matches[0].Trim(), "$prefix$NewVersion") -Force -NoNewline
    }
}

$regexPrefix = "<PackageReference Include=`"Jab`" Version=`""
$regexToFind = "$regexPrefix([\w\d\.-]+)"

Get-ChildItem -Path $BasePath -File -Recurse | ForEach-Object {
    Replace-In-File $_.FullName $regexPrefix $regexToFind
}

$projectPath = Join-Path $BasePath "src/Jab/Jab.Common.props"
$project = Get-Content $projectPath -Raw 
$projectRegexPrefix = "<Version>"
$projectRegexToFind = "$projectRegexPrefix([\w\d\.]+)"
Replace-In-File $projectPath $projectRegexPrefix $projectRegexToFind

