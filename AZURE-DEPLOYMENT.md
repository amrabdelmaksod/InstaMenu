# Azure Deployment Guide for InstaMenu

This guide will help you deploy the InstaMenu application to Azure.

## Prerequisites

- Azure CLI installed and logged in
- Azure Functions Core Tools
- Azure subscription

## Step 1: Create Azure Resources

### 1.1 Create Resource Group
```bash
az group create --name InstaMenu-RG --location "East US"
```

### 1.2 Create Azure Database for PostgreSQL
```bash
az postgres flexible-server create \
  --resource-group InstaMenu-RG \
  --name instamenu-db \
  --location "East US" \
  --admin-user adminuser \
  --admin-password "YourStrongPassword123!" \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --public-access 0.0.0.0 \
  --storage-size 32
```

### 1.3 Create Azure Function App
```bash
az functionapp create \
  --resource-group InstaMenu-RG \
  --consumption-plan-location "East US" \
  --runtime dotnet-isolated \
  --functions-version 4 \
  --name instamenu-api \
  --storage-account instamenustore001
```

### 1.4 Create Storage Account (if needed)
```bash
az storage account create \
  --name instamenustore001 \
  --resource-group InstaMenu-RG \
  --location "East US" \
  --sku Standard_LRS
```

## Step 2: Configure Application Settings

Set all environment variables in Azure Function App:

```bash
# Database Configuration
az functionapp config appsettings set --name instamenu-api --resource-group InstaMenu-RG \
  --settings DB_HOST="instamenu-db.postgres.database.azure.com" \
             DB_PORT="5432" \
             DB_NAME="postgres" \
             DB_USERNAME="adminuser" \
             DB_PASSWORD="YourStrongPassword123!"

# Twilio Configuration
az functionapp config appsettings set --name instamenu-api --resource-group InstaMenu-RG \
  --settings TWILIO_ACCOUNT_SID="your-twilio-sid" \
             TWILIO_AUTH_TOKEN="your-twilio-token" \
             TWILIO_FROM_NUMBER="whatsapp:+14155238886"

# JWT Configuration
az functionapp config appsettings set --name instamenu-api --resource-group InstaMenu-RG \
  --settings JWT_SECRET="your-base64-jwt-secret" \
             JWT_ISSUER="InstaMenu" \
             JWT_AUDIENCE="InstaMenuClient" \
             JWT_EXPIRES_DAYS="7"
```

## Step 3: Database Setup

### 3.1 Configure Database Firewall
```bash
# Allow Azure services
az postgres flexible-server firewall-rule create \
  --resource-group InstaMenu-RG \
  --name instamenu-db \
  --rule-name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow your IP for migrations
az postgres flexible-server firewall-rule create \
  --resource-group InstaMenu-RG \
  --name instamenu-db \
  --rule-name AllowMyIP \
  --start-ip-address YOUR_IP \
  --end-ip-address YOUR_IP
```

### 3.2 Run Database Migrations
Update your local.settings.json with Azure database connection string, then:

```bash
cd InstaMenu.Infrastructure
dotnet ef database update --startup-project ../InstaMenu.Functions/
```

## Step 4: Deploy Application

### 4.1 Build and Deploy
```bash
cd InstaMenu.Functions
func azure functionapp publish instamenu-api
```

### 4.2 Verify Deployment
- Check function app logs: `az functionapp logs tail --name instamenu-api --resource-group InstaMenu-RG`
- Test API endpoints: `https://instamenu-api.azurewebsites.net/api/swagger/ui`

## Step 5: Configure Custom Domain (Optional)

### 5.1 Add Custom Domain
```bash
az functionapp config hostname add \
  --webapp-name instamenu-api \
  --resource-group InstaMenu-RG \
  --hostname yourdomain.com
```

### 5.2 Configure SSL Certificate
```bash
az functionapp config ssl bind \
  --certificate-thumbprint YOUR_CERT_THUMBPRINT \
  --ssl-type SNI \
  --name instamenu-api \
  --resource-group InstaMenu-RG
```

## Step 6: Monitoring and Logging

### 6.1 Enable Application Insights
```bash
az monitor app-insights component create \
  --app instamenu-insights \
  --location "East US" \
  --resource-group InstaMenu-RG

# Link to Function App
az functionapp config appsettings set \
  --name instamenu-api \
  --resource-group InstaMenu-RG \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY="your-insights-key"
```

## Step 7: Production Environment Variables

Make sure to update these values for production:

```bash
# Generate strong JWT secret
JWT_SECRET=$(openssl rand -base64 32)

# Use production Twilio credentials
TWILIO_ACCOUNT_SID="your-production-sid"
TWILIO_AUTH_TOKEN="your-production-token"
TWILIO_FROM_NUMBER="your-verified-whatsapp-number"

# Use production database
DB_HOST="your-production-db-host"
DB_PASSWORD="your-strong-production-password"
```

## Security Checklist

- [ ] Database firewall configured properly
- [ ] Strong passwords for all services
- [ ] JWT secret is securely generated
- [ ] Application Insights enabled for monitoring
- [ ] SSL certificate configured
- [ ] API keys are stored in Azure Key Vault (optional)
- [ ] CORS properly configured
- [ ] Rate limiting implemented (if needed)

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check firewall rules
   - Verify connection string
   - Ensure database is accessible from Azure

2. **Function App Not Starting**
   - Check application settings
   - Review function app logs
   - Verify all required environment variables are set

3. **API Endpoints Not Working**
   - Check CORS configuration
   - Verify authentication settings
   - Review function bindings

### Useful Commands

```bash
# View function app logs
az functionapp logs tail --name instamenu-api --resource-group InstaMenu-RG

# Restart function app
az functionapp restart --name instamenu-api --resource-group InstaMenu-RG

# List application settings
az functionapp config appsettings list --name instamenu-api --resource-group InstaMenu-RG

# Scale function app
az functionapp plan update --name instamenu-plan --resource-group InstaMenu-RG --sku S1
```

## Cost Optimization

- Use consumption plan for low traffic
- Scale database based on actual usage
- Monitor Application Insights costs
- Set up billing alerts

## Backup Strategy

- Enable automated database backups
- Export application settings regularly
- Keep deployment scripts in source control
- Document recovery procedures

---

For more detailed information, refer to the [Azure Functions documentation](https://docs.microsoft.com/en-us/azure/azure-functions/) and [Azure Database for PostgreSQL documentation](https://docs.microsoft.com/en-us/azure/postgresql/).