# Git Setup Script for InstaMenu Project
# Run this script to initialize git and push to GitHub

Write-Host "?? Setting up Git repository for InstaMenu..." -ForegroundColor Green

# Initialize git repository (if not already initialized)
if (-not (Test-Path ".git")) {
    Write-Host "?? Initializing Git repository..." -ForegroundColor Yellow
    git init
} else {
    Write-Host "? Git repository already initialized" -ForegroundColor Green
}

# Add all files
Write-Host "?? Adding files to git..." -ForegroundColor Yellow
git add .

# Check if there are changes to commit
$status = git status --porcelain
if ($status) {
    Write-Host "?? Committing changes..." -ForegroundColor Yellow
    git commit -m "Initial commit: InstaMenu project with secure environment variables"
} else {
    Write-Host "? No changes to commit" -ForegroundColor Green
}

# Set main branch name
Write-Host "?? Setting main branch..." -ForegroundColor Yellow
git branch -M master

# Add remote origin (replace with your repository URL)
Write-Host "?? Adding remote origin..." -ForegroundColor Yellow
$remoteExists = git remote get-url origin 2>$null
if (-not $remoteExists) {
    git remote add origin https://github.com/amrabdelmaksod/InstaMenu.git
} else {
    Write-Host "? Remote origin already exists" -ForegroundColor Green
}

# Push to GitHub
Write-Host "?? Pushing to GitHub..." -ForegroundColor Yellow
try {
    git push -u origin master
    Write-Host "? Successfully pushed to GitHub!" -ForegroundColor Green
} catch {
    Write-Host "? Failed to push. You may need to authenticate with GitHub." -ForegroundColor Red
    Write-Host "?? Try running: git push -u origin master" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "?? Setup complete!" -ForegroundColor Green
Write-Host "?? Next steps:" -ForegroundColor Cyan
Write-Host "1. Configure your local.settings.json with real values" -ForegroundColor White
Write-Host "2. Set up your Azure Function App with environment variables" -ForegroundColor White
Write-Host "3. Deploy to Azure: func azure functionapp publish YourFunctionAppName" -ForegroundColor White
Write-Host ""
Write-Host "?? Important Security Notes:" -ForegroundColor Yellow
Write-Host "- local.settings.json is excluded from git (.gitignore)" -ForegroundColor White
Write-Host "- All sensitive data is now in environment variables" -ForegroundColor White
Write-Host "- Use the template files to set up new environments" -ForegroundColor White