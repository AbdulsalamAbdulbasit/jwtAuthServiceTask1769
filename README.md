# jwtAuthService

A Clean Architecture .NET 8 solution for secure JWT authentication and refresh token management, built with ASP.NET Core Web API, Microsoft Identity, Entity Framework Core, and SQL Server. This project demonstrates best practices in RESTful API design, robust authentication/authorization, error handling, and modular software architecture.

---

## 🚀 Features

- **JWT Bearer Authentication** with secure access and refresh tokens
- **Refresh Token Rotation & Revocation** for enhanced security
- **Microsoft Identity** for user and role management
- **Email confirmation** for user registration
- **Secure password hashing** (via ASP.NET Identity)
- **Comprehensive error handling** via custom middleware
- **Logging** with Serilog (or similar provider)
- **Input validation** using FluentValidation
- **OpenAPI/Swagger** for interactive API documentation
- **Dockerized** API and SQL Server support
- **Dependency Injection** throughout the solution
- **Unit and integration tests** for core logic
- **RESTful resource naming** and correct HTTP methods/status codes
- **CORS configuration** for secure cross-origin requests
- **Session/device tracking** and rate limiting (if implemented)

---

## 🏛️ Architecture Overview

This project follows the **Clean Architecture** pattern, ensuring clear separation of concerns and maintainability:

- **API Layer**: Entry point, controllers, dependency injection, middleware, and API docs.
- **Application Layer**: Application logic, DTOs, validators, and service contracts.
- **Domain Layer**: Business entities, aggregates, and repository/service interfaces.
- **Infrastructure Layer**: Data access (EF Core), Identity, and external integrations.
- **Test Layer**: Automated tests for business logic and API endpoints.

Each layer is decoupled and communicates via interfaces, promoting testability and scalability.

---

## ⚡ Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (local or Docker)
- [Docker](https://www.docker.com/) (optional, for containerized setup)

### Environment Setup

1. **Clone the repository:**
   ```sh
   git clone <your-repo-url>
   cd JWTImplementation
   ```

2. **Configure the database:**
   - Update `appsettings.json` and `appsettings.Development.json` in `jwtAuthService.API/` with your SQL Server connection string and JWT settings.
   - Example JWT section:
     ```json
     "Jwt": {
       "Key": "<your-secret-key>",
       "Issuer": "jwtAuthService",
       "Audience": "jwtAuthServiceUsers",
       "AccessTokenExpirationMinutes": 60,
       "RefreshTokenExpirationDays": 7
     }
     ```

3. **Apply database migrations:**
   ```sh
   dotnet ef database update --project jwtAuthService.Infrastructure --startup-project jwtAuthService.API
   ```

4. **Run the API:**
   ```sh
   dotnet run --project jwtAuthService.API
   ```

   The API will be available at `https://localhost:5001` (or as configured).

### Docker Setup (Optional)

- To run both API and SQL Server in Docker, use the provided `docker-compose.yml` (if available) or create one as needed.

---

## 📖 API Documentation

- **Swagger UI** is available at:  
  `https://localhost:5001/swagger`  
  (or `/swagger` on your running instance)

- Use Swagger to explore endpoints, try requests, and view models.

### Key Endpoints

#### Register
- **POST** `/api/auth/register`
- **Request:**
  ```json
  {
    "email": "user@example.com",
    "password": "YourPassword123!",
    "confirmPassword": "YourPassword123!"
  }
  ```
- **Response:**
  - `200 OK` (confirmation email sent)
  - `400 Bad Request` (validation errors)

#### Login
- **POST** `/api/auth/login`
- **Request:**
  ```json
  {
    "email": "user@example.com",
    "password": "YourPassword123!"
  }
  ```
- **Response:**
  ```json
  {
    "accessToken": "<jwt-token>",
    "refreshToken": "<refresh-token>",
    "expiresIn": 3600
  }
  ```
  - `200 OK` (tokens issued)
  - `400/401` (invalid credentials)

#### Refresh Token
- **POST** `/api/auth/refresh`
- **Request:**
  ```json
  {
    "refreshToken": "<refresh-token>"
  }
  ```
- **Response:**
  ```json
  {
    "accessToken": "<new-jwt-token>",
    "refreshToken": "<new-refresh-token>",
    "expiresIn": 3600
  }
  ```
  - `200 OK` (tokens rotated)
  - `400/401` (invalid/expired token)

#### Logout
- **POST** `/api/auth/logout`
- **Headers:**
  - `Authorization: Bearer <jwt-token>`
- **Response:**
  - `200 OK` (token revoked)

---

## 🛠️ Usage Instructions

- Use [Postman](https://www.postman.com/), [curl](https://curl.se/), or Swagger UI to interact with the API.
- **Register** → **Confirm Email** → **Login** → **Use JWT for Authenticated Requests**
- **Refresh Token** when access token expires.
- **Logout** to revoke refresh tokens.

### Example Authenticated Request
```http
GET /api/notes
Authorization: Bearer <jwt-token>
```

---

## 🔒 Security Measures

- **JWT Signing & Validation:**
  - Tokens are signed with a secure secret key and validated on every request.
- **Refresh Token Rotation:**
  - Each refresh request issues a new refresh token and invalidates the old one (prevents reuse attacks).
- **Password Hashing:**
  - User passwords are securely hashed using ASP.NET Identity's built-in algorithms (PBKDF2 by default).
- **HTTPS Enforcement:**
  - Always use HTTPS in production to protect tokens in transit.
- **CORS Configuration:**
  - Only trusted origins are allowed (configured in API startup).
- **Secret Management:**
  - Secrets (JWT keys, connection strings) should be stored in environment variables or user secrets, not in source code.

---

## 🧪 Testing

- **Unit and Integration Tests:**
  - Business logic and API endpoints are covered by tests in the `jwtAuthService.Test` project.
  - Edge cases and critical paths are prioritized.
- **How to Run:**
  ```sh
  dotnet test
  ```
- **Tools Used:**
  - xUnit, Moq, FluentAssertions (or as implemented)

---

## 📂 Folder Structure

```
jwtAuthService.sln
│
├── jwtAuthService.Domain/         # Core entities, domain models, and interfaces
├── jwtAuthService.Application/    # Use cases, DTOs, validators, service contracts
├── jwtAuthService.Infrastructure/ # EF Core, Identity, persistence, external services
├── jwtAuthService.API/            # Controllers, DI, middleware, Swagger, logging
└── jwtAuthService.Test/           # Unit and integration tests
```

- **Domain**: Business entities, aggregates, and repository/service interfaces.
- **Application**: Application logic, DTOs, validators, and service contracts.
- **Infrastructure**: Data access (EF Core), Identity, and external integrations.
- **API**: Entry point, controllers, dependency injection, middleware, and API docs.
- **Test**: Automated tests for business logic and API endpoints.

---

## 🤝 Contribution Guidelines

1. Fork the repository
2. Create a new branch (`feature/your-feature`)
3. Commit your changes
4. Push to your fork
5. Open a Pull Request

---

## 📄 License

This project is licensed under the terms described in [LICENSE.txt](LICENSE.txt).

---

## 💡 Notes

- Follows Clean Architecture principles for maintainability and testability.
- Uses dependency injection for all services and repositories.
- Implements robust error handling and logging for production readiness.
- All sensitive configuration (e.g., connection strings, JWT secrets) should be managed via environment variables or user secrets in development.

---

For any questions or support, please open an issue or contact the maintainer.
