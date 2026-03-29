using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Prictionary.Database;
using Prictionary.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PrictionaryContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(ConnectionStringBuilder.BuildPostgres());
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<PrictionaryContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
}

app.Run();
