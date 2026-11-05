
param(
    [int]$RequestsPerMethod = 50,
    [string]$GatewayUrl = "http://localhost:5000/lb",
    [string]$Mode = $(
    if ($env:BALANCER_MODE) {
        $env:BALANCER_MODE
    }
    elseif (Test-Path "$PSScriptRoot\balancer-mode.txt") {
        (Get-Content "$PSScriptRoot\balancer-mode.txt" -Raw).Trim()
    }
    else {
        "RoundRobin"
    }
)
)

$ErrorActionPreference = "Stop"

$methods = @("GET", "POST", "PUT", "DELETE")
$counts = @{ X = 0; Y = 0; Z = 0 }
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$total = 0
$errors = 0

Write-Host ""
Write-Host "========== LOAD BALANCER TEST =========="
Write-Host "Mode: $Mode"

foreach ($method in $methods) {
    Write-Host ""
    Write-Host "Sending $RequestsPerMethod requests: $method"

    for ($i = 1; $i -le $RequestsPerMethod; $i++) {
        try {
            $response = Invoke-RestMethod `
                -Uri $GatewayUrl `
                -Method $method `
                -WebSession $session `
                -TimeoutSec 90

            $nick = [string]$response.Nick

            if ($counts.ContainsKey($nick)) {
                $counts[$nick]++
                $total++
            }
            else {
                Write-Warning "Unknown Nick: '$nick'"
                $errors++
            }
        }
        catch {
            Write-Warning "Request $method #$i failed: $($_.Exception.Message)"
            $errors++
        }
    }
}

Write-Host ""
Write-Host "========== RESULTS =========="
Write-Host "Successfully processed: $total of $($RequestsPerMethod * 4)"
Write-Host "Failed or unrecognized: $errors"

foreach ($nick in @("X", "Y", "Z")) {
    $percent = if ($total -gt 0) {
        [math]::Round((100.0 * $counts[$nick] / $total), 2)
    }
    else {
        0
    }

    Write-Host "Nick ${nick}: $($counts[$nick]) requests - $percent percent"
}

if ($Mode -eq "Sticky") {

    Write-Host ""
    Write-Host "========== STICKY SESSIONS TEST =========="
    Write-Host "Creating 20 independent clients, 3 requests per client..."
    Write-Host ""

    $stickyResults = @()
    $stickyErrors = 0

    for ($clientNumber = 1; $clientNumber -le 20; $clientNumber++) {

        $clientSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession

        $cookie = New-Object System.Net.Cookie(
            "TDWA05-Session",
            [guid]::NewGuid().ToString("N"),
            "/",
            "localhost"
        )

        $clientSession.Cookies.Add($cookie)

        $clientServers = @()

        for ($requestNumber = 1; $requestNumber -le 3; $requestNumber++) {
            try {
                $response = Invoke-RestMethod `
                    -Uri $GatewayUrl `
                    -Method GET `
                    -WebSession $clientSession `
                    -TimeoutSec 90

                $clientServers += [string]$response.Nick
            }
            catch {
                Write-Warning "Client $clientNumber, request $requestNumber failed: $($_.Exception.Message)"
                $clientServers += "ERROR"
                $stickyErrors++
            }
        }

        $stickyResults += [PSCustomObject]@{
            Client = $clientNumber
            Servers = $clientServers -join " -> "
        }
    }

    Write-Host "Client Servers"
    Write-Host "------ -------"

    $stickyResults | Format-Table -AutoSize

    Write-Host "Total clients: 20"
    Write-Host "Requests per client: 3"
    Write-Host "Failed requests: $stickyErrors"
}
