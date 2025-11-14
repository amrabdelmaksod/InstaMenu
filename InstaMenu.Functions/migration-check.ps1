#!/usr/bin/env pwsh

# PowerShell script to systematically update all remaining command and query handlers
# This script will identify files that need updating and provide guidance

Write-Host "=== InstaMenu Result Pattern Migration Tool ===" -ForegroundColor Green
Write-Host ""

# Get all command and query files
$applicationPath = "InstaMenu.Application"
$commandFiles = Get-ChildItem -Path $applicationPath -Recurse -Filter "*Command.cs" | Where-Object { $_.Name -notlike "UpdateMerchantAboutCommand.cs" }
$queryFiles = Get-ChildItem -Path $applicationPath -Recurse -Filter "*Query.cs"

Write-Host "Found Command Files:" -ForegroundColor Yellow
$commandFiles | ForEach-Object { 
    $content = Get-Content $_.FullName -Raw
if ($content -match "IRequest<(?!Result)" -or $content -match "IRequestHandler<[^,]+,\s*(?!Result)") {
 Write-Host "  ? $($_.Name) - Needs Update" -ForegroundColor Red
    } else {
        Write-Host "  ? $($_.Name) - Already Updated" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Found Query Files:" -ForegroundColor Yellow
$queryFiles | ForEach-Object { 
    $content = Get-Content $_.FullName -Raw
    if ($content -match "IRequest<(?!Result)" -or $content -match "IRequestHandler<[^,]+,\s*(?!Result)") {
        Write-Host "  ? $($_.Name) - Needs Update" -ForegroundColor Red
    } else {
        Write-Host "  ? $($_.Name) - Already Updated" -ForegroundColor Green
 }
}

Write-Host ""
Write-Host "Checking Azure Functions..." -ForegroundColor Yellow
$functionFiles = Get-ChildItem -Path "InstaMenu.Functions/Functions" -Filter "*Function.cs"
$functionFiles | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match "Task<HttpResponseData>" -and $content -notmatch "Task<Result") {
        Write-Host "  ? $($_.Name) - Needs Update" -ForegroundColor Red
    } else {
        Write-Host "  ? $($_.Name) - Already Updated" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "=== Next Steps ===" -ForegroundColor Cyan
Write-Host "1. Update remaining command handlers to return Result or Result<T>"
Write-Host "2. Update remaining query handlers to return Result<T>"  
Write-Host "3. Update Azure Functions to return Result or Result<T>"
Write-Host "4. Remove manual HTTP response handling from functions"
Write-Host "5. Test the middleware integration"
Write-Host ""
Write-Host "Example patterns to follow:"
Write-Host "  Commands: IRequest<Result> or IRequest<Result<T>>" -ForegroundColor Green
Write-Host "  Queries:  IRequest<Result<T>>" -ForegroundColor Green
Write-Host "  Functions: Task<Result> or Task<Result<T>>" -ForegroundColor Green