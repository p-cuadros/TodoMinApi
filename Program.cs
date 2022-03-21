using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TodoMinApi.Data;
using TodoMinApi.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApiDbContext>(options => options.UseSqlite(connectionString));
//builder.Services.AddScoped(_ => new SqliteConnection(connectionString));
builder.Services.AddSingleton(new ItemRepository(connectionString));
var app = builder.Build();
app.UseSwagger();

//await EnsureDb(app.Services, app.Logger);

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/error");
app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500)).ExcludeFromDescription();

app.MapGet("/", () => "Hello World!");

app.MapGet("/items", ([FromServices] ItemRepository items) => {
    return items.GetAll();
});

app.MapGet("/items/{id}", ([FromServices] ItemRepository items, int id) =>
{
    var result = items.GetById(id);
    return result != null ? Results.Ok(result) :  Results.NotFound();
})
//.WithName("GetTodoById")
.Produces<Item>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/items", ([FromServices] ItemRepository items, Item item) =>
{
    items.Add(item);
    return Results.Created($"/items/{item.Id}",item);
});

app.MapPut("/items/{id}", ([FromServices] ItemRepository items, int id, Item item) =>
{
    if (items.GetById(id) == null)
        return Results.NotFound();
    items.Update(item);
    return Results.Ok(item);
});

app.MapDelete("/items/{id}", ([FromServices] ItemRepository items, int id) =>
{
    if (items.GetById(id) == null)
        return Results.NotFound();
    items.Delete(id);
    return Results.NoContent();
});

// app.MapGet("/items/{id}", async (int id, ApiDbContext db) =>
//     await db.Items.FindAsync(id)
//         is Item item ? Results.Ok(item) : Results.NotFound());

// app.MapPost("/items", async (Item item, ApiDbContext db) =>
//     {
//         db.Items.Add(item);
//         await db.SaveChangesAsync();

//         return Results.Created($"/items/{item.Id}", item);
//     });

// app.MapPut("/items/{id}", async (int id, Item item, ApiDbContext db) =>
//     {
//         var existItem = await db.Items.FindAsync(id);

//         if (existItem is null) return Results.NotFound();

//         existItem.Title = item.Title;
//         existItem.Completed = item.Completed;

//         await db.SaveChangesAsync();

//         return Results.NoContent();
//     });

// app.MapDelete("/items/{id}", async (int id, ApiDbContext db) =>
//     {
//         if (await db.Items.FindAsync(id) is Item item)
//         {
//             db.Items.Remove(item);
//             await db.SaveChangesAsync();
//             return Results.NoContent();
//         }

//         return Results.NotFound();
//     });

app.UseSwaggerUI();
app.Run();


// async Task EnsureDb(IServiceProvider services, ILogger logger)
// {
//     logger.LogInformation("Ensuring database exists at connection string '{connectionString}'", connectionString);

//     using var db = services.CreateScope().ServiceProvider.GetRequiredService<SqliteConnection>();
//     var sql = $@"CREATE TABLE IF NOT EXISTS Todos (
//                 {nameof(Item.Id)} INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
//                 {nameof(Item.Title)} TEXT NOT NULL,
//                 {nameof(Item.Completed)} INTEGER DEFAULT 0 NOT NULL CHECK({nameof(Item.Completed)} IN (0, 1))
//             );";
//     await db.ExecuteAsync(sql);
// }
