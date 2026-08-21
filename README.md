# BNS360

BNS360 is an ASP.NET Core 8 Web API for managing businesses, craftsmen, jobs, properties, favorites, feedback, user profiles, and role-based access. It uses SQL Server for persistence, ASP.NET Core Identity with JWT authentication, Cloudinary for image storage, and SMTP for account and password-recovery emails.

## Features

- Registration, email confirmation, login, password change, and password recovery.
- Short-lived JWT access tokens with hashed, rotating refresh tokens and token revocation.
- Role-based authorization for users, administrators, business owners, and craftsmen.
- Business, category, craft, craftsman, job, property, profile, favorite, feedback, and saved-job management.
- Ownership checks that prevent a user from updating or deleting another user's job.
- Image upload validation with file-size, extension, MIME-type, and file-signature checks.
- Centralized exception handling and consistent HTTP status codes.
- Authentication lockout and rate limiting for authentication and email endpoints.
- Scalar API documentation in the Development environment.

## Project Structure

```text
BNS360.sln
├── BNS360.Apis        HTTP controllers, middleware, authentication, configuration, and composition root
├── BNS360.Core        Models, DTOs, repository contracts, and service contracts
├── BNS360.Repository  EF Core persistence, Identity, repositories, mapping, email, and authentication services
└── BNS360.Tests       Focused xUnit tests for OTP security and job ownership
```

The current compile-time dependency direction is:

```text
BNS360.Apis → BNS360.Repository → BNS360.Core
BNS360.Tests → BNS360.Repository
```

This is a layered architecture. It should not be described as strict Clean Architecture because `Core` currently references ASP.NET Core Identity and transport-related packages.

## Technology Stack

- .NET 8 and ASP.NET Core Web API
- Entity Framework Core 8 and SQL Server
- ASP.NET Core Identity
- JWT Bearer authentication
- MailKit for SMTP email
- CloudinaryDotNet for image storage
- Serilog for structured logging
- Scalar and Swagger/OpenAPI
- xUnit and EF Core InMemory for tests

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server Express/LocalDB
- A Cloudinary account
- An SMTP account. For Gmail, enable two-step verification and use an App Password instead of the normal account password.

## Configuration and Secrets

The tracked [`appsettings.json`](BNS360.Apis/appsettings.json) intentionally contains no credentials. Development secrets belong in `BNS360.Apis/appsettings.Development.json`, which is ignored by Git.

Create that file locally with the following structure and replace the placeholders only on your machine:

```json
{
  "AllowedOrigins": [
    "https://localhost:4200",
    "http://localhost:4200"
  ],
  "PublicBaseUrl": "https://localhost:7293",
  "JwtConfig": {
    "Issuer": "BNS360.Apis",
    "Audience": "BNS360.Client",
    "Secret": "<at-least-32-bytes-random-secret>",
    "ExpirationInMinutes": 60
  },
  "MailSettings": {
    "Port": 587,
    "SmtpServer": "smtp.gmail.com",
    "Email": "<smtp-email>",
    "DisplayedName": "BNS360",
    "Password": "<smtp-app-password>"
  },
  "CloudinarySetting": {
    "CloudName": "<cloud-name>",
    "ApiKey": "<api-key>",
    "ApiSecret": "<api-secret>"
  },
  "ConnectionStrings": {
    "DefaultConnection": "<sql-server-connection-string>"
  }
}
```

`PublicBaseUrl` must be an absolute HTTPS URL. HTTP is accepted only for a loopback development address. The JWT secret must contain at least 32 UTF-8 bytes.

For production, use the deployment platform's secret manager or environment variables instead of configuration files. ASP.NET Core maps nested configuration keys using double underscores:

```text
ConnectionStrings__DefaultConnection
JwtConfig__Issuer
JwtConfig__Audience
JwtConfig__Secret
MailSettings__Email
MailSettings__Password
CloudinarySetting__CloudName
CloudinarySetting__ApiKey
CloudinarySetting__ApiSecret
PublicBaseUrl
AllowedOrigins__0
```

