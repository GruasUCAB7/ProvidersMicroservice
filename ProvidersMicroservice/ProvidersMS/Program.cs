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
using ProvidersMS.Core.Infrastructure.Swagger;
using ProvidersMS.Core.Infrastructure.UUID;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Infrastructure.Repositories;
using ProvidersMS.src.Cranes.Infrastructure.Validators;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;
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

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<ImageStorage, CloudinaryImageStorage>();
builder.Services.AddTransient<IValidator<CreateCraneCommand>, CreateCraneValidator>();
builder.Services.AddTransient<IValidator<UpdateCraneCommand>, UpdateCraneValidator>();
builder.Services.AddTransient<IValidator<CreateDriverWithImagesCommand>, CreateDriverValidator>();
builder.Services.AddTransient<IValidator<UpdateDriverCommand>, UpdateDriverValidator>();
builder.Services.AddTransient<IValidator<CreateProviderCommand>, CreateProviderValidator>();
builder.Services.AddTransient<IValidator<UpdateProviderCommand>, UpdateProviderValidator>();
builder.Services.AddScoped<ICraneRepository, MongoCraneRepository>();
builder.Services.AddScoped<IDriverRepository, MongoDriverRepository>();
builder.Services.AddScoped<IProviderRepository, MongoProviderRepository>();
builder.Services.AddScoped<IdGenerator<string>, GuidGenerator>();
builder.Services.AddScoped<ILoggerContract, Logger>();
builder.Services.AddScoped<IGoogleApiService, GoogleApiService>();
builder.Services.AddScoped<IRestClient, RestClient>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProvidersMS API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
app.UseAuthorization();
app.MapControllers();

app.Run();