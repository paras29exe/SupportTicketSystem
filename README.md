# Support Ticket System

This is a small support ticket system made for the .NET bootcamp assessment. Customers can raise tickets and agents can be assigned to them. The project also includes SQL scripts, an ASP.NET Core API, a simple HTML/JavaScript frontend, and unit tests.

## Projects

- `SupportTicketSystem.Core` contains the domain models, DTOs, enums, interfaces, and shared helpers.
- `SupportTicketSystem.Infrastructure` contains the EF Core context, repositories, services, Star Wars API call, JSON writing, and ADO.NET code.
- `SupportTicketSystem.Api` contains the Web API controllers and configuration.
- `SupportTicketSystem.Console` contains the console/LINQ and Star Wars API work.
- `SupportTicketSystem.Tests` contains the unit tests.
- `Frontend` contains the plain HTML, CSS, and JavaScript pages.
- `sql` contains the database creation, seed, query, and stored procedure scripts.

## Clone the repository

Clone the project from GitHub and move into the project folder:

```powershell
git clone https://github.com/paras29exe/SupportTicketSystem.git
cd SupportTicketSystem
```

## Requirements

You will need:

- .NET SDK 8 or later
- SQL Server and SQL Server Management Studio (SSMS), or another SQL Server client
- A browser
- Optional: the VS Code Live Server extension for running the frontend

## Database setup

1. Open SQL Server Management Studio and connect to your SQL Server instance.
2. Create a database named `SupportTicketDb`.
3. Open `sql/01_schema_and_seed.sql` and run it against `SupportTicketDb`. This creates the tables and seed data.
4. Run `sql/02_queries.sql` to try the required queries.
5. Run `sql/03_advanced_sql.sql` to create the view and stored procedures.
6. Check `SupportTicketSystem.Api/appsettings.json` and set the `DefaultConnection` value to match your SQL Server instance. For example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SupportTicketDb;User:***;Password:***;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

If you use SQL Server Express or LocalDB, change the `Server` value as needed. The API must be able to connect to this database before it will start successfully.

After running the SQL scripts in SSMS, run the EF Core migration from the project root so the database and EF model are in sync:

```powershell
dotnet ef database update --project SupportTicketSystem.Infrastructure --startup-project SupportTicketSystem.Api
```

If `dotnet ef` is not installed, install it once with:

```powershell
dotnet tool install --global dotnet-ef
```

The migration files are in `SupportTicketSystem.Infrastructure/Migrations`.

While you are in `appsettings.json`, also check these settings:

- `Pagination:DefaultPageSize` is the page size used when the request does not include `pageSize`.
- `Pagination:MaxPageSize` is the largest page size allowed by the API. The current values are 5 and 20.
- `StarWarsApi:StorageDirectoryPath` is the folder where the Star Wars JSON output is saved.
- `StarWarsApi:FileName` is the name of the JSON output file.

The `StarWarsApi` values are empty in the current settings file, so fill them in before running the console Star Wars exercise. For example:

```json
"Pagination": {
	"DefaultPageSize": 5,
	"MaxPageSize": 20
},
"StarWarsApi": {
	"StorageDirectoryPath": "your\\directory\\path",
	"FileName": "starwars.json"
}
```

## Run the API

From the project root, run:

```powershell
dotnet run --project SupportTicketSystem.Api --launch-profile https
```

Or, in Visual Studio, press `CTRL+F5`.

The API should be available at `https://localhost:7040`. The Swagger page is:

`https://localhost:7040/swagger`

The first time, the browser may show a warning for the local HTTPS certificate. It is safe to trust it for local development if you created the certificate with the .NET SDK.

Some useful endpoints are:

- `GET /api/tickets`
- `GET /api/tickets/{id}`
- `POST /api/tickets`
- `PUT /api/tickets/{id}`
- `GET /api/tickets/by-customer/{customerId}`
- `PATCH /api/tickets/{id}/status`
- `POST /api/tickets/assign/{ticketId}/agent/{agentId}`
- `GET /api/customers`
- `GET /api/customers/{id}`
- `POST /api/customers`
- `PUT /api/customers/{id}`
- `DELETE /api/customers/{id}`

The API uses a default page size of 5 when `pageSize` is not supplied. This value is in `appsettings.json`.

## Run the frontend

Keep the API running first. Then serve the `Frontend` folder as a local website. For example, with VS Code Live Server:

1. Open `Frontend/index.html`.
2. Right-click the file and choose **Open with Live Server**.
3. Open the page at `http://127.0.0.1:5500/index.html`.

The frontend has pages for tickets, customers, and agents. It calls the API at `https://localhost:7040/api`.

If you do not use Live Server, any local static server can be used, but its origin may need to be added to `FrontendOrigins` in `SupportTicketSystem.Api/appsettings.json`.

## Run the tests

From the project root, run:

```powershell
dotnet test SupportTicketSystem.Tests/SupportTicketSystem.Tests.csproj
```

To build the solution, run:

```powershell
dotnet build SupportTicketSystem.slnx
```

## Console project

To run the console exercises, use:

```powershell
dotnet run --project SupportTicketSystem.Console
```

The console project contains the LINQ examples and the asynchronous Star Wars API JSON task.

## Postman

Import `SupportTicketSystem.postman_collection.json` into Postman. It contains sample requests for tickets, customers, agents, filtering, pagination, assignment, and status updates. Change the collection variables if the API URL or database IDs are different.

## Notes

The API uses EF Core for the normal customer and ticket operations and has an ADO.NET path for getting tickets by customer and changing ticket status. Swagger is enabled in the Development environment, so using the `https` launch profile is the easiest way to test the API manually.