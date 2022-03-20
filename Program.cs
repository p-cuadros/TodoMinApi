using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoMinApi.Data;
using TodoMinApi.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton<ItemRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options => options.UseSqlite(connectionString));
var app = builder.Build();
app.UseSwagger();

app.MapGet("/", () => "Hello World!");

// app.MapGet("/items", ([FromServices] ItemRepository items) =>
// {
//     return items.GetAll();
// });

// app.MapGet("/items/{id}", ([FromServices] ItemRepository items, int id) =>
// {
//     var result = items.GetById(id);
//     return result != null ? Results.Ok(result) :  Results.NotFound();
// });

// app.MapPost("/items", ([FromServices] ItemRepository items, Item item) =>
// {
//     items.Add(item);
//     return Results.Created($"/items/{item.Id}",item);
// });

// app.MapPut("/items/{id}", ([FromServices] ItemRepository items, int id, Item item) =>
// {
//     if (items.GetById(id) == null)
//         return Results.NotFound();
//     items.Update(item);
//     return Results.Ok(item);
// });

// app.MapDelete("/items/{id}", ([FromServices] ItemRepository items, int id) =>
// {
//     if (items.GetById(id) == null)
//         return Results.NotFound();
//     items.Delete(id);
//     return Results.NoContent();
// });

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
