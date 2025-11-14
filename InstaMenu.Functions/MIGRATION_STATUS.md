# InstaMenu Result Pattern Implementation Guide

## ? What's Been Implemented

### 1. Core Result Pattern
- `Result` and `Result<T>` classes with clean API
- Comprehensive `ResultErrors` with business-specific error messages
- `ResultHandlingMiddleware` for automatic HTTP response handling

### 2. Updated Commands & Queries ?
- ? `UpdateMerchantAboutCommand`
- ? `RegisterMerchantCommand`
- ? `LoginMerchantCommand`
- ? `CreateCategoryCommand`
- ? `UpdateCategoryCommand`
- ? `DeleteCategoryCommand`
- ? `UpdateMerchantCommand`
- ? `CreateMenuItemCommand`
- ? `GetMenuBySlugQuery`
- ? `CreateOrderCommand`
- ? `GetOrderByIdQuery`

### 3. Updated Functions ?
- ? `UpdateMerchantAboutFunction`
- ? `LoginMerchantFunction`
- ? `CreateOrderFunction`

## ?? Files That Still Need Updating

### Commands to Update:
```
InstaMenu.Application\Merchants\Commands\UpdateMerchantSeoCommand.cs
InstaMenu.Application\Merchants\Commands\UpdateMerchantBusinessHoursCommand.cs
InstaMenu.Application\Merchants\Commands\UpdateMerchantSocialLinksCommand.cs
InstaMenu.Application\Merchants\Commands\DeleteMerchantSocialLinkCommand.cs
InstaMenu.Application\Merchants\Commands\UpdateMerchantBrandingCommand.cs
InstaMenu.Application\Merchants\Commands\BulkToggleMenuItemsAvailabilityCommand.cs
InstaMenu.Application\Orders\Commands\SendOrderToWhatsAppCommand.cs
InstaMenu.Application\Merchants\Commands\UpdateMerchantGoogleTagManagerCommand.cs
InstaMenu.Application\Orders\Commands\UpdateOrderStatusCommand.cs
InstaMenu.Application\Merchants\Commands\UpsertMerchantSocialLinkCommand.cs
InstaMenu.Application\Merchants\Commands\UpsertBusinessHourCommand.cs
InstaMenu.Application\MenuItems\Commands\UpdateMenuItemCommand.cs
InstaMenu.Application\MenuItems\Commands\DeleteMenuItemCommand.cs
```

### Queries to Update:
```
InstaMenu.Application\Merchants\Queries\GetCurrentMerchantQuery.cs
InstaMenu.Application\Merchants\Queries\GetMerchantOrdersQuery.cs
```

### Functions to Update:
```
InstaMenu.Functions\Functions\SendOrderToWhatsAppFunction.cs
InstaMenu.Functions\Functions\UpdateMerchantSeoFunction.cs
InstaMenu.Functions\Functions\UpdateMerchantBrandingFunction.cs
InstaMenu.Functions\Functions\SeedDemoDataFunction.cs
InstaMenu.Functions\Functions\UpsertBusinessHourFunction.cs
InstaMenu.Functions\Functions\GetOrderByIdFunction.cs
InstaMenu.Functions\Functions\GetMerchantOrdersFunction.cs
InstaMenu.Functions\Functions\UpdateMerchantBasicInfoFunction.cs
InstaMenu.Functions\Functions\CreateCategoryFunction.cs
InstaMenu.Functions\Functions\UpdateOrderStatusFunction.cs
InstaMenu.Functions\Functions\UpdateMerchantSocialLinksFunction.cs
InstaMenu.Functions\Functions\UpdateCategoryFunction.cs
InstaMenu.Functions\Functions\GetCurrentMerchantFunction.cs
InstaMenu.Functions\Functions\DeleteMenuItemFunction.cs
InstaMenu.Functions\Functions\GetMenuBySlugFunction.cs
InstaMenu.Functions\Functions\UpdateMenuItemFunction.cs
InstaMenu.Functions\Functions\UpsertMerchantSocialLinkFunction.cs
InstaMenu.Functions\Functions\UpdateMerchantGoogleTagManagerFunction.cs
InstaMenu.Functions\Functions\CreateMenuItemFunction.cs
InstaMenu.Functions\Functions\RegisterMerchantFunction.cs
InstaMenu.Functions\Functions\UpdateMerchantBusinessHoursFunction.cs
InstaMenu.Functions\Functions\DeleteMerchantSocialLinkFunction.cs
InstaMenu.Functions\Functions\DeleteCategoryFunction.cs
```

