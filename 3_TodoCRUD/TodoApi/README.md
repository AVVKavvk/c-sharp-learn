# TodoApi — ASP.NET Core CRUD with PostgreSQL

A production-style REST API built with **C# / ASP.NET Core 8**, following a clean **Controller → Service → Repository** architecture with raw SQL (Dapper / Npgsql), structured JSON logging, Swagger UI, and DB migrations via DbUp.

---

## Tech Stack

| Layer      | Technology                |
| ---------- | ------------------------- |
| Framework  | ASP.NET Core 8            |
| Language   | C# 12                     |
| Database   | PostgreSQL                |
| DB Driver  | Npgsql + Dapper           |
| Migrations | DbUp                      |
| Logging    | Serilog (structured JSON) |
| API Docs   | Swagger / Swashbuckle     |

---

## Project Structure

```
TodoApi/
├── Controllers/
│   └── Todo/
│       └── TodoController.cs       # Route handlers
├── Services/
│   └── Todo/
│       ├── ITodoService.cs
│       └── TodoService.cs          # Business logic
├── Repos/
│   └── Todo/
│       ├── ITodoRepository.cs
│       └── TodoRepository.cs       # Raw SQL queries
├── Models/
│   └── TodoModel.cs                # DB model
├── Dtos/
│   └── Todo/
│       ├── CreateTodoDto.cs
│       └── UpdateTodoDto.cs
├── Logger/
│   ├── AppLog.cs                   # Static logger helper
│   ├── CallerInfoEnricher.cs       # Injects fileName/funcName/lineNumber
│   └── JsonFormatter.cs            # Custom JSON log formatter
├── Migrations/
│   └── 001_create_todos_table.sql  # SQL migration scripts
├── Program.cs                      # App entry point + DI wiring
├── appsettings.json
└── TodoApi.csproj
```

---

## Architecture

```
HTTP Request
    │
    ▼
TodoController          (Controllers/Todo/TodoController.cs)
    │   [ApiController] [Route("api/v1/todos")]
    │
    ▼
TodoService             (Services/Todo/TodoService.cs)
    │   Business logic, validation, throws typed exceptions
    │
    ▼
TodoRepository          (Repos/Todo/TodoRepository.cs)
    │   Raw SQL via Dapper + Npgsql
    │
    ▼
PostgreSQL
```

---

## API Endpoints

| Method   | Route                           | Description               |
| -------- | ------------------------------- | ------------------------- |
| `GET`    | `/api/v1/todos?page=1&limit=20` | Get all todos (paginated) |
| `GET`    | `/api/v1/todos/:id`             | Get todo by ID            |
| `POST`   | `/api/v1/todos`                 | Create a new todo         |
| `PUT`    | `/api/v1/todos/:id`             | Update a todo             |
| `DELETE` | `/api/v1/todos/:id`             | Delete a todo             |

Swagger UI available at: http://localhost:5242/docs

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL running locally (or via Docker)

### 1. Clone the repo

```bash
git clone https://github.com/AVVKavvk/c-sharp-learn.git
cd c-sharp-learn/3_TodoCRUD/TodoApi
```

### 2. Configure the connection string

Edit `appsettings.json`:

```json
{
    "ConnectionStrings": {
        "Postgres": "Host=localhost;Port=5432;Database=tododb;Username=postgres;Password=postgres"
    }
}
```

Or set via environment variable:

```bash
export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=tododb;Username=postgres;Password=postgres"
```

### 3. Run

```bash
dotnet restore
dotnet run
```

Migrations run automatically on startup. The app will:

1. Connect to PostgreSQL
2. Run any pending `.sql` migration files from `Migrations/`
3. Start the HTTP server

---

## Database Migrations

Migrations are managed by **DbUp**. SQL files live in `Migrations/` and are embedded into the binary.

### How it works

- DbUp tracks executed scripts in a `todo_migration_version` table
- Scripts run in filename order (`0001_`, `0002_`, etc.)
- Already-executed scripts are skipped on subsequent startups — safe to run on every boot

### Adding a new migration

1. Create a new numbered SQL file:

```bash
touch Migrations/0002_add_priority_column.sql
```

2. Write your SQL:

```sql
ALTER TABLE todos ADD COLUMN priority INT NOT NULL DEFAULT 0;
```

3. Restart the app — it runs automatically.

### Migration files

| File                          | Description                            |
| ----------------------------- | -------------------------------------- |
| `0001_create_todos_table.sql` | Creates the `todos` table with indexes |

---

## Logging

Structured JSON logs are written to stdout and `logs/app-YYYYMMDD.log`.

Every log line follows this shape:

```json
{
    "timestamp": "2026-03-22T10:45:12.003Z",
    "level": "Information",
    "fileName": "TodoRepository.cs",
    "functionName": "GetAllAsync",
    "lineNumber": 42,
    "message": "Get all todos",
    "extras": {
        "page": "1",
        "limit": "20"
    },
    "exception": null
}
```

