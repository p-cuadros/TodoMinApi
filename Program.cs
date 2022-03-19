using Microsoft.AspNetCore.Mvc;
using TodoMinApi.Data;
using TodoMinApi.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ItemRepository>();
var app = builder.Build();
app.UseSwagger();

app.MapGet("/", () => "Hello World!");

app.MapGet("/items", ([FromServices] ItemRepository items) =>
{
    return items.GetAll();
});

app.MapGet("/items/{id}", ([FromServices] ItemRepository items, int id) =>
{
    var result = items.GetById(id);
    return result != null ? Results.Ok(result) :  Results.NotFound();
});

app.MapPost("/items", ([FromServices] ItemRepository items, Item item) =>
{
    items.Add(item);
    return Results.Created($"/items/{item.Id}",item);
});

app.MapPut("/items/{id}", ([FromServices] ItemRepository items, int id, Item item) =>
{
    if (items.GetById(id) == null)
    {
        return Results.NotFound();
    }

    items.Update(item);
    return Results.Ok(item);
});

app.MapDelete("/items/{id}", ([FromServices] ItemRepository items, int id) =>
{
    if (items.GetById(id) == null)
    {
        return Results.NotFound();
    }

    items.Delete(id);
    return Results.NoContent();
});

app.UseSwaggerUI();
app.Run();
