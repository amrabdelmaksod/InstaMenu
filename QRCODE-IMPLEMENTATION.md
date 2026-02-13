# QR Code Feature Implementation

## Overview
This implementation adds QR code generation functionality for merchants. Each merchant can have a unique QR code that redirects to their menu page.

## Changes Made

### 1. Database Changes
- **MerchantSettings Entity**: Added `QRCodeUrl` property to store the generated QR code as a base64 string
- **Configuration**: Updated `MerchantSettingsConfiguration` in `DomainConfigurations.cs`

### 2. Application Layer
- **Query**: `GetMerchantQRCodeQuery.cs` - Handles QR code retrieval and generation
- **DTO**: `MerchantQRCodeDto` - Response model containing:
  - `MenuUrl`: The merchant's menu URL (e.g., https://menux.app/{slug})
  - `QRCodeBase64`: Base64 encoded QR code image (data URI format)
  - `QRCodeUrl`: Same as QRCodeBase64 (for backwards compatibility)

### 3. Functions/API Layer
- **Function**: `GetMerchantQRCodeFunction.cs` - Azure Function endpoint
- **Route**: `GET /api/merchants/{merchantId}/qrcode`

### 4. NuGet Package
- **QRCoder v1.6.0**: Free, open-source QR code generator library
  - No limitations
  - Fully free for commercial use
  - License: MIT

## Setup Instructions

### Step 1: Restore NuGet Packages
```bash
dotnet restore
```

### Step 2: Create Database Migration
Run the following command from the solution root directory:

```bash
dotnet ef migrations add AddQRCodeToMerchantSettings --project InstaMenu.Infrastructure --startup-project InstaMenu.Functions
```

### Step 3: Update Database
```bash
dotnet ef database update --project InstaMenu.Infrastructure --startup-project InstaMenu.Functions
```

### Step 4: Update Base URL Configuration
In `GetMerchantQRCodeQueryHandler`, update the `_baseUrl` to match your domain:
- **Development**: `https://menux.app` or `http://localhost:3000`
- **Production**: Your actual domain

**Better approach**: Move this to `appsettings.json` or environment variables.

## Usage

### API Endpoint
**GET** `/api/merchants/{merchantId}/qrcode`

### Request Example
```http
GET /api/merchants/123e4567-e89b-12d3-a456-426614174000/qrcode
```

### Response Example
```json
{
  "menuUrl": "https://menux.app/a-coffe-2",
  "qrCodeBase64": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA...",
  "qrCodeUrl": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA..."
}
```

### Frontend Integration

#### React/Next.js Example
```jsx
const MerchantQRCode = ({ merchantId }) => {
  const [qrData, setQrData] = useState(null);

  useEffect(() => {
    fetch(`/api/merchants/${merchantId}/qrcode`)
      .then(res => res.json())
      .then(data => setQrData(data));
  }, [merchantId]);

  if (!qrData) return <div>Loading...</div>;

  return (
    <div>
      <h2>رابط القائمة</h2>
      <p>{qrData.menuUrl}</p>
      
      <h2>QR Code</h2>
      <img src={qrData.qrCodeBase64} alt="Merchant QR Code" />
      
      <button onClick={() => {
        // Download QR code
        const link = document.createElement('a');
        link.href = qrData.qrCodeBase64;
        link.download = `qrcode-${merchantId}.png`;
        link.click();
      }}>
        تحميل الكود
      </button>
    </div>
  );
};
```

## How It Works

1. **First Request**: 
   - Check if merchant exists
   - Check if merchant has a slug (required)
   - Check if QR code already exists in database
   - If not exists, generate new QR code using QRCoder library
   - Save QR code to database
   - Return QR code and menu URL

2. **Subsequent Requests**:
   - Return existing QR code from database
   - No regeneration needed

## QR Code Details

- **Format**: PNG image encoded as base64 data URI
- **Size**: 20px per module (adjustable in code)
- **Error Correction Level**: Q (25% recovery capability)
- **Data**: Contains the full menu URL

## Notes

- The QR code is stored as a base64 string in the database to avoid file system dependencies
- For production with many merchants, consider storing QR codes as files in blob storage (Azure Blob Storage, AWS S3, etc.)
- The base URL should be configured via environment variables for different environments

## Future Improvements

1. **Configuration Service**: Move base URL to configuration
2. **Blob Storage**: Store QR codes in Azure Blob Storage for better scalability
3. **Customization**: Allow merchants to customize QR code colors and logo
4. **Analytics**: Track QR code scans
5. **Multiple QR Codes**: Support different QR codes for different purposes (menu, social media, etc.)