Never commit `appsettings.Development.json`, SMTP passwords, JWT secrets, connection strings, or Cloudinary credentials. If a credential was committed previously, removing it from the file is not enough; revoke and rotate it at the provider.

## Restore and Build

From the repository root:

```bash
dotnet restore BNS360.sln
dotnet build BNS360.sln --no-restore
```

## Database Migrations

Apply the migrations after configuring `DefaultConnection`:

```bash
dotnet ef database update --project BNS360.Repository/BNS360.Repository.csproj --startup-project BNS360.Apis/BNS360.Apis.csproj
```

> The `SecurityHardening` migration removes legacy plaintext refresh tokens, invalidates existing refresh-token sessions, removes invalid favorite/feedback records, and deduplicates saved jobs and favorites before adding database constraints. Back up an existing database and review the migration before applying it.

To verify that the EF Core model matches the latest migration:

```bash
dotnet ef migrations has-pending-model-changes --project BNS360.Repository/BNS360.Repository.csproj --startup-project BNS360.Apis/BNS360.Apis.csproj
```

## Run the API

```bash
dotnet run --project BNS360.Apis/BNS360.Apis.csproj
```

The Development launch profiles open Scalar automatically:

- Scalar: `https://localhost:7293/scalar/v1`
- OpenAPI document: `https://localhost:7293/openapi/v1.json`
- HTTP fallback: `http://localhost:5098`

Scalar and the OpenAPI document are exposed only in the Development environment.

## Authentication Notes

The password-recovery flow is intentionally split into three steps:

1. Call `POST /api/Auth/forget-password` with the email address.
2. Call `POST /api/Auth/verify-otp` with the email and OTP. A successful response returns a single-use reset token.
3. Call `POST /api/Auth/reset-password` with the email, new password, password confirmation, and returned reset token.

The OTP expires after five minutes and is invalidated after five failed attempts. The reset token expires after ten minutes and can be consumed only once. This storage is currently in process memory, so a distributed cache or database-backed implementation is required before running multiple API instances.

Changing or resetting a password revokes the user's active refresh tokens.

## API Areas

| Area | Base route | Access summary |
|---|---|---|
| Authentication | `/api/Auth` | Public authentication endpoints and authenticated password change |
| Businesses | `/api/Business` | Public reads; business-owner/admin writes |
| Categories | `/api/Category` | Public reads; admin writes |
| Crafts | `/api/Craft` | Public reads; admin writes |
| Craftsmen | `/api/CraftsMen` | Public reads; craftsman/admin writes |
| Jobs | `/api/Job` | Public reads; authenticated owner writes |
| Properties | `/api/Property` | Public reads; authenticated user writes |
| Favorites | `/api/Favorite` | Authenticated users |
| Feedback | `/api/Feedback` | Public reads; authenticated users write |
| Saved jobs | `/api/SavedJobs` | Authenticated users |
| Profiles | `/api/Profile` | Authenticated users |
| Roles | `/api/UserRole` | Administrators |

Use Scalar for the exact request models, query parameters, and response schemas.

## Tests and Security Audit

Run the automated tests:

```bash
dotnet test BNS360.sln --no-build
```

The current suite contains focused tests for:

- Job update/delete ownership enforcement.
- Owner-authorized job deletion.
- OTP verification and single-use reset tokens.
- OTP invalidation after repeated failed attempts.

Check direct and transitive NuGet dependencies for known vulnerabilities:

```bash
dotnet list BNS360.sln package --vulnerable --include-transitive
```

## Logging and Error Handling

- `GlobalExceptionHandler` converts unhandled failures into a safe JSON response and records the server-side exception with its trace ID.
- Validation failures return structured field errors.
- API responses use their actual HTTP status codes.
- Development startup lifetime messages are suppressed; request and error logs remain available through Serilog.
- Do not log access tokens, refresh tokens, OTPs, passwords, SMTP credentials, or connection strings.
