using System.Reflection;
using System.Text.Json.Serialization;
using AcquiringBankSimulator;
using Infrastructure.AcquiringBank;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;
using PaymentGateway.API.Application.Commands;
using PaymentGateway.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IAcquiringBankService, SimulatedAcquiringBankService>();
builder.Services.AddScoped<IMediator, Mediator>();

builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("PaymentGatewayDatabase");
    return new MongoClient(connectionString);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            if (contextFeature.Error is PaymentProcessedException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            await context.Response.WriteAsync(new 
            {
                context.Response.StatusCode,
                Message = contextFeature.Error.Message
            }.ToString()!);
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
