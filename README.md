# EastencherAPI

A production-ready ASP.NET Core 8 Web API for comprehensive user authentication, role-based access control, and product management. The API implements JWT-based security, multi-role authorization, and enterprise-grade CRUD operations with full audit logging and transaction support.

**Key Features:**
- JWT Authentication & Authorization with Role-Based Access Control (RBAC)
- User Management with profile picture support
- Product Management with admin-restricted operations
- Email notifications and OTP verification
- Comprehensive audit logging and error handling
- Soft and permanent delete operations
- Transaction-safe database operations

---

## Prerequisites

Before setting up the project, ensure you have the following installed:

- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server 2019 or later** or **SQL Server Express**
- **SQL Server Management Studio (SSMS)** (optional, for direct DB management)
- **Postman** or similar API testing tool
- **Visual Studio 2022** or **Visual Studio Code** with C# extension

---

## Project Setup Instructions

### 1. Clone the Repository
git clone https://github.com/Madhu-D025/product-management-api.git cd EastencherAPI
### 2. Configure Database Connection

Open `appsettings.json` and update the connection string:
"ConnectionStrings": { "myconn": "Server=YOUR_SERVER_NAME\SQLEXPRESS;Database=EastencherProductDB;Trusted_Connection=True;TrustServerCertificate=True" }
**Replace `YOUR_SERVER_NAME`** with your SQL Server instance name (e.g., `DESKTOP-AF2MUVU\\SQLEXPRESS`).

### 3. Restore Dependencies
dotnet restore
### 4. Build the Project
dotnet build
## Database Setup Options

### Option 1: Migrate Database (Recommended)

Using **Package Manager Console** in Visual Studio:
Add-Migration InitialCreate Update-Database
Or using **.NET CLI**:
dotnet ef migrations add InitialCreate dotnet ef database update

**What each command does:**
- `Add-Migration InitialCreate` → Creates a migration script based on your DbContext models
- `Update-Database` → Applies the migration and creates all tables in the database

### Option 2: Use Existing Database

If you already have a pre-created database:

1. **Restore the Database** using SSMS:
   - Right-click **Databases** → **Restore Database**
   - Select your backup file
   - Complete the restore process

2. **Update Connection String** in `appsettings.json` to match your database name

3. **Verify Schema** matches the project's DbContext models

---

## Application Flow Setup (Critical Order)

Follow this sequence to properly set up and test the API:

### Step 1: Configure Database Connection
- Update `appsettings.json` with your SQL Server connection string
- Ensure database is created and migrated

### Step 2: Run the API
dotnet run

The API will start on `https://localhost:7214` or `http://localhost:5214`

### Step 3: Setup Initial Data (One-time)

Using **SSMS** or **Postman**, create the following:

1. **Create Client** (if not seeded):
   - ClientId: `EastencherApp`

2. **Create Roles**:
   - Role Name: `Admin` (IsActive: true)
   - Role Name: `User` (IsActive: true)

3. **Create Admin User FIRST**:
   - Username: `admin`
   - Email: `admin@gmail.com`
   - Password: `Admin@12345#` (must include uppercase, lowercase, digit, special char)
   - Role: `Admin`

4. **Create Normal User**:
   - Username: `user1`
   - Email: `user1@gmail.com`
   - Password: `User1@12345#`
   - Role: `User`

### Step 4: Authenticate & Generate JWT Token

Use **Postman** to login:
POST: http://localhost:5214/api/AuthController/Login Body (JSON): { "email": "admin@gmail.com", "password": "Admin@12345#" }

**Response:**
{ "success": true, "message": "Login successful", "data": { "token": "eyJhbGciOiJIUzI1NiIs...", "userID": "550e8400-e29b-41d4-a716-446655440000" } }

### Step 5: Test Product APIs

All Product APIs require authentication. Add token to every request:

**Authorization Header:**

---

## API Testing (Postman Guide)

### 1. Get JWT Token

### 2. Add Token to Headers

In Postman:
- Go to **Authorization** tab
- Select **Bearer Token**
- Paste the token from login response

### 3. Test Product APIs

#### Create Product (Admin Only)

#### Get All Products

#### Delete Product (Admin Only)


---

## Default Credentials

Use these credentials for immediate testing if database is pre-configured:

### Admin User

| Field    | Value              |
|----------|-------------------|
| Email    | admin@gmail.com   |
| Password | Admin@12345#      |
| ClientId | EastencherApp     |
| Role     | Admin             |

### Regular User

| Field    | Value              |
|----------|-------------------|
| Email    | user1@gmail.com   |
| Password | User1@12345#      |
| ClientId | EastencherApp     |
| Role     | User              |

---

## Project Structure Overview


**Key Layers:**

- **Controllers** → HTTP endpoints and request handling
- **Services** → Core business logic and data operations
- **Models** → Database entities (DbFirst or CodeFirst)
- **DTOs** → Request/Response data transfer objects
- **DbContext** → Entity Framework configuration and data access
- **Middleware** → Cross-cutting concerns (logging, error handling)

---

## Security Features

- **JWT Authentication** → Stateless token-based authentication
- **Role-Based Authorization** → Admin and User roles with permission control
- **Password Encryption** → TripleDES encryption for sensitive data
- **Transaction Support** → ACID compliance for critical operations
- **Audit Logging** → Complete operation tracking for compliance
- **Email Verification** → OTP-based email confirmation

---

## Important Notes

⚠️ **Authentication Required:**
- All Product APIs require a valid JWT token in the `Authorization` header
- Token must be prefixed with `Bearer ` (e.g., `Bearer eyJhbGciOiJIUzI1NiIs...`)

⚠️ **Admin-Only Operations:**
- Create Product
- Update Product
- Delete Product

⚠️ **User-Access Operations:**
- Get All Products
- Get Product by ID

---

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database name is correct

### JWT Token Errors
- Token may have expired (default: 30 minutes)
- Ensure `Authorization` header format is: `Bearer {token}`
- Re-login to get a fresh token

### Permission Denied (401/403)
- Verify user has Admin role for write operations
- Check user's active status in database
- Ensure token is valid and not expired

---

## Support & Contact

For issues or questions, refer to the repository: [GitHub - Product Management API](https://github.com/Madhu-D025/product-management-api)

---

**Last Updated:** April 2026  
**Version:** 1.0.0  
**.NET Target:** 8.0





