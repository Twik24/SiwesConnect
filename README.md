# SiwesConnect

A web based SIWES internship management system that connects students, companies, supervisors and administrators. Built as a Final Year Project at Precious Cornerstone University.

## Demo
[Link to demo video]

## Features
- Four roles with role based access control: Admin, Student, Company, Supervisor
- Company registration and supervisor to company linking
- Students browse internships and apply with a cover letter
- Supervisors review applications and add comments
- Application deadline enforcement
- Placement creation and tracking, with admin monitoring
- Digital weekly logbook: students submit entries, supervisors verify them, and duplicate entries are prevented
- REST API with JWT authentication, CORS and Swagger documentation

## Tech Stack
- ASP.NET Core MVC (.NET 10), Entity Framework Core, SQL Server
- ASP.NET Identity (cookie authentication for the Razor views)
- JWT authentication for the API consumed by a separate JavaScript frontend

## Architecture
- MVC controllers for the web interface
- API controllers for applications, authentication, internships, logbooks, placements and users
- Entities: ApplicationUser, Company, Internship, Application, Placement, LogbookEntry

## Testing
Evaluated with 14 system test cases. [Link or summary]

## Running Locally
1. Clone the repository
2. Set the connection string in `appsettings.json`
3. Set a JWT secret key using user secrets
4. Run `dotnet ef database update`
5. Run `dotnet run`

## My Role
Backend developer and primary editor of the project report. Built with [teammates' names].

## Status
Demonstrated on a local server. Not deployed to production.
