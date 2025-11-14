namespace InstaMenu.Application.Common.Results
{
    /// <summary>
    /// Standard business error types that map to HTTP status codes
    /// </summary>
    public static class ResultErrors
    {
        // 400 Bad Request errors
        public static class BadRequest
        {
            public static string InvalidData(string? details = null) =>
                 string.IsNullOrEmpty(details) ? "Invalid request data" : $"Invalid request data: {details}";

            public static string MissingRequiredFields(params string[] fields) =>
                    $"Missing required fields: {string.Join(", ", fields)}";

            public static string InvalidFormat(string field) =>
               $"Invalid format for {field}";

            public static string InvalidCredentials() => "Invalid email or password";

            public static string WeakPassword() => "Password must be at least 8 characters long and contain uppercase, lowercase, number and special character";
        }

        // 404 Not Found errors
        public static class NotFound
        {
            public static string Resource(string resourceType, string? identifier = null) =>
             string.IsNullOrEmpty(identifier)
       ? $"{resourceType} not found"
        : $"{resourceType} with identifier '{identifier}' not found";

            public static string Merchant(Guid? merchantId = null) =>
          merchantId.HasValue ? $"Merchant with ID '{merchantId}' not found" : "Merchant not found";

            public static string MerchantSettings(Guid? merchantId = null) =>
        merchantId.HasValue ? $"Merchant settings for merchant '{merchantId}' not found" : "Merchant settings not found";

            public static string Category(Guid? categoryId = null) =>
         categoryId.HasValue ? $"Category with ID '{categoryId}' not found" : "Category not found";

            public static string MenuItem(Guid? menuItemId = null) =>
    menuItemId.HasValue ? $"Menu item with ID '{menuItemId}' not found" : "Menu item not found";

            public static string Order(Guid? orderId = null) =>
          orderId.HasValue ? $"Order with ID '{orderId}' not found" : "Order not found";

            public static string SocialLink(string? platform = null) =>
        !string.IsNullOrEmpty(platform) ? $"Social link for platform '{platform}' not found" : "Social link not found";

          public static string BusinessHour(string? dayOfWeek = null) =>
       !string.IsNullOrEmpty(dayOfWeek) ? $"Business hour for '{dayOfWeek}' not found" : "Business hour not found";
        }

        // 409 Conflict errors
        public static class Conflict
        {
            public static string AlreadyExists(string resourceType, string identifier) =>
               $"{resourceType} with identifier '{identifier}' already exists";

            public static string StateConflict(string message) => message;

        public static string EmailAlreadyExists(string email) =>
$"Email '{email}' is already registered";

 public static string SlugAlreadyExists(string slug) =>
   $"Slug '{slug}' is already taken";

  public static string CategoryNameExists(string name) =>
    $"Category with name '{name}' already exists for this merchant";

        public static string MenuItemNameExists(string name) =>
   $"Menu item with name '{name}' already exists in this category";
 }

     // 422 Validation errors
  public static class Validation
  {
      public static string Failed(string message) => $"Validation failed: {message}";

          public static string InvalidValue(string field, string reason) =>
    $"Invalid value for {field}: {reason}";

  public static string InvalidEmail(string email) =>
     $"Email '{email}' is not in a valid format";

            public static string InvalidSlug(string slug) =>
 $"Slug '{slug}' contains invalid characters. Use only lowercase letters, numbers, and hyphens";

         public static string InvalidPhoneNumber(string phoneNumber) =>
      $"Phone number '{phoneNumber}' is not in a valid format";

            public static string InvalidUrl(string url) =>
            $"URL '{url}' is not in a valid format";

     public static string InvalidTimeRange(string startTime, string endTime) =>
     $"Start time '{startTime}' must be before end time '{endTime}'";

        public static string InvalidOrderStatus(string status) =>
       $"Order status '{status}' is not valid";

            public static string InvalidSortOrder(int sortOrder) =>
          $"Sort order '{sortOrder}' must be a positive number";

      public static string InvalidPrice(decimal price) =>
     $"Price '{price}' must be greater than 0";
        }

        // 403 Forbidden errors
        public static class Forbidden
{
         public static string InsufficientPermissions() => "You do not have permission to perform this action";

      public static string MerchantMismatch() => "You can only access resources belonging to your merchant";

       public static string InactiveAccount() => "Your account is inactive. Please contact support";

       public static string ResourceOwnership(string resourceType) => 
     $"You do not have permission to access this {resourceType}";
        }

        // 401 Unauthorized errors  
  public static class Unauthorized
      {
         public static string TokenExpired() => "Your session has expired. Please log in again";

         public static string InvalidToken() => "Invalid authentication token";

            public static string MissingToken() => "Authentication token is required";

          public static string LoginRequired() => "You must be logged in to access this resource";
    }

        // 500 Server errors
   public static class Server
        {
      public static string DatabaseError() => "A database error occurred";
      public static string UnexpectedError() => "An unexpected error occurred";
         public static string ExternalServiceError(string service) => $"Error communicating with {service}";
     public static string WhatsAppError() => "Error sending WhatsApp message";
            public static string EmailServiceError() => "Error sending email";
        }

        // Business Logic errors  
        public static class BusinessLogic
        {
            public static string CannotDeleteCategoryWithItems() => "Cannot delete category that contains menu items";

  public static string CannotUpdateInactiveOrder() => "Cannot update order that is already completed or cancelled";

         public static string CannotDeleteLastCategory() => "Cannot delete the last category. Merchant must have at least one category";

    public static string OrderAlreadyProcessed() => "Order has already been processed";

            public static string MenuItemOutOfStock() => "Menu item is currently out of stock";

 public static string InvalidOperationForOrderStatus(string currentStatus, string requestedOperation) =>
     $"Cannot {requestedOperation} order with status '{currentStatus}'";

            public static string MerchantNotActive() => "Merchant account is not active";

            public static string CategoryNotActive() => "Category is not active";

            public static string MenuItemNotAvailable() => "Menu item is not currently available";
 }
    }
}