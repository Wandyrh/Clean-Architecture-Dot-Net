# Clean Architecture .NET Portfolio Project

This project is a backend portfolio demonstration by **Wandy Rodríguez**, showcasing software development using **.NET C#** and the **Clean Architecture** pattern. It is designed to illustrate best practices in maintainable, scalable, and testable backend API development.

---

## Table of Contents

- [About the Project](#about-the-project)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Requirements](#requirements)
- [Setup & Configuration](#setup--configuration)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [AppSettings Configuration](#appsettings-configuration)
- [Author](#author)

---

## About the Project

This API provides CRUD operations for Products, Product Categories, and Users, with authentication and authorization using JWT. It is intended as a demonstration of backend development skills and Clean Architecture principles.

---

## Tech Stack

- .NET 8 (C#)
- ASP.NET Core Web API
- Entity Framework Core
- AutoMapper
- FluentValidation
- JWT Authentication

---

## Architecture

The solution follows the Clean Architecture pattern, with clear separation of concerns:

- **Domain**: Entities and repository interfaces
- **Application**: Business logic, DTOs, services, validators, mappings
- **Infrastructure**: Data access, repository implementations, EF Core context
- **WebApi**: API controllers, authentication, configuration

---

## Requirements

- [.NET 8 SDK or later](https://dotnet.microsoft.com/download)
- SQL Server (or compatible, update connection string as needed)
- (Optional) Docker (for containerized deployment)

---

## Setup & Configuration

1. **Clone the repository:**
   ```bash
   git clone <repo-url>
   cd Clean-Architecture-Dot-Net
   ```

2. **Configure the database:**
   - Update the connection string in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
     }
     ```

3. **Configure JWT settings in `appsettings.json`:**
   ```json
   "JWTOptions": {
     "Issuer": "your-issuer",
     "Audience": "your-audience",
     "Secret": "your-very-strong-secret-key",
     "ExpiryHours": 24
   }
   ```

4. **Apply database migrations:**
   ```bash
   dotnet ef database update --project CleanArchitecture.Infrastructure
   ```

5. **Run the application:**
   ```bash
   dotnet run --project CleanArchitecture.WebApi
   ```

---

## Running the Application

- The API will be available at `https://localhost:5001` or `http://localhost:5000` by default.
- Use Swagger UI (if enabled) at `/swagger` for interactive API documentation.

---

## API Endpoints

### Authentication

- `POST /api/v1/authentication/login`
  - Request: `{ "userName": "email", "password": "password" }`
  - Response: `{ "accessToken": "...", "expiresAt": "..." }`

### Products

- `GET /api/v1/products`
- `GET /api/v1/products/{id}`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`
- `GET /api/v1/products/paged?page=1&pageSize=10`

### Product Categories

- `GET /api/v1/productcategories`
- `GET /api/v1/productcategories/{id}`
- `POST /api/v1/productcategories`
- `PUT /api/v1/productcategories/{id}`
- `DELETE /api/v1/productcategories/{id}`
- `GET /api/v1/productcategories/paged?page=1&pageSize=10`

### Users

- `GET /api/v1/users`
- `GET /api/v1/users/{id}`
- `POST /api/v1/users`
- `PUT /api/v1/users/{id}`
- `DELETE /api/v1/users/{id}`
- `GET /api/v1/users/paged?page=1&pageSize=10`

---

## Security

- **Authentication**: JWT Bearer tokens are required for all endpoints except `/authentication/login`.
- **Authorization**: Endpoints are protected with `[Authorize]` attributes.
- **Password Storage**: Passwords are hashed using SHA-256 before storage.

---

## AppSettings Configuration

Key sections in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
},
"JWTOptions": {
  "Issuer": "your-issuer",
  "Audience": "your-audience",
  "Secret": "your-very-strong-secret-key",
  "ExpiryHours": 24
}
```

---

## Author

**Wandy Rodríguez**  
Portfolio project for backend software development using .NET 8 and Clean Architecture.

---