### Usage in code

```csharp
// Simple log
AppLog.Info("Todo created");

// With extras
AppLog.Info("Get all todos", extras: new()
{
    ["page"]  = page,
    ["limit"] = limit
});

// Warning
AppLog.Warn("Todo not found", extras: new() { ["id"] = id });

// Error with exception
AppLog.Error("Database error", ex, extras: new() { ["query"] = sql });
```

---

## Dependency Injection

All services are registered in `Program.cs`:

```csharp
// Singleton — one shared DB connection pool for the whole app lifetime
builder.Services.AddSingleton(dataSource);

// Scoped — one instance per HTTP request
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
```

| Lifetime    | C#                   | Use for                       |
| ----------- | -------------------- | ----------------------------- |
| `Singleton` | One instance ever    | DB pool, logger, config       |
| `Scoped`    | One per HTTP request | Repository, Service           |
| `Transient` | New every injection  | Lightweight stateless helpers |

---

## Request / Response Examples

### Swagger Docs

http://localhost:5242/docs/index.html

### CURL

#### Create a todo

```bash
curl -X POST http://localhost:5242/api/v1/todos \
  -H "Content-Type: application/json" \
  -d '{"title": "Buy groceries", "description": "Milk, eggs, bread"}'
```

```json
{
    "id": 1,
    "title": "Buy groceries",
    "description": "Milk, eggs, bread",
    "isCompleted": false,
    "createdAt": "2026-03-22T10:00:00Z",
    "updatedAt": "2026-03-22T10:00:00Z"
}
```

#### Get all todos (paginated)

```bash
curl "http://localhost:5242/api/v1/todos?page=1&limit=10"
```

#### Update a todo

```bash
curl -X PUT http://localhost:5242/api/v1/todos/1 \
  -H "Content-Type: application/json" \
  -d '{"title": "Buy groceries", "isCompleted": true}'
```

#### Delete a todo

```bash
curl -X DELETE http://localhost:5242/api/v1/todos/1
```

---

## Packages

| Package                         | Version | Purpose                |
| ------------------------------- | ------- | ---------------------- |
| `Npgsql`                        | 8.x     | PostgreSQL driver      |
| `Dapper`                        | 2.x     | Lightweight SQL mapper |
| `dbup-postgresql`               | 5.x     | Database migrations    |
| `Serilog.AspNetCore`            | 8.x     | Structured logging     |
| `Serilog.Formatting.Compact`    | 2.x     | JSON log formatter     |
| `Serilog.Enrichers.Environment` | 2.x     | Env/machine enrichment |
| `Swashbuckle.AspNetCore`        | 6.x     | Swagger / OpenAPI docs |

---

## CSharpier: The Opinionated C# Formatter

**CSharpier** is an "opinionated" code formatter for C#. It is heavily inspired by **Prettier** (the famous JavaScript formatter). Its core philosophy is to end all debates about code style by enforcing a single, consistent look across your entire project.

Instead of having 50 different settings for where a brace goes or how many spaces to use, CSharpier parses your code into an abstract syntax tree and reprints it according to its own internal rules.

### Key Features

- **No Configuration Fatigue**: It has very few options. You just run it, and your code is "correct."

- **Fast**: It is built on the Roslyn compiler platform.

- **Team Consistency**: Every developer's code looks identical, regardless of their IDE settings.

### Installation Guide

Since you are working in a modern .NET environment, the **Local Tool** method is the best way to ensure everyone on your team uses the same version.

#### 1. Initialize the Tool Manifest

If you haven't already, create a manifest file in your project root:

```bash
dotnet new tool-manifest
```

#### 2. Install CSharpier Locally

Run this command to add CSharpier to your project:

```bash
dotnet tool install csharpier
```

#### 3. Verify Installation

Check that it is working correctly:

```bash
dotnet csharpier --version
```

### Usage

#### Command Line

To format your entire project from the terminal:

```bash
dotnet csharpier .
```

#### VS Code (Format on Save)

1. Install the CSharpier extension from the VS Code Marketplace.

2. Open your `settings.json` and ensure CSharpier is set as the default formatter for C#:

```json
"[csharp]": {
  "editor.defaultFormatter": "csharpier.csharpier-vscode",
  "editor.formatOnSave": true
}
```

#### Ignoring Files

If you have generated code (like Migrations) that you don't want formatted, create a `.csharpierignore` file in your root:

```txt
# Ignore migrations
Migrations/
# Ignore specific files
Temp.cs
```

---

## Background

This project was built to explore C# / ASP.NET Core coming from a background in **Go (Echo)**, **FastAPI**, **C++ (Drogon)**, and **Node.js (NestJS / Express / Fastify)**. The architecture intentionally mirrors the layered patterns used in those frameworks.
MIT
