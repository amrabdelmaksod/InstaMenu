# Merchant Settings API Endpoints

This document provides comprehensive information about all Azure Functions created for managing merchant settings in the InstaMenu API.

## Overview

The merchant settings API provides granular control over different aspects of a merchant's profile and configuration. Each endpoint is designed to handle specific settings to ensure better security, maintainability, and user experience.

## Authentication

All merchant settings endpoints require authentication. The merchant can only update their own settings.

## API Endpoints

### 1. Basic Merchant Information

#### Update Basic Info
- **Endpoint**: `PUT /api/merchants/{merchantId}/basic-info`
- **Function**: `UpdateMerchantBasicInfoFunction`
- **Description**: Updates fundamental merchant information
- **Request Body**:
```json
{
  "name": "Restaurant Name",
  "nameAr": "??? ??????",
  "slug": "restaurant-slug",
  "status": 1
}
```
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data (Name and Slug required)
  - `404 Not Found` - Merchant not found
  - `401 Unauthorized` - Authentication required

### 2. SEO Settings

#### Update SEO Settings
- **Endpoint**: `PUT /api/merchants/{merchantId}/seo`
- **Function**: `UpdateMerchantSeoFunction`
- **Description**: Updates search engine optimization settings
- **Request Body**:
```json
{
  "seoTitle": "Best Restaurant in City",
  "seoTitleAr": "???? ???? ?? ???????",
  "seoDescription": "Delicious food and great service",
  "seoDescriptionAr": "???? ???? ????? ??????"
}
```
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant settings not found
  - `401 Unauthorized` - Authentication required

### 3. Branding Settings

#### Update Branding
- **Endpoint**: `PUT /api/merchants/{merchantId}/branding`
- **Function**: `UpdateMerchantBrandingFunction`
- **Description**: Updates visual branding elements
- **Request Body**:
```json
{
  "logoUrl": "https://example.com/logo.png",
  "coverImageUrl": "https://example.com/cover.jpg"
}
```
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant settings not found
  - `401 Unauthorized` - Authentication required

### 4. Google Tag Manager

#### Update Google Tag Manager Settings
- **Endpoint**: `PUT /api/merchants/{merchantId}/google-tag-manager`
- **Function**: `UpdateMerchantGoogleTagManagerFunction`
- **Description**: Updates Google Tag Manager configuration
- **Request Body**:
```json
{
  "googleTagManagerId": "GTM-XXXXXXX",
  "isGoogleTagManagerEnabled": true
}
```
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant settings not found
  - `401 Unauthorized` - Authentication required

### 5. About Section

#### Update About Information
- **Endpoint**: `PUT /api/merchants/{merchantId}/about`
- **Function**: `UpdateMerchantAboutFunction`
- **Description**: Updates about us section content
- **Request Body**:
```json
{
  "aboutUs": "We are a family restaurant...",
  "aboutUsAr": "??? ???? ?????..."
}
```
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant settings not found
  - `401 Unauthorized` - Authentication required

### 6. Social Media Links

#### Update All Social Links (Bulk)
- **Endpoint**: `PUT /api/merchants/{merchantId}/social-links`
- **Function**: `UpdateMerchantSocialLinksFunction`
- **Description**: Replaces all social media links with new set
- **Request Body**:
```json
{
  "socialLinks": [
    {
      "platform": "facebook",
      "url": "https://facebook.com/restaurant",
      "displayOrder": 1
    },
    {
      "platform": "instagram",
      "url": "https://instagram.com/restaurant",
      "displayOrder": 2
    }
  ]
}
```
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant not found
  - `401 Unauthorized` - Authentication required

#### Add/Update Single Social Link
- **Endpoint**: `POST /api/merchants/{merchantId}/social-links`
- **Function**: `UpsertMerchantSocialLinkFunction`
- **Description**: Creates or updates a single social media link
- **Request Body**:
```json
{
  "platform": "twitter",
  "url": "https://twitter.com/restaurant",
  "displayOrder": 3
}
```
- **Status Codes**:
  - `200 OK` - Created/Updated successfully
  - `400 Bad Request` - Invalid data (Platform and Url required)
  - `404 Not Found` - Merchant not found
  - `401 Unauthorized` - Authentication required

