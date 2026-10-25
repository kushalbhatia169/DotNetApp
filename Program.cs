using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApp.Api.GraphQL.Mutations;
using MyApp.Api.GraphQL.Queries;
using MyApp.Api.GraphQL.Types;
using MyApp.Application.Abstractions;
using MyApp.Application.Services;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException("Configure Jwt:Secret with at least 32 bytes using a secret store or environment variable.");
}
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero,

            // Map standard JWT claims to .NET Identity claims
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICatalogService, CatalogService>();


// Register GraphQL Engine (Hot Chocolate)

// Register GraphQL Engine (Hot Chocolate)
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<ProductQuery>()
    .AddTypeExtension<CategoryQuery>()
    .AddMutationType(mutation => mutation.Name(OperationTypeNames.Mutation))
    .AddTypeExtension<ProductMutation>()
    .AddTypeExtension<CategoryMutation>()
    .AddType<ProductObjectType>()
    .AddType<CategoryObjectType>();

var app = builder.Build();

await DatabaseBootstrapper.InitializeAsync(app.Services, app.Environment.ContentRootPath);

app.UseAuthentication();
app.UseAuthorization();

// Map GraphQL Endpoint
app.MapGraphQL(); // Default route: /graphql



if (app.Environment.IsDevelopment())
{
    app.MapGet("/get-test-token", (IConfiguration config) =>
{
    var secretKey = config["Jwt:Secret"]!;
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim("role", "Admin") // Handles both standard and custom JWT role claims
    };

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(24),
        signingCredentials: creds
    );

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        });
    }

    app.Run();