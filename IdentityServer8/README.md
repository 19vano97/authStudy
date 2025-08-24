# IdentityServer8

## Overview

**IdentityServer8** is an ASP.NET Core application implementing authentication and authorization using [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/). It provides user management, third-party login integration (e.g., Microsoft), and secure API access via OAuth2 and OpenID Connect. The project uses ASP.NET Identity for user management and Entity Framework Core for data persistence.

## Features

- **User Registration & Login:** Supports local account creation, login, password reset, and email validation.
- **Third-Party Login:** Integrates Microsoft login via OpenID Connect.
- **OAuth2 & OpenID Connect:** Issues JWT tokens for API access, supports PKCE, refresh tokens, and custom scopes.
- **Role Management:** Uses ASP.NET Identity roles for authorization.
- **API Protection:** Secures APIs using JWT Bearer authentication.
- **Customizable Clients & Resources:** Configurable clients, API resources, and scopes via settings.
- **MVC UI:** Razor views for account management, error handling, and home pages.
- **Logging:** Integrated with Serilog for structured logging.
- **FluentValidation:** Model validation for view models.
- **CORS:** Configured to allow specific frontend clients.
- **HTTPS Support:** Runs on both HTTP and HTTPS ports.

## Project Structure

- `Program.cs` – Main entry point, configures services, authentication, IdentityServer, and middleware.
- `Controllers/` – MVC controllers for home, account, and API endpoints.
- `Entities/Account/` – User entity and related domain models.
- `Data/` – Entity Framework Core DbContext and migrations.
- `Models/` – DTOs, view models, constants, and validation logic.
- `Services/` – Business logic for accounts, authentication, and third-party login.
- `Views/` – Razor views for UI.
- `wwwroot/` – Static files (CSS, JS, images, libraries).
- `appsettings.json` – Configuration for IdentityServer, database, and third-party login.
- `logs/` – Serilog log files.

## Configuration

### IdentityServer Settings

Configure clients, API resources, scopes, and own access in `appsettings.json` under `IdentityServerSettings`.

### Microsoft Login

Configure Microsoft login parameters in `appsettings.json` under `MicrosoftLogin`.

### Database

Set the connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or compatible connection string)
- Node.js (optional, for frontend assets)

### Setup

1. **Restore NuGet Packages**
   ```sh
   dotnet restore
   ```

2. **Apply Database Migrations**
   ```sh
   dotnet ef database update
   ```

3. **Run the Application**
   ```sh
   dotnet run
   ```
   The app listens on HTTP port `5168` and HTTPS port `7270`.

4. **Access the UI**
   - Open [https://localhost:7270](https://localhost:7270) in your browser.

### Frontend Integration

- Allowed CORS origins:  
  - `https://localhost:5173`  
  - `https://localhost:7124`  
  - `https://localhost:7188`

### Logging

- Logs are written to `logs/serilog-*.log` using Serilog.

## Development

- **Controllers:**  
  - [`HomeController`](Controllers/HomeController.cs) – Home and privacy pages  
  - [`AccountController`](Controllers/ViewControllers/AccountController.cs) – Account management  
  - [`AuthController`](Controllers/Api/AuthController.cs) – API authentication endpoints

- **DbContext:**  
  - [`IdentityServer8DbContext`](Data/IdentityServer8DbContext.cs)

- **Services:**  
  - [`AccountService`](Services/Implemenrations/AccountService.cs)  
  - [`AuthService`](Services/Implemenrations/AuthService.cs)  
  - [`ThirdPartyLogin`](Services/Implemenrations/ThirdPartyLogin.cs)

- **Models & Validation:**  
  - View models and validators in [`Models/ModelViewModels`](Models/ModelViewModels)

## License

- jQuery, Bootstrap, and other libraries are MIT licensed (see respective LICENSE files).
- Application code is copyright © 2024.

## Contact

For questions or support, contact the project maintainer.
