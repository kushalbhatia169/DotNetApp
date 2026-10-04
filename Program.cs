using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApp.Api;
using MyApp.Api.GraphQL;
using MyApp.Api.GraphQL.Queries;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
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

// Register Services with concrete file paths
builder.Services.AddSingleton<IProductRepository>(sp => new ProductRepository(System.IO.Path.Combine(builder.Environment.ContentRootPath, "Data", "products.json")));
builder.Services.AddScoped<ICategoryRepository>(sp => new CategoryRepository(System.IO.Path.Combine(builder.Environment.ContentRootPath, "Data", "categories.json")));

// Register GraphQL Engine (Hot Chocolate)
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<ProductQuery>()
    .AddType<ProductObjectType>()
    .AddType<CategoryObjectType>();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

// Map GraphQL Endpoint
app.MapGraphQL(); // Default route: /graphql



// Temporary Endpoint to Get Valid JWT Token for Testing
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

app.Run();