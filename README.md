## SocialNetwork

Full‑stack social network demo using ASP.NET Core Web API and Angular.

### Prerequisites
- **.NET 8 SDK**
- **SQL Server** (LocalDB or full SQL Server)
- **Node.js 18+** and **npm**
- **Angular CLI** (`npm i -g @angular/cli`)

### Project Structure
- `API/` — ASP.NET Core Web API
- `client/` — Angular app (v20)

## Backend (API)

1) Configure database connection
- Update `API/appsettings.json` → `ConnectionStrings:DefaultConnection` to match your SQL Server instance.
  - Example: `Server=(localdb)\\MSSQLLocalDB;Database=SocialNetwork;Trusted_Connection=True;Trust Server Certificate=True`

2) Configure JWT secret
- The API expects `PrivateKey` in configuration (see `Program.cs`). Set it via User Secrets or `appsettings.Development.json`.
  - User Secrets (recommended during dev):
    ```bash
    dotnet user-secrets init --project API
    dotnet user-secrets set --project API "PrivateKey" "<your-strong-random-secret>"
    ```

3) Apply migrations
```bash
cd API
dotnet ef database update
```

4) Run the API
```bash
dotnet run --project API
```
By default it serves on `https://localhost:5001`. CORS is enabled for `http://localhost:4200`.

## Frontend (Angular client)

1) Install dependencies
```bash
cd client
npm install
```

2) Verify API base URL
- `client/src/core/services/account-service.ts` uses `https://localhost:5001/api/` by default. Adjust if your API runs elsewhere.

3) Start the dev server
```bash
ng serve
```
Open `http://localhost:4200`.

## Useful Notes
- Postman collection: import `Social Network App.postman_collection.json` to test endpoints.
- CORS: configured in API to allow Angular dev origin(s). If your port/origin differs, update the CORS policy in `Program.cs`.
- HTTPS dev cert: if you see SSL trust issues on Windows, run:
  ```bash
  dotnet dev-certs https --trust
  ```

## Troubleshooting
- SQL connection errors: ensure the connection string matches your SQL Server instance and that the database is reachable.
- 401 Unauthorized: confirm `PrivateKey` is configured and the client points to the correct API URL.
- CORS errors: update the allowed origins in `API/Program.cs` to include your Angular URL.

## Scripts Quickstart
```bash
# Backend
dotnet ef database update --project API
dotnet run --project API

# Frontend
cd client && npm i && ng serve
```
