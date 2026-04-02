using DbUp;

var connectionString = args.Length > 0
    ? args[0]
    : Environment.GetEnvironmentVariable("CONNECTION_STRING")
      ?? "Host=postgres;Port=5432;Database=telemetry;Username=telemetry;Password=telemetry";

EnsureDatabase.For.PostgresqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
    .WithTransactionPerScript()
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.Error.WriteLine($"Migration failed: {result.Error}");
    return 1;
}

Console.WriteLine("Migrations applied successfully.");
return 0;
