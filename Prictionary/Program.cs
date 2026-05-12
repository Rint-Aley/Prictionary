using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prictionary.Configuration;
using Prictionary.Database;
using Prictionary.Extensions;
using Prictionary.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddMvc()
  .AddApiExplorer(options =>
  {
      options.GroupNameFormat = "'v'V";
      options.SubstituteApiVersionInUrl = true;
  });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v0");

builder.Services
    .AddPrictionaryConfiguration(builder.Configuration)
    .AddPrictionaryServices();

var connectionString = ConnectionStringBuilder.BuildPostgres();
var secrets = new Secrets();
builder.Configuration.GetSection(Secrets.Section).Bind(secrets);

builder.Services.AddDbContext<PrictionaryContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(connectionString);
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<PrictionaryContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secrets.JwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v0.json", "Prictionary API v0");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
