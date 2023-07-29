using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using PaymentGateway.API.Application.Commands;
using PaymentGateway.API.Application.Response;
using PaymentGateway.Domain;
using PaymentGateway.Tests;
using Xunit;

namespace PaymentGateway.API.Tests.Controller;

public class PaymentControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;


    public PaymentControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProcessPayment_ValidCommand_ReturnsOk()
    {
        // Arrange
        var command = new ProcessPaymentCommandBuilder().Build();
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/payments", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ProcessPayment_InvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var command = new ProcessPaymentCommandBuilder().WithCurrency("happycurrency").Build();
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/payments", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ProcessPayment_InvalidCommand_ReturnsNotFound()
    {
        // Arrange
        var command = new ProcessPaymentCommand();
        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/payments", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
    
    
    [Fact]
    public async Task GetPaymentDetails_Returns_NotFound_For_NonExist_Payment()
    {
        // Arrange
        var nonExistentPaymentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/payments/{nonExistentPaymentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task GetPaymentDetails_Returns_PaymentDetails_For_ExistPayment()
    {
        // Arrange
        var database = _factory.GetMongoClient().GetDatabase("payment-gateway");
        var collection = database.GetCollection<Payment>("payments");
        var payment = new PaymentBuilder().Build();
        await collection.InsertOneAsync(payment);
        
        // Act
        var response = await _client.GetAsync($"/payments/{payment.Id.ToString()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseString = await response.Content.ReadAsStringAsync();
        var paymentDetails = JsonConvert.DeserializeObject<PaymentDetailsResponse>(responseString);
        paymentDetails.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ProcessPayment_ReturnsBadRequest_WhenCommandIsNull()
    {
        var response = await _client.GetAsync($"/payments/null");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}