#### Delete Social Link
- **Endpoint**: `DELETE /api/merchants/{merchantId}/social-links/{platform}`
- **Function**: `DeleteMerchantSocialLinkFunction`
- **Description**: Soft deletes a social media link
- **Path Parameters**:
  - `merchantId` - The merchant UUID
  - `platform` - The social media platform name
- **Status Codes**:
  - `204 No Content` - Deleted successfully
  - `404 Not Found` - Social link not found
  - `401 Unauthorized` - Authentication required

### 7. Business Hours

#### Update All Business Hours (Bulk)
- **Endpoint**: `PUT /api/merchants/{merchantId}/business-hours`
- **Function**: `UpdateMerchantBusinessHoursFunction`
- **Description**: Replaces all business hours with new schedule
- **Request Body**:
```json
{
  "businessHours": [
    {
      "dayOfWeek": 0,
      "openTime": "09:00:00",
      "closeTime": "22:00:00",
      "isClosed": false
    },
    {
      "dayOfWeek": 1,
      "openTime": "09:00:00",
      "closeTime": "22:00:00",
      "isClosed": false
    },
    {
      "dayOfWeek": 6,
      "openTime": "00:00:00",
      "closeTime": "00:00:00",
      "isClosed": true
    }
  ]
}
```
- **Notes**:
  - `dayOfWeek`: 0=Sunday, 1=Monday, ..., 6=Saturday
  - Times in HH:MM:SS format
- **Status Codes**:
  - `200 OK` - Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant settings not found
  - `401 Unauthorized` - Authentication required

#### Add/Update Single Business Hour
- **Endpoint**: `POST /api/merchants/{merchantId}/business-hours`
- **Function**: `UpsertBusinessHourFunction`
- **Description**: Creates or updates business hours for a specific day
- **Request Body**:
```json
{
  "dayOfWeek": 2,
  "openTime": "10:00:00",
  "closeTime": "21:00:00",
  "isClosed": false
}
```
- **Validation**:
  - `dayOfWeek` must be 0-6
  - `openTime` must be before `closeTime` (unless `isClosed` is true)
- **Status Codes**:
  - `200 OK` - Created/Updated successfully
  - `400 Bad Request` - Invalid data
  - `404 Not Found` - Merchant settings not found
  - `401 Unauthorized` - Authentication required

## API Tags

All endpoints are organized under the `Merchant Settings` tag in the OpenAPI/Swagger documentation.

## Error Responses

All endpoints follow a consistent error response format:

```json
{
  "message": "Descriptive error message"
}
```

## Common HTTP Status Codes

- **200 OK** - Request successful
- **201 Created** - Resource created successfully
- **204 No Content** - Resource deleted successfully
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Authentication required
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server error

## Data Transfer Objects (DTOs)

All request and response models are defined in `InstaMenuFunctions.DTOs.ApiModels` with proper validation and documentation.

## Usage Examples

### Complete Merchant Setup Flow

1. **Update Basic Info**
2. **Configure SEO Settings**
3. **Upload Branding Assets**
4. **Set Business Hours**
5. **Add Social Media Links**
6. **Configure About Section**
7. **Setup Analytics (GTM)**

### Recommended Update Order

For new merchants, we recommend updating settings in this order:
1. Basic Information (required)
2. Business Hours (for customer information)
3. Branding (visual identity)
4. About Section (customer trust)
5. Social Media Links (customer engagement)
6. SEO Settings (discoverability)
7. Google Tag Manager (analytics)

## Notes

- All timestamps are automatically managed by the system
- Soft deletes are used for social media links
- Business hours support both 24-hour format and closed days
- SEO settings support bilingual content (English/Arabic)
- All settings are optional except basic merchant information