using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoMinApi.Data;
using TodoMinApi.Domain;
var cn = Environment.GetEnvironmentVariable("DYNO");
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(GetHerokuConnectionString("DATABASE_URL")));
var app = builder.Build();
app.UseSwagger();

app.MapGet("/", () => "Hello World!");

app.MapGet("/items", async (ApiDbContext db) =>
    await db.Items.ToListAsync());

app.MapGet("/items/{id}", async (int id, ApiDbContext db) =>
    await db.Items.FindAsync(id)
        is Item item ? Results.Ok(item) : Results.NotFound());

app.MapPost("/items", async (Item item, ApiDbContext db) =>
    {
        db.Items.Add(item);
        await db.SaveChangesAsync();

        return Results.Created($"/items/{item.Id}", item);
    });

app.MapPut("/items/{id}", async (int id, Item item, ApiDbContext db) =>
    {
        var existItem = await db.Items.FindAsync(id);

        if (existItem is null) return Results.NotFound();

        existItem.Title = item.Title;
        existItem.Completed = item.Completed;

        await db.SaveChangesAsync();

        return Results.NoContent();
    });

app.MapDelete("/items/{id}", async (int id, ApiDbContext db) =>
    {
        if (await db.Items.FindAsync(id) is Item item)
        {
            db.Items.Remove(item);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    });

app.UseSwaggerUI();
app.Run();


string GetHerokuConnectionString(string connectionString)
{
    string connectionUrl = Environment.GetEnvironmentVariable(connectionString);
    var databaseUri = new Uri(connectionUrl);
    string db = databaseUri.LocalPath.TrimStart('/');
    string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
    return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
}