## ?? Quick Migration Pattern

### For Commands (No return data):
```csharp
// OLD:
public class SomeCommand : IRequest<bool>
public async Task<bool> Handle(SomeCommand request, CancellationToken cancellationToken)
{
  // business logic
    if (entity == null) return false;
    // save
    return true;
}

// NEW:
public class SomeCommand : IRequest<Result>
public async Task<Result> Handle(SomeCommand request, CancellationToken cancellationToken)
{
    try 
    {
        // business logic with validation
   if (entity == null) 
    return Result.Failure(ResultErrors.NotFound.SomeEntity(id));
    
        // save
        return Result.Success();
    }
    catch (Exception ex) 
{
        return Result.Failure(ResultErrors.Server.DatabaseError());
    }
}
```

### For Commands/Queries (With return data):
```csharp
// OLD:
public class SomeQuery : IRequest<SomeDto?>
public async Task<SomeDto?> Handle(SomeQuery request, CancellationToken cancellationToken)
{
    var entity = await _context.Entities.FindAsync(request.Id);
    return entity == null ? null : new SomeDto { ... };
}

// NEW:
public class SomeQuery : IRequest<Result<SomeDto>>
public async Task<Result<SomeDto>> Handle(SomeQuery request, CancellationToken cancellationToken)
{
    try 
    {
   var entity = await _context.Entities.FindAsync(request.Id);
        if (entity == null)
            return Result<SomeDto>.Failure(ResultErrors.NotFound.SomeEntity(request.Id));
         
      var dto = new SomeDto { ... };
        return Result.Success(dto);
    }
    catch (Exception ex) 
    {
    return Result<SomeDto>.Failure(ResultErrors.Server.DatabaseError());
    }
}
```

### For Azure Functions:
```csharp
// OLD:
public async Task<HttpResponseData> Run([HttpTrigger] HttpRequestData req)
{
    var result = await _mediator.Send(command);
    var response = req.CreateResponse(HttpStatusCode.OK);
    await response.WriteAsJsonAsync(result);
    return response;
}

// NEW:
public async Task<Result> Run([HttpTrigger] HttpRequestData req)  // or Result<T>
{
    var request = await req.ReadFromJsonAsync<SomeRequest>();
  if (request == null)
        return Result.Failure(ResultErrors.BadRequest.InvalidData());
        
    var command = new SomeCommand { /* map properties */ };
    return await _mediator.Send(command);
}
```

## ?? Validation & Testing Checklist

After updating each file:
1. ? Command/Query returns `Result` or `Result<T>`
2. ? Function returns `Result` or `Result<T>`
3. ? Proper error handling with try-catch
4. ? Use `ResultErrors` for consistent messages
5. ? Validate input parameters
6. ? Check business rules
7. ? Remove manual HTTP response handling from functions
8. ? Test compilation with `dotnet build`

## ?? Benefits After Full Implementation

- **Frontend Integration**: Clean HTTP status codes, no nested errors
- **Consistency**: All APIs follow the same error response format
- **Maintainability**: Centralized error messages and handling
- **Testing**: Easy to unit test business logic separately
- **Documentation**: Clear API responses for OpenAPI/Swagger

## ?? Migration Command

Run this PowerShell script to check progress:
```powershell
.\migration-check.ps1
```

This will show you which files still need updating and provide examples.