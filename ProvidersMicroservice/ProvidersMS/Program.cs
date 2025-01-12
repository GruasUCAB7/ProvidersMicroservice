using DotNetEnv;
using FluentValidation;
using Microsoft.OpenApi.Models;
using ProvidersMS.Core.Application.GoogleApiService;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.Core.Application.Storage.Images;
using ProvidersMS.Core.Infrastructure.Data;
using ProvidersMS.Core.Infrastructure.GoogleMaps;
using ProvidersMS.Core.Infrastructure.Logger;
using ProvidersMS.Core.Infrastructure.Storage;
using ProvidersMS.Core.Infrastructure.UUID;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Infrastructure.Repositories;
using ProvidersMS.src.Cranes.Infrastructure.Validators;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation.Types;
using ProvidersMS.src.Drivers.Application.Repositories;
using ProvidersMS.src.Drivers.Infrastructure.Dtos;
using ProvidersMS.src.Drivers.Infrastructure.Repositories;
using ProvidersMS.src.Drivers.Infrastructure.Validators;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;
using ProvidersMS.src.Providers.Application.Repositories;
using ProvidersMS.src.Providers.Infrastructure.Repositories;
using ProvidersMS.src.Providers.Infrastructure.Validators;
using RestSharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IImageStorage, CloudinaryImageStorage>();
builder.Services.AddTransient<IValidator<CreateCraneCommand>, CreateCraneValidator>();
builder.Services.AddTransient<IValidator<UpdateCraneCommand>, UpdateCraneValidator>();
builder.Services.AddTransient<IValidator<CreateDriverWithImagesCommand>, CreateDriverValidator>();
builder.Services.AddTransient<IValidator<UpdateDriverCommand>, UpdateDriverValidator>();
builder.Services.AddTransient<IValidator<CreateProviderCommand>, CreateProviderValidator>();
builder.Services.AddTransient<IValidator<UpdateProviderCommand>, UpdateProviderValidator>();
builder.Services.AddTransient<IValidator<UpdateDriverLocationCommand>, UpdateDriverLocationValidator>();
builder.Services.AddScoped<ICraneRepository, MongoCraneRepository>();
builder.Services.AddScoped<IDriverRepository, MongoDriverRepository>();
builder.Services.AddScoped<IProviderRepository, MongoProviderRepository>();
builder.Services.AddScoped<IdGenerator<string>, GuidGenerator>();
builder.Services.AddScoped<ILoggerContract, Logger>();
builder.Services.AddScoped<IGoogleApiService, GoogleApiService>();
builder.Services.AddScoped<IRestClient, RestClient>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API ProviderssMicroservice",
        Description = "Endpoints de ProvidersMicroservice",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!))
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApiGateway",
        builder => builder.WithOrigins("https://localhost:4050")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var cloudinary = CloudinaryConfiguration.ConfigureCloudinary();
builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ProvidersMicroservice");
        c.RoutePrefix = string.Empty;
    });

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowApiGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();