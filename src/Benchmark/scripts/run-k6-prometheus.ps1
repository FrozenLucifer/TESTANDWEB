#!/bin/pwsh

# Проверка здоровья сервисов
try {
    $response1 = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 5
    $response2 = Invoke-WebRequest -Uri "http://localhost:5001/health" -UseBasicParsing -TimeoutSec 5
} catch {
    Write-Host "Not working " -ForegroundColor Yellow
    exit 1
}

$efcoreScript = Join-Path $PSScriptRoot "efcore.ps1"
$efcoreCommand = @"
Write-Host "Starting EFCore Tests..." -ForegroundColor Magenta
& "$efcoreScript"
"@

$rawsqlScript = Join-Path $PSScriptRoot "rawsql.ps1"
$rawsqlCommand = @"
Write-Host "Starting RawSQL Tests..." -ForegroundColor Magenta
& "$rawsqlScript"
"@

# Start-Process powershell -ArgumentList "-NoExit", "-Command", $efcoreCommand

# Start-Sleep -Seconds 2
# 
Start-Process powershell -ArgumentList "-NoExit", "-Command", $rawsqlCommand

Write-Host "   Grafana: http://localhost:3000 (admin/admin)" -ForegroundColor Yellow
Write-Host "   Prometheus: http://localhost:9090" -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")