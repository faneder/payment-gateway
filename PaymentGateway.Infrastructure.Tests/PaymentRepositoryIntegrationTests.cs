using FluentAssertions;
using Infrastructure.Persistence;
using Mongo2Go;
using MongoDB.Driver;
using PaymentGateway.Tests;

namespace PaymentGateway.Infrastructure.IntegrationTests;

public class PaymentRepositoryIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly PaymentRepository _repository;

    public PaymentRepositoryIntegrationTests()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _repository = new PaymentRepository(client);
    }

    [Fact]
    public async Task GetPaymentAsync_ShouldReturnCorrectPayment()
    {
        var expectedPayment = new PaymentBuilder().Build();
        await _repository.SavePaymentAsync(expectedPayment);

        var payment = await _repository.GetPaymentAsync(expectedPayment.Id);

        payment.Should().BeEquivalentTo(expectedPayment, options => options.Excluding(p => p.Created));
    }

    [Fact]
    public async Task GetPaymentAsync_ShouldReturnNull_NoPaymentData()
    {
        var expectedPayment = new PaymentBuilder().Build();

        var payment = await _repository.GetPaymentAsync(expectedPayment.Id);

        payment.Should().BeNull();
    }

    public void Dispose()
    {
        _runner.Dispose();
    }
}
