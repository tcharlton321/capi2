using CaesarsAPI;
using CaesarsAPI.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(x => x.With(y => y.HttpContext.Request.Query["nocache"] == "true").NoCache());
    options.AddPolicy(nameof(AuthCachePolicy), AuthCachePolicy.Instance);
});

builder.Services.AddAuthentication().AddBearerToken();
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapControllers();

app.MapGet("/login", (string username, string password) =>
{
    DB_manager db = new DB_manager();
    bool goodLogin = db.VerifyLogin(username, password);
    if(goodLogin)
    {
        var claimsPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, username) },
                BearerTokenDefaults.AuthenticationScheme
            )
        );
        return Results.SignIn(claimsPrincipal);
    }

    return Results.Unauthorized();
    
});

app.MapGet("/register", (string username, string password) =>
{
    DB_manager db = new DB_manager();
    bool success = db.InsertNewUser(username, password);

    if (success)
    {
        return Results.Ok($"User created, please login.");
    } else
    {
        return Results.Problem($"a problem has occurred");
    }
});

app.MapGet("/user", (ClaimsPrincipal user) =>
{
    return Results.Ok($"Welcome {user.Identity.Name}!");
}).RequireAuthorization();



app.Run();
