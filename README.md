# Coffee4You

A small two-part app:
- **`coffee4you.server/`** — ASP.NET Core 8 Web API with EF Core + SQL Server
- **`coffee4you.client/coffee-4-you/`** — React + TypeScript front-end (Vite)

## Prerequisites

Install once:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) (any LTS works)
- [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) running locally

Make sure your SQL Server instance name matches the connection string in `coffee4you.server/appsettings.json` (currently `Server=.\MSSQLSERVER01;...`). If yours is `.\SQLEXPRESS` or something else, edit that line.

## First-time setup

```powershell
# from the repo root
cd coffee4you.client\coffee-4-you
npm install
```

The server has no separate install step; `dotnet run` will restore dependencies on first launch; creates + seeds the database on first start.

## Running app

Open **two terminals** at the repo root.

**Terminal 1 — server:**
```powershell
dotnet run --project coffee4you.server
```

**Terminal 2 — client:**
```powershell
cd coffee4you.client\coffee-4-you
npm run dev
```

Then open **http://localhost:5173** in your browser.


http://localhost:5131/swagger to explore api

## Running the tests

```powershell
dotnet test coffee4you.server.tests
```

26 tests. Uses an in-memory SQLite database

## Stopping

`Ctrl+C` in each terminal.
