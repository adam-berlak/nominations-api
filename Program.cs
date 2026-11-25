using Microsoft.EntityFrameworkCore;
using NominationsApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Capture the test DB name once at startup so all requests within a test share one DB.
// A new GUID is generated each time Program.cs runs (once per WithWebHostBuilder call),
// giving each test its own isolated in-memory database.
var testDbName = Guid.NewGuid().ToString();

// Use the two-argument overload so IWebHostEnvironment is resolved from the runtime
// service provider rather than from builder.Environment, which is captured before
// WithWebHostBuilder("Testing") takes effect.
builder.Services.AddDbContext<NominationsDbContext>((serviceProvider, options) =>
{
    var env = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
    if (env.IsEnvironment("Testing"))
        options.UseInMemoryDatabase(testDbName);
    else
        options.UseSqlite(builder.Configuration.GetConnectionString("Nominations")
            ?? throw new InvalidOperationException("Connection string 'Nominations' not found."));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<NominationsDbContext>();
    db.Database.Migrate();

    // Safety net: EF migration history can get out of sync with the actual schema
    // when Railway redeploys with ephemeral storage. These statements are idempotent
    // and ensure the ScheduleRuns table always exists after startup.
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "ScheduleRuns" (
            "Id"              INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            "PipelineSegment" TEXT    NOT NULL,
            "RanAt"           TEXT    NOT NULL,
            "Capacity"        TEXT    NOT NULL,
            "TotalRequested"  TEXT    NOT NULL,
            "Outcome"         TEXT    NOT NULL
        )
        """);
    db.Database.ExecuteSqlRaw("""
        CREATE INDEX IF NOT EXISTS "IX_ScheduleRuns_PipelineSegment"
        ON "ScheduleRuns" ("PipelineSegment")
        """);
}

app.Run();

public partial class Program { }
