using System.Reflection;
using Npgsql;
using Serilog;
using Serilog.Events;
using TodoApi.Logger;
using TodoApi.Migrations;
using TodoApi.Repos.Todo;
using TodoApi.Services.Todo;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // ── suppress ASP.NET Core system logs ─────────────────────────────
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    // ──────────────────────────────────────────────────────────────────
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.File(
        formatter: new JsonFormatter(),
        path: "logs/app-.log",
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var connString =
    builder.Configuration.GetConnectionString("Postgres")
    ?? "Host=localhost;Port=5432;Database=tododb;Username=postgres;Password=postgres";

Console.WriteLine($"Using connection string: {connString}");

// migrations
Migrator.Run(connString);

var dataSource = NpgsqlDataSource.Create(connString);
builder.Services.AddSingleton(dataSource);

//  DI
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Todo API", Version = "v1" });

    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
    );
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    options.RoutePrefix = "docs"; // → http://localhost:5000/docs
});

app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
