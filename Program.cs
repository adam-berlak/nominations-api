using Microsoft.EntityFrameworkCore;
using NominationsApi.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC controllers
builder.Services.AddControllers();

// OpenAPI document at /openapi/v1.json (package ships with the webapi template)
builder.Services.AddOpenApi();

// EF Core + SQLite. DbContext is registered as scoped (one per request).
builder.Services.AddDbContext<NominationsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Nominations")
        ?? throw new InvalidOperationException("Connection string 'Nominations' not found.")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NominationsDbContext>();
    db.Database.Migrate();
}

app.Run();
