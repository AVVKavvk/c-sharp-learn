using System.Reflection;
using DbUp;

namespace TodoApi.Migrations;

public static class Migrator
{
    public static void Run(string connectionSting)
    {
        // ensure the database exists
        EnsureDatabase.For.PostgresqlDatabase(connectionSting);

        var upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionSting)
            .JournalToPostgresqlTable("public", "todo_migration_version")
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransactionPerScript() // each migration runs in its own transaction
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (result.Successful)
        {
            Console.WriteLine("Migration Ran Success!");
        }
        else
        {
            throw new Exception($"Migration failed: {result.Error.Message}");
        }
    }
}
