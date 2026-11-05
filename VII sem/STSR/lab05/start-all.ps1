$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host ""
Write-Host "======================================"
Write-Host "       LABORATORY WORK 05"
Write-Host "======================================"
Write-Host ""
Write-Host "Select load balancing mode:"
Write-Host ""
Write-Host "1 - Round Robin"
Write-Host "2 - Sticky Sessions"
Write-Host "3 - Weighted (50/30/20)"
Write-Host ""

do {
    $choice = Read-Host "Enter mode number (1-3)"

    switch ($choice) {
        "1" { $mode = "RoundRobin" }
        "2" { $mode = "Sticky" }
        "3" { $mode = "Weighted" }
        default {
            Write-Host "Invalid choice. Enter 1, 2 or 3." -ForegroundColor Yellow
            $mode = $null
        }
    }
} while (-not $mode)

$env:BALANCER_MODE = $mode
$mode | Set-Content -Path "$root\balancer-mode.txt" -Encoding ASCII

Write-Host ""
Write-Host "Selected mode: $mode" -ForegroundColor Green
Write-Host "======================================"

$servers = @(
    @{ Nick = "X"; Port = 5101; Delay = 1000 },
    @{ Nick = "Y"; Port = 5102; Delay = 2000 },
    @{ Nick = "Z"; Port = 5103; Delay = 3000 }
)

$tabs = @()

foreach ($server in $servers) {
    $cmd = "Set-Location -LiteralPath '$root'; " +
           "dotnet run --no-build --project '$root\TDWA05-01\TDWA05-01.csproj' -- " +
           "--Nick $($server.Nick) --Port $($server.Port) --Delay $($server.Delay)"
    $tabs += @{
        Title   = "Server $($server.Nick) (:$($server.Port))"
        Command = $cmd
    }
}

$gatewayCmd = "Set-Location -LiteralPath '$root'; " +
              "`$env:BALANCER_MODE = '$mode'; " +
              "dotnet run --project '$root\ApiGateway\ApiGateway.csproj'"
$tabs += @{
    Title   = "API Gateway (:$5000)"
    Command = $gatewayCmd
}

$encoded = @()
foreach ($item in $tabs) {
    $encoded += [Convert]::ToBase64String(
        [Text.Encoding]::Unicode.GetBytes($item.Command)
    )
}

Write-Host ""
Write-Host "Starting backend servers and API Gateway in Windows Terminal tabs..."
Write-Host "Mode: $mode"
Write-Host "Gateway: http://localhost:5000/lb"
Write-Host "======================================"
Write-Host ""

& wt.exe -w new `
    new-tab --title $tabs[0].Title powershell.exe -NoExit -EncodedCommand $encoded[0] `
    `; new-tab --title $tabs[1].Title powershell.exe -NoExit -EncodedCommand $encoded[1] `
    `; new-tab --title $tabs[2].Title powershell.exe -NoExit -EncodedCommand $encoded[2] `
    `; new-tab --title $tabs[3].Title powershell.exe -NoExit -EncodedCommand $encoded[3]