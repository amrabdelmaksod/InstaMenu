# PowerShell script to install OpenAPI packages for Azure Functions

Write-Host "Installing OpenAPI/Swagger packages for Azure Functions..." -ForegroundColor Green

# Navigate to the Functions project directory
Set-Location "InstaMenu.Functions"

# Add the OpenAPI package
Write-Host "Adding Microsoft.Azure.Functions.Worker.Extensions.OpenApi package..." -ForegroundColor Yellow
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.OpenApi --version 2.0.0-preview2

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build the project to verify everything is working
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build

Write-Host "OpenAPI setup complete!" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Add OpenAPI attributes to your Azure Functions" -ForegroundColor White
Write-Host "2. Create DTOs for request/response models" -ForegroundColor White
Write-Host "3. Access Swagger UI at: https://localhost:7071/api/swagger/ui" -ForegroundColor White