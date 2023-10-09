using System.Text;
using System.Text.Json.Serialization;
using BookStore.Extensions;
using BookStore.Services.IMPL;
using BookStore.Services.Interfaces;
using BookStoreWebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    //Штука для возможности получения расширенных json'ов
    .AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BookStoresDbContext>(ServiceLifetime.Transient);

//Пример того, как конфиги мапятся к классу
var jwtSection = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
builder.Services.AddSingleton(jwtSection);
builder.Services.AddTransient<IUserTokenHelper, UserTokenHelper>();

// 👇 Configuring the Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        //convert the string signing key to byte array
        byte[] signingKeyBytes = Encoding.UTF8
            .GetBytes(jwtSection.SecretKey);

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.Issuer,
            ValidAudience = jwtSection.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
        };
    });

// 👇 Configuring the Authorization Service
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Warning. Possible problems with IServiceProvider
UserExtensions._serviceProvider = app.Services;

app.Run();
