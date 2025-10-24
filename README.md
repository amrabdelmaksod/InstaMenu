# InstaMenu - Restaurant Menu Management System

A comprehensive restaurant menu management system built with .NET 8, Azure Functions, and PostgreSQL. This system allows restaurants to manage their menus, process orders, and handle customer interactions through WhatsApp integration.

## ??? Architecture

- **Domain Layer**: Core business entities and logic
- **Application Layer**: Use cases, commands, queries using MediatR
- **Infrastructure Layer**: Data persistence, external services
- **Functions Layer**: Azure Functions for serverless API endpoints

## ?? Features

- **Merchant Management**: Registration, authentication, and profile management
- **Menu Management**: Categories, menu items with full CRUD operations
- **Order Processing**: Order creation, status tracking, WhatsApp notifications
- **Settings Management**: SEO, branding, business hours, social media links
- **Multi-language Support**: Arabic and English content
- **OpenAPI Documentation**: Complete Swagger documentation for all endpoints

## ??? Tech Stack

- **.NET 8** - Latest .NET framework
- **Azure Functions** - Serverless compute platform
- **PostgreSQL** - Primary database
- **Entity Framework Core** - ORM for data access
- **MediatR** - Mediator pattern implementation
- **Twilio** - WhatsApp messaging integration
- **JWT** - Authentication and authorization
- **OpenAPI/Swagger** - API documentation

## ?? Prerequisites

- .NET 8 SDK
- Azure Functions Core Tools
- PostgreSQL database
- Twilio account (for WhatsApp integration)
- Visual Studio 2022 or VS Code

## ?? Environment Setup

### 1. Clone the Repository
```bash
git clone https://github.com/amrabdelmaksod/InstaMenu.git
cd InstaMenu
```

### 2. Configure Environment Variables

Copy the template file and configure your environment variables:

```bash
# For Azure Functions
cp InstaMenu.Functions/local.settings.template.json InstaMenu.Functions/local.settings.json
```

Edit `local.settings.json` with your actual configuration:

```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "DB_HOST": "your-database-host",
    "DB_PORT": "5432",
    "DB_NAME": "your-database-name",
    "DB_USERNAME": "your-database-username",
    "DB_PASSWORD": "your-database-password",

    "TWILIO_ACCOUNT_SID": "your-twilio-account-sid",
    "TWILIO_AUTH_TOKEN": "your-twilio-auth-token",
    "TWILIO_FROM_NUMBER": "whatsapp:+14155238886",

    "JWT_SECRET": "your-jwt-secret-key-base64",
    "JWT_ISSUER": "InstaMenu",
    "JWT_AUDIENCE": "InstaMenuClient",
    "JWT_EXPIRES_DAYS": "7"
  }
}
```

### 3. Database Setup

Run Entity Framework migrations:

```bash
cd InstaMenu.Infrastructure
dotnet ef database update --startup-project ../InstaMenu.Functions/
```

### 4. Install Dependencies

```bash
dotnet restore
```

### 5. Run the Application

```bash
cd InstaMenu.Functions
func start
```

The API will be available at `http://localhost:7071`

## ?? API Documentation

Once the application is running, access the Swagger UI at:
- **Local**: `http://localhost:7071/api/swagger/ui`
- **Production**: `https://your-function-app.azurewebsites.net/api/swagger/ui`

## ?? Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_HOST` | PostgreSQL host | `localhost` |
| `DB_PORT` | PostgreSQL port | `5432` |
| `DB_NAME` | Database name | `instamenu` |
| `DB_USERNAME` | Database username | `postgres` |
| `DB_PASSWORD` | Database password | `your-password` |
| `TWILIO_ACCOUNT_SID` | Twilio Account SID | `AC...` |
| `TWILIO_AUTH_TOKEN` | Twilio Auth Token | `your-token` |
| `TWILIO_FROM_NUMBER` | WhatsApp number | `whatsapp:+14155238886` |
| `JWT_SECRET` | JWT signing secret | `base64-encoded-secret` |
| `JWT_ISSUER` | JWT issuer | `InstaMenu` |
| `JWT_AUDIENCE` | JWT audience | `InstaMenuClient` |
| `JWT_EXPIRES_DAYS` | JWT expiration (days) | `7` |

