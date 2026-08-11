# BNS360 API

Graduation-project backend for discovering places, businesses, services, jobs, and properties across Beni Suef.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![API](https://img.shields.io/badge/HTTP_actions-73-blue)](#api-modules)
[![SQL Server](https://img.shields.io/badge/SQL_Server-EF_Core-CC2927?logo=microsoftsqlserver)](https://learn.microsoft.com/ef/core/)
[![Graduation Grade](https://img.shields.io/badge/graduation_grade-Excellent-success)](#project-background)

## Overview

BNS360 was created as a digital guide to Beni Suef. It brings local businesses, craftspeople, job opportunities, properties, and useful services into one platform so users can discover what they need in the governorate and access relevant details.

The repository contains the complete REST API used by the client applications.

## Project Background

- Built as a Computer Science graduation project during the 2023-2024 academic year.
- Delivered by a four-person team covering backend, frontend, Flutter, and UI/UX.
- I served as backend developer and team leader and implemented the complete API.
- Awarded an **Excellent** graduation-project grade.
- The deployed pilot received **200+ user registrations** before it was taken offline.

## Main Features

- Registration, login, email verification, password reset, and refresh tokens.
- Role-based authorization and user-role administration.
- User profiles and Cloudinary image uploads.
- Business directory with categories, search-related data, and ratings.
- Crafts and craftsperson profiles.
- Job publishing and saved jobs.
- Property listings for rent and sale.
- Favorites for quick access to selected records.
- Feedback and review/rating summaries.

## Architecture

```text
BNS360.Apis
    Controllers, middleware, Swagger, dependency injection and mapping
        |
BNS360.Core
    Domain models, DTOs, enums, interfaces and API responses
        |
BNS360.Repository
    EF Core context, migrations, repositories, Identity and email services
```

The solution uses a pragmatic layered architecture that separates HTTP concerns, domain contracts, and persistence implementations.

## Tech Stack

| Area | Technology |
| --- | --- |
| Runtime | .NET 8, ASP.NET Core Web API |
| Persistence | Entity Framework Core, SQL Server |
| Identity | ASP.NET Core Identity, JWT, refresh tokens, RBAC |
| Email and OTP | MailKit, Otp.NET |
| Media | Cloudinary |
| Mapping and docs | AutoMapper, Swagger/OpenAPI |

## Project Structure

```text
BNS360/
|-- BNS360.Apis/        # API host and 12 controllers
|-- BNS360.Core/        # Models, DTOs, enums and contracts
|-- BNS360.Repository/  # EF Core, repositories, migrations and services
`-- BNS360.sln
```

## Getting Started

### Requirements

- .NET 8 SDK
- SQL Server
- SMTP credentials for verification and password-reset emails
- Cloudinary account for image upload features
- EF Core CLI tools

### 1. Clone and restore

```powershell
git clone https://github.com/uosefahmed22/BNS360.git
cd BNS360
dotnet restore
```

### 2. Configure the application

Create a local development file from the safe template:

```powershell
Copy-Item BNS360.Apis/appsettings.Example.json BNS360.Apis/appsettings.Development.json
```

Replace the placeholders under `ConnectionStrings`, `jwtConfig`, `MailSettings`, and `CloudinarySetting`. Keep real credentials out of source control.

### 3. Apply migrations

```powershell
dotnet ef database update --project BNS360.Repository --startup-project BNS360.Apis
```

### 4. Run the API

```powershell
dotnet run --project BNS360.Apis
```

Local launch profiles use `https://localhost:7293` and `http://localhost:5098`. Swagger is available at `/swagger` in Development.

## API Modules

The 12 controllers expose **73 HTTP actions**.

| Module | Base route |
| --- | --- |
| Authentication | `/api/Auth` |
| Businesses | `/api/Business` |
| Categories | `/api/Category` |
| Crafts | `/api/Craft` |
| Craftspeople | `/api/CraftsMen` |
| Favorites | `/api/Favorite` |
| Feedback | `/api/Feedback` |
| Jobs | `/api/Job` |
| Profiles | `/api/Profile` |
| Properties | `/api/Property` |
| Saved jobs | `/api/SavedJobs` |
| User roles | `/api/UserRole` |

Use the generated Swagger document for the complete request and response contracts.

## Project Status

- The former pilot deployment is offline.
- The repository currently has no automated test project; adding coverage for auth and authorization-sensitive repositories is a recommended next step.
- Review token lifetime, CORS, validation, database indexes, and production secret management before redeployment.

## Author

**Youssef Ahmed** - Backend Developer and Team Leader
[LinkedIn](https://www.linkedin.com/in/youssef-ahmed-eg/) | [GitHub](https://github.com/uosefahmed22) | [Portfolio](https://uosefahmed22.github.io/)
