# Multi-Tenant Application with IdentityDbContext in .NET Core 8 and SQL Server

## Overview

This sample project demonstrates a multi-tenant architecture using ASP.NET Core 8 and SQL Server. The application is designed with a centralized Master database that handles global tenant information and authorization through its own `IdentityDbContext`. Additionally, each tenant has its own database with a separate `IdentityDbContext` for tenant-specific authorization and identity management.

To protect sensitive data, such as passwords and personally identifiable information (PII), **SQL Server’s Always Encrypted** feature is highly recommended to ensure data is encrypted both at rest and in transit.


## Features
- **Multi-Tenant Architecture**: Supports multiple tenants with isolated databases.
- **Entity Framework Core**: Code-first approach for database migrations and management.
- **SQL Server**: Separate databases for each tenant.
- **ASP.NET Core 8**: Built on the latest .NET Core 8 framework.

## Technologies Used
- **ASP.NET Core 8**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core Identity**
- **SQL Server Always Encrypted** for sensitive data
- **C#**
- **Code-First Migrations**

## Prerequisites
- .NET Core 8 SDK
- SQL Server (local or remote) with **Always Encrypted** feature enabled
- Visual Studio or any preferred IDE
- Azure Key Vault or other certificate


### Tenant Database Connection Methods

In this multi-tenant system, there are two approaches to connect to the appropriate tenant database:

1. **For Users Not Logged In**:  
   The application uses the `TenantContextProvider` function `SetTenantDbContextAsync` to establish a connection to the tenant database based on the tenant's **ID** or **name**. This method is typically used when the user is not authenticated or logged in, and the tenant can be resolved based on the request (e.g., through subdomain routing or a query parameter).

   Example usage:
   ```csharp
   await _tenantContextProvider.SetTenantDbContextAsync(tenantIdOrName);
   ```
2. **For Logged-In Users or JWT Token Authorization**:
For authenticated users, particularly when using JWT tokens, the tenant is resolved from the TenantId present in the token payload. This is handled by the TenantMiddleware, which extracts the TenantId from the JWT token and sets the appropriate tenant database context.

##### Example workflow:

The JWT token contains the TenantId in its payload.
The TenantMiddleware intercepts the request, extracts the TenantId, and establishes the tenant context.
This method ensures that the correct tenant database is selected based on the authenticated user's JWT token.

### Database Migrations

#### Main Database Migration
If changes are made in the **Main Database**, use the following commands to apply migrations:

1. **Add Migration**:
   ```bash
   Add-Migration InitialMigration -Context MainDbContext -Project Infrastructure -StartupProject MultiTenant -OutputDir Persistence\Data\Migrations\MainDbContextData    
    ```
2. **Update Database**:    
    ```bash
    Update-Database -Context MainDbContext
    ```
#### Tenant Database Migration
For the **Tenant Database**, use the following commands:
1. **Add Migration**:
   ```bash
   Add-Migration InitialMigration001 -Context TenantDbContext -Project Infrastructure -StartupProject MultiTenant -OutputDir Persistence\Data\Migrations\TenantDbContextData\Migrations
    ```
2. **Update Database**:
    ```bash
    Update-Database -Context TenantDbContext
    ``` 
 
 ### 1. Clone the Repository
```bash
git clone https://github.com/your-username/multi-tenant-app.git
```


## Contributing
Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

This package is free to use for any purpose.