## ?? Project Structure

```
InstaMenu/
??? InstaMenu.Domain/           # Domain entities and interfaces
??? InstaMenu.Application/      # Application logic, commands, queries
??? InstaMenu.Infrastructure/   # Data access, external services
??? InstaMenu.Functions/        # Azure Functions (API endpoints)
??? .gitignore                  # Git ignore rules
??? .env.example               # Environment variables template
??? README.md                  # This file
```

## ?? Development Workflow

### Adding New Features

1. **Domain**: Define entities in `InstaMenu.Domain/Entities/`
2. **Application**: Create commands/queries in `InstaMenu.Application/`
3. **Infrastructure**: Add data configurations in `InstaMenu.Infrastructure/Presistence/Configurations/`
4. **Functions**: Create Azure Functions in `InstaMenu.Functions/Functions/`
5. **Documentation**: Add OpenAPI attributes for API documentation

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --startup-project ../InstaMenu.Functions/

# Update database
dotnet ef database update --startup-project ../InstaMenu.Functions/
```

## ?? Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test InstaMenu.Tests/
```

## ?? Deployment

### Azure Functions

1. **Create Azure Function App**
2. **Configure Application Settings** (environment variables)
3. **Deploy using Azure CLI**:

```bash
func azure functionapp publish YourFunctionAppName
```

### Database

1. **Set up Azure Database for PostgreSQL**
2. **Update connection string in Azure Application Settings**
3. **Run migrations against production database**

## ?? Security

- **Environment Variables**: All sensitive data is stored in environment variables
- **JWT Authentication**: Secure API endpoints with JWT tokens
- **SQL Injection Protection**: Entity Framework prevents SQL injection
- **CORS**: Configured for secure cross-origin requests

## ?? API Endpoints

### Authentication
- `POST /api/auth/register` - Register new merchant
- `POST /api/auth/login` - Merchant login

### Menu Management
- `GET /api/menu/{slug}` - Get public menu by slug
- `POST /api/categories` - Create category
- `PUT /api/categories/{id}` - Update category
- `POST /api/menu-items` - Create menu item
- `PUT /api/menu-items/{id}` - Update menu item

### Merchant Settings
- `PUT /api/merchants/{id}/basic-info` - Update basic information
- `PUT /api/merchants/{id}/seo` - Update SEO settings
- `PUT /api/merchants/{id}/branding` - Update branding
- `PUT /api/merchants/{id}/social-links` - Update social media links
- `PUT /api/merchants/{id}/business-hours` - Update business hours

### Orders
- `POST /api/orders` - Create order
- `GET /api/orders/{id}` - Get order details
- `PUT /api/orders/{id}/status` - Update order status
- `POST /api/orders/{id}/send-whatsapp` - Send order to WhatsApp

## ?? Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Authors

- **Amr Abdelmaksoud** - *Initial work* - [amrabdelmaksod](https://github.com/amrabdelmaksod)

## ?? Links

- [GitHub Repository](https://github.com/amrabdelmaksod/InstaMenu)
- [API Documentation](https://your-function-app.azurewebsites.net/api/swagger/ui)
- [Issues](https://github.com/amrabdelmaksod/InstaMenu/issues)

## ?? Status

- ? Core functionality complete
- ? API documentation with OpenAPI/Swagger
- ? Environment variables configuration
- ? Security implementation
- ?? Additional features in development

---

For detailed API documentation, please refer to the [Merchant Settings API](InstaMenu.Functions/MERCHANT-SETTINGS-API.md) and [Implementation Summary](InstaMenu.Functions/IMPLEMENTATION-SUMMARY.md).