# QR Code Feature - Quick Start Guide 🚀

## ✅ What's Been Implemented

Your QR code feature is now complete! Here's what was added:

### 📦 New Files Created
1. **`GetMerchantQRCodeQuery.cs`** - Query handler with QR generation logic
2. **`GetMerchantQRCodeQueryValidator.cs`** - Input validation
3. **`GetMerchantQRCodeFunction.cs`** - Azure Function HTTP endpoint
4. **`add-qrcode-migration.ps1`** - Helper script for database migration

### 🔧 Modified Files
1. **`MerchantSettings.cs`** - Added `QRCodeUrl` property
2. **`DomainConfigurations.cs`** - Added QRCodeUrl configuration
3. **`InstaMenu.Application.csproj`** - Added QRCoder and Configuration packages
4. **`local.settings.template.json`** - Added BaseUrl configuration

---

## 🎯 Next Steps (Required)

### Step 1: Install QRCoder Package
Run this command from the solution directory:
```bash
dotnet restore
```

### Step 2: Run the Migration Script
**Option A: Using the PowerShell Script (Recommended)**
```powershell
.\add-qrcode-migration.ps1
```

**Option B: Manual Commands**
```bash
# Create migration
dotnet ef migrations add AddQRCodeToMerchantSettings --project InstaMenu.Infrastructure --startup-project InstaMenu.Functions

# Update database
dotnet ef database update --project InstaMenu.Infrastructure --startup-project InstaMenu.Functions
```

### Step 3: Update Your local.settings.json
Add this line to your actual `local.settings.json` (not the template):
```json
"AppSettings:BaseUrl": "https://menux.app"
```
Or use your development URL like `http://localhost:3000`

---

## 🧪 Testing the API

### Using Postman/Thunder Client
```http
GET http://localhost:7071/api/merchants/{your-merchant-id}/qrcode
```

### Example Response
```json
{
  "menuUrl": "https://menux.app/a-coffe-2",
  "qrCodeBase64": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA...",
  "qrCodeUrl": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA..."
}
```

### Using cURL
```bash
curl http://localhost:7071/api/merchants/YOUR-MERCHANT-ID/qrcode
```

---

## 🎨 Frontend Integration Example

### React Component
```jsx
import { useState, useEffect } from 'react';

export default function MerchantQRCodePage({ merchantId }) {
  const [qrData, setQrData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch(`/api/merchants/${merchantId}/qrcode`)
      .then(res => res.json())
      .then(data => {
        setQrData(data);
        setLoading(false);
      })
      .catch(error => {
        console.error('Error:', error);
        setLoading(false);
      });
  }, [merchantId]);

  const downloadQRCode = () => {
    const link = document.createElement('a');
    link.href = qrData.qrCodeBase64;
    link.download = `qrcode-${merchantId}.png`;
    link.click();
  };

  if (loading) return <div>جاري التحميل...</div>;
  if (!qrData) return <div>حدث خطأ</div>;

  return (
    <div className="qr-container">
      <div className="qr-code-section">
        <h2>Qr كود الخاص بقائمتك</h2>
        <img 
          src={qrData.qrCodeBase64} 
          alt="QR Code" 
          className="qr-image"
        />
        <button onClick={downloadQRCode} className="download-btn">
          تحميل الكود
        </button>
      </div>

      <div className="menu-link-section">
        <h3>رابط القائمة</h3>
        <div className="link-box">
          <input 
            type="text" 
            value={qrData.menuUrl} 
            readOnly 
            className="menu-url-input"
          />
          <button 
            onClick={() => navigator.clipboard.writeText(qrData.menuUrl)}
            className="copy-btn"
          >
            نسخ
          </button>
        </div>
        <p className="hint">
          قم بطباعة Qr code الخاص بقائمتك وقم بتوجيهه على مناطق الجلوس أو على منافذ الكرتون
          طاولات المطعم أو على رسائل الميديو مباشرة
        </p>
      </div>
    </div>
  );
}
```

### CSS Styling (Optional)
```css
.qr-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  text-align: center;
}

.qr-image {
  max-width: 300px;
  margin: 20px auto;
  border: 2px solid #ddd;
  padding: 10px;
  background: white;
}

.download-btn, .copy-btn {
  background: #00695C;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin: 10px;
}

.menu-url-input {
  width: 70%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  margin-right: 10px;
}

.link-box {
  display: flex;
  justify-content: center;
  align-items: center;
  margin: 20px 0;
}

.hint {
  color: #666;
  font-size: 14px;
  margin-top: 20px;
  line-height: 1.6;
}
```

---

## 📊 How It Works

1. **First Request**:
   - ✅ Check if merchant exists
   - ✅ Check if merchant has completed setup (has slug)
   - ✅ Check if QR code exists in database
   - ✅ If not, generate new QR code
   - ✅ Save to database
   - ✅ Return QR code + menu URL

2. **Subsequent Requests**:
   - ✅ Return existing QR code from database
   - ⚡ No regeneration (faster response)

---

## 🔍 Troubleshooting

### Error: "Merchant slug is not set"
**Solution**: The merchant must complete setup first using the `CompleteMerchantSetup` endpoint

### Error: "Merchant not found"
**Solution**: Make sure the merchant ID is correct

### QR Code doesn't scan
**Solution**: 
- Make sure the BaseUrl is set correctly
- Test with a QR code scanner app
- Ensure the URL is accessible

---

## 📝 Package Information

**QRCoder** (v1.6.0)
- ✅ Completely FREE
- ✅ Open source (MIT License)
- ✅ No limitations
- ✅ No API calls or internet required
- ✅ Works offline
- 🔗 GitHub: https://github.com/codebude/QRCoder

---

## 🚀 Production Deployment

### Environment Variables (Azure)
Add to your Azure Function App Configuration:
```
AppSettings:BaseUrl = https://yourdomain.com
```

### Recommended Future Enhancements
1. **Blob Storage**: Move QR codes to Azure Blob Storage
2. **CDN**: Serve QR codes through Azure CDN
3. **Customization**: Allow custom colors and logos
4. **Analytics**: Track QR code scans
5. **Caching**: Add Redis caching for frequently accessed QR codes

---

## 📞 Support

If you encounter any issues:
1. Check build errors with `dotnet build`
2. Verify database connection
3. Ensure migrations are applied
4. Check local.settings.json configuration

---

**Happy Coding! 🎉**
