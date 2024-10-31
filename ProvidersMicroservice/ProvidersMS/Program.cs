using DotNetEnv;
using FluentValidation;
using Microsoft.OpenApi.Models;
using ProvidersMS.Core.Application.IdGenerator;
using ProvidersMS.Core.Application.Logger;
using ProvidersMS.Core.Infrastructure.Data;
using ProvidersMS.Core.Infrastructure.Logger;
using ProvidersMS.Core.Infrastructure.UUID;
using ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;
using ProvidersMS.src.Cranes.Application.Repositories;
using ProvidersMS.src.Cranes.Infrastructure.Repositories;
using ProvidersMS.src.Cranes.Infrastructure.Validators;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddTransient<IValidator<CreateCraneCommand>, CreateCraneCommandValidator>();
builder.Services.AddTransient<IValidator<UpdateCraneCommand>, UpdateCraneCommandValidator>();
builder.Services.AddScoped<ICraneRepository, MongoCraneRepository>();
builder.Services.AddScoped<IdGenerator<string>, GuidGenerator>();
builder.Services.AddScoped<ILoggerContract, Logger>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API ProvidersMicroservice",
        Version = "v1",
        Description = "Endpoints de ProvidersMicroservice",
    });
});

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
app.UseAuthorization();
app.MapControllers();

app.Run();
