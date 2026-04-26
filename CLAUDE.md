# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SiwesConnect is a .NET 10.0 ASP.NET Core MVC web application for managing student internships and industrial work experience (SIWES). The platform connects students, companies, supervisors, and administrators in a structured internship management system.

## Technology Stack

- **Framework**: .NET 10.0 with ASP.NET Core MVC
- **Database**: SQL Server with Entity Framework Core 10.0.5
- **Authentication**: ASP.NET Core Identity with role-based authorization
- **Architecture**: MVC pattern with dependency injection

## Common Development Commands

### Building and Running
```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Run in development mode
dotnet run --environment Development
```

### Database Operations
```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# View SQL for migration
dotnet ef migrations script
```

### Testing
```bash
# Run all tests (if test projects exist)
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~TestName"
```

## Architecture

### Domain Model
The application follows a domain-driven design with these core entities:

- **ApplicationUser**: Extends IdentityUser, represents all user types
- **Company**: Represents employer organizations with industry and contact info
- **Internship**: Available internship positions with title, description, duration
- **Application**: Student applications to internships with status tracking
- **Placement**: Final placement assignments linking students, companies, and supervisors
- **LogbookEntry**: Weekly work logs submitted by students for supervisor review

### Role-Based Access Control
The system uses four distinct roles with specific permissions:

- **Admin**: System oversight, view all placements and statistics
- **Student**: Browse internships, submit applications, manage logbook entries
- **Company**: Post internships, review applications
- **Supervisor**: Review logbook entries, approve/reject applications, manage assigned students

### Controller Structure
Controllers are organized by role and responsibility:
- `AccountController`: Authentication (login/register)
- `AdminController`: Administrative functions and oversight
- `StudentController`: Student-specific operations (applications, logbooks)
- `CompanyController`: Company internship management
- `SupervisorController`: Supervisor review and approval workflows

### Database Context
`ApplicationDbContext` extends `IdentityDbContext<ApplicationUser>` and manages all domain entities. Connection string is configured in `appsettings.json` using Windows Authentication with SQL Express.

### View Organization
Views follow standard MVC conventions organized by controller:
- `Views/Account/`: Authentication pages
- `Views/Student/`: Student dashboard and operations
- `Views/Company/`: Company internship management
- `Views/Supervisor/`: Supervisor review interfaces
- `Views/Admin/`: Administrative interfaces
- `Views/Shared/`: Layouts and partial views

## Key Workflows

### Student Application Flow
1. Student browses available internships
2. Submits application with cover letter
3. Supervisor reviews application
4. If accepted, placement is created automatically
5. Student can then submit weekly logbook entries

### Logbook Approval Process
1. Students submit weekly logbook entries
2. Supervisors review entries for their assigned students
3. Supervisors can approve, reject, or add comments
4. Students can resubmit rejected entries

### Database Schema Management
- Use EF Core migrations for all schema changes
- Migrations are stored in `Migrations/` directory
- Always test migrations in development before production deployment

## Configuration

### Database Connection
The default connection uses SQL Express with Windows Authentication. Modify `appsettings.json` for different environments:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SiwesConnectDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### Environment-Specific Settings
- Development: `appsettings.Development.json`
- Production: Use environment variables or secure configuration providers

## Important Notes

- The application uses implicit usings and nullable reference types
- All controllers use dependency injection for `ApplicationDbContext` and `UserManager<ApplicationUser>`
- Authorization is handled via `[Authorize(Roles = "RoleName")]` attributes
- The project follows standard .NET naming conventions and MVC patterns
- Static files are served from `wwwroot/` directory