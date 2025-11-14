# Result Pattern Implementation Guide

## Overview

This implementation provides a clean, simple Result pattern that maintains proper HTTP semantics. The middleware automatically converts Results to appropriate HTTP responses, keeping your Azure Functions free of business logic.

## Key Benefits

? **HTTP Semantics**: HTTP status codes match the actual response status  
? **Clean API**: No nested status codes or confusing responses  
? **Type Safety**: Strong typing with generic `Result<T>`  
? **Consistent Errors**: Standardized error messages  
? **Testable**: Business logic separated from HTTP concerns  
? **Frontend Friendly**: Simple, predictable response format  

## Usage Patterns

### Commands (Operations without return values)

```csharp
// Success
return Result.Success();

// Business errors (will return HTTP 400)
return Result.Failure("Invalid operation");
return Result.Failure(ResultErrors.BadRequest.InvalidData());

// Not found (will return HTTP 404)
return Result.Failure(ResultErrors.NotFound.Merchant(merchantId));

// Validation errors (will return HTTP 400)
return Result.Failure(ResultErrors.Validation.Failed("Name is required"));
```

### Queries (Operations with return values)

```csharp
// Success with data
return Result.Success(merchantData);

// Not found
return Result<MerchantDto>.Failure(ResultErrors.NotFound.Merchant(id));

// Business error
return Result<MerchantDto>.Failure("Cannot retrieve merchant data");
```

### Azure Function Pattern

```csharp
[Function("SomeFunction")]
public async Task<Result> Run([HttpTrigger] HttpRequestData req)
{
    // 1. Validate input (optional)
    var request = await req.ReadFromJsonAsync<SomeRequest>();
    if (request == null)
        return Result.Failure(ResultErrors.BadRequest.InvalidData());

    // 2. Create and send command/query
    var command = new SomeCommand { /* map properties */ };
  return await _mediator.Send(command);
    
 // 3. Middleware handles the rest automatically!
}

// For queries that return data
[Function("GetSomething")]
public async Task<Result<SomeDto>> Run([HttpTrigger] HttpRequestData req)
{
    var query = new GetSomethingQuery { Id = id };
    return await _mediator.Send(query);
}
```

## Response Examples

### Success Response (HTTP 200)
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Sample Restaurant",
  "aboutUs": "We serve great food!"
}
```

### Error Response (HTTP 404)
```json
{
  "error": "Merchant with ID '123e4567-e89b-12d3-a456-426614174000' not found"
}
```

### Error Response (HTTP 400)
```json
{
  "error": "Invalid request data: Name is required"
}
```

## Standard Error Messages

Use the `ResultErrors` class for consistent error messages:

```csharp
// Not Found errors
ResultErrors.NotFound.Merchant(merchantId)
ResultErrors.NotFound.MerchantSettings(merchantId)
ResultErrors.NotFound.Resource("Order", orderId.ToString())

// Bad Request errors
ResultErrors.BadRequest.InvalidData()
ResultErrors.BadRequest.InvalidData("specific reason")
ResultErrors.BadRequest.MissingRequiredFields("name", "email")
ResultErrors.BadRequest.InvalidFormat("email")

// Validation errors
ResultErrors.Validation.Failed("Name must be between 2 and 50 characters")
ResultErrors.Validation.InvalidValue("email", "must be a valid email format")

// Conflict errors
ResultErrors.Conflict.AlreadyExists("Merchant", "slug-name")
ResultErrors.Conflict.StateConflict("Cannot update inactive merchant")

// Server errors
ResultErrors.Server.DatabaseError()
ResultErrors.Server.UnexpectedError()
ResultErrors.Server.ExternalServiceError("Payment Gateway")
```

## Command Handler Pattern

```csharp
public class SomeCommandHandler : IRequestHandler<SomeCommand, Result>
{
    public async Task<Result> Handle(SomeCommand request, CancellationToken cancellationToken)
    {
      try
    {
 // 1. Validate business rules
 var entity = await _context.Entities
       .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
    
      if (entity == null)
        return Result.Failure(ResultErrors.NotFound.Resource("Entity", request.Id.ToString()));

 // 2. Perform business logic
         entity.UpdateProperty(request.Value);
   entity.UpdatedAt = DateTime.UtcNow;

       // 3. Save changes
 await _context.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
     catch (Exception ex)
        {
    // Log the exception in production
            return Result.Failure(ResultErrors.Server.DatabaseError());
        }
    }
}
```

## Query Handler Pattern

```csharp
public class SomeQueryHandler : IRequestHandler<SomeQuery, Result<SomeDto>>
{
    public async Task<Result<SomeDto>> Handle(SomeQuery request, CancellationToken cancellationToken)
    {
        try
        {
    var entity = await _context.Entities
           .Where(e => e.Id == request.Id)
        .Select(e => new SomeDto
   {
  Id = e.Id,
      Name = e.Name
        })
 .FirstOrDefaultAsync(cancellationToken);

         if (entity == null)
 return Result<SomeDto>.Failure(ResultErrors.NotFound.Resource("Entity", request.Id.ToString()));

            return Result.Success(entity);
   }
        catch (Exception ex)
        {
       // Log the exception in production
    return Result<SomeDto>.Failure(ResultErrors.Server.DatabaseError());
        }
    }
}
```

## Middleware Configuration

The `ResultHandlingMiddleware` automatically:
- Detects Result and Result<T> return types
- Maps error messages to appropriate HTTP status codes
- Formats responses consistently
- Must be registered last in the middleware pipeline

## HTTP Status Code Mapping

| Error Pattern | HTTP Status | Example |
|---------------|-------------|---------|
| "not found" | 404 | Merchant not found |
| "already exists", "conflict" | 409 | Email already exists |
| "validation", "invalid", "required" | 400 | Invalid email format |
| "unauthorized", "forbidden" | 401 | Access denied |
| "database", "unexpected" | 500 | Database connection error |
| Default business errors | 400 | General business rule violation |

## Migration Guide

To migrate existing functions:

1. Change return type from `HttpResponseData` to `Result` or `Result<T>`
2. Replace manual response creation with `Result.Success()` or `Result.Failure()`
3. Use `ResultErrors` for consistent error messages
4. Remove HTTP response handling logic (middleware handles this)
5. Update command/query handlers to return Result types