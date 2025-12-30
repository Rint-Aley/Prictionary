using Microsoft.EntityFrameworkCore;
using Prictionary.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<DictionaryContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(ConnectionStringBuilder.BuildPostgres());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var apiGroup = app.MapGroup("/api/v1");
var wordsGroup = apiGroup.MapGroup("/words").WithTags("Words");
var groupsGroup = apiGroup.MapGroup("/groups").WithTags("Groups");

groupsGroup.MapGet("/", () =>
{
    return "Returns existinig groups (id + name)";
});
groupsGroup.MapPost("/", () =>
{
    return "Creates a new group";
});
groupsGroup.MapGet("/{id}", (long id) =>
{
    return $"Returns ids of words in group: {id}";
});
groupsGroup.MapPut("/{id}", (long id) =>
{
    return $"Changes group {id}";
});
groupsGroup.MapDelete("{id}", (long id) =>
{
    return $"Deletes group {id}";
});

groupsGroup.MapGet("/{id}/words", (long id) =>
{
    return $"Returns ids of group {id}";
});
groupsGroup.MapPost("/{id}/words", (long id) =>
{
    return $"Adds word to the group {id}";
});
groupsGroup.MapDelete("/{group_id}/words/{word_id}", (long group_id, long word_id) =>
{
    return $"Deletes word {word_id} in group {group_id}";
});

wordsGroup.MapPost("/", () =>
{
    return "Creates a new word";
});
wordsGroup.MapGet("/{id}", (long id) =>
{
    return $"Returns word: {id}";
});
wordsGroup.MapPut("/{id}", (long id) =>
{
    return $"Changes word {id}";
});
wordsGroup.MapDelete("/{id}", (long id) =>
{
    return $"Deletes word {id}";
});

app.Run();
