using System.Reflection;
using AcquiringBankSimulator;
using Infrastructure.AcquiringBank;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using PaymentGateway.Domain.Interfaces;

namespace PaymentGateway.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    public IMongoClient _mongoClient;
    public MongoDbRunner _mongoDbRunner;

    public IMongoClient GetMongoClient()
    {
        return _mongoClient;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddSingleton<IPaymentRepository, PaymentRepository>();
            services.AddSingleton<IAcquiringBankService, SimulatedAcquiringBankService>();
     
            // Add a new IMongoClient registration with a Mongo2Go instance.
            _mongoDbRunner = MongoDbRunner.Start();
            _mongoClient = new MongoClient(_mongoDbRunner.ConnectionString);
            services.AddSingleton(_mongoClient);
        });
    }
    
    public void Dispose()
    {
        _mongoDbRunner.Dispose();
    }
}