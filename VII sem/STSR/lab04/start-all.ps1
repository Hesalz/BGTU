$ErrorActionPreference = "Stop"

$root = $PSScriptRoot

$commands = @(
    @{
        Title = "Server X"
        Command = "Set-Location -LiteralPath '$root'; dotnet run --project '.\TDWA04-01\TDWA04-01.csproj' -- --Nick X --Port 5001"
    },
    @{
        Title = "Server Y"
        Command = "Set-Location -LiteralPath '$root'; dotnet run --project '.\TDWA04-01\TDWA04-01.csproj' -- --Nick Y --Port 5002"
    },
    @{
        Title = "Server Z"
        Command = "Set-Location -LiteralPath '$root'; dotnet run --project '.\TDWA04-01\TDWA04-01.csproj' -- --Nick Z --Port 5003"
    },
    @{
        Title = "API Gateway"
        Command = "Set-Location -LiteralPath '$root'; Start-Sleep -Seconds 3; dotnet run --project '.\ApiGateway\ApiGateway.csproj' -- --urls 'http://localhost:5000'"
    }
)

$wt = Get-Command wt.exe -ErrorAction SilentlyContinue

if (-not $wt) {
    Write-Host "Windows Terminal не найден. Запускаю отдельные окна." -ForegroundColor Yellow

    foreach ($item in $commands) {
        $encoded = [Convert]::ToBase64String(
            [Text.Encoding]::Unicode.GetBytes($item.Command)
        )

        Start-Process powershell.exe -ArgumentList "-NoExit -EncodedCommand $encoded"
    }

    exit
}

$encoded = @()

foreach ($item in $commands) {
    $encoded += [Convert]::ToBase64String(
        [Text.Encoding]::Unicode.GetBytes($item.Command)
    )
}

& wt.exe -w new `
    new-tab --title "Server X" powershell.exe -NoExit -EncodedCommand $encoded[0] `
    `; new-tab --title "Server Y" powershell.exe -NoExit -EncodedCommand $encoded[1] `
    `; new-tab --title "Server Z" powershell.exe -NoExit -EncodedCommand $encoded[2] `
    `; new-tab --title "API Gateway" powershell.exe -NoExit -EncodedCommand $encoded[3]

Write-Host "Команды запуска отправлены." -ForegroundColor Green