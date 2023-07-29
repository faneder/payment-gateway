# Context

Payment gateway API refers to **the technical interface that enables software developers to integrate payment gateway services into their applications or websites**. Payment gateway integration, on the other hand, is the process of connecting a website or application to a payment gateway using that

## Requirement Gathering

1. A merchant should be able to process a payment through the payment gateway and receive either a successful or unsuccessful response.
2. A merchant should be able to retrieve the details of a previously made payment. The next section will discuss each of these in more detail.

## Assumption

- Only support Credit Card Payment
- [PAN](https://en.wikipedia.org/wiki/Payment_card_number) lengths is 16 ***digits and support cvv with 3 digits. e.g: Master card***
- Only support currency for USD
- Simulate bank response with amount divided by 2 is even then returns success otherwise failure

## Tech

- C#
- .Net 7
- Testing: Xunit, Fluentassertions
- MongoDB

- Docker

## Estimation

TPS: 100

## Clean architecture

- Domain Layer
    - **Payment**: This entity represents a payment made by a shopper. It holds all the information about the payment, such as the merchant, the shopper, the card information, the amount, the currency, and the status.
    - **CardInformation**: This value object represents the card information.
    - **Money**: This value object represents an amount of money. It includes properties like amount and currency.
- API Layer
- Infrastructure Layer
- **PaymentRepository**: This class implements `IPaymentRepository`. It uses a *IMongoClient* client to interact with the database.
- IAcquiringBankService: This interface defines the contract for a bank service.
- SimulatedAcquiringBankService Layer
    - SimulatedAcquiringBankService: This class implements IAcquiringBankService.

### Tests (TDD approach)

- Unit Tests
- Integration Tests

### Bank Simulator

- Using a class with interface to simulate the behavior.

## APIs

| Endpoint | Method | Description | Request Body | Response Body | Status Code                                                                                       |
| --- | --- | --- | --- | --- |---------------------------------------------------------------------------------------------------|
| /payments | POST | Process a payment | PaymentRequest | PaymentResponse | - 201 Created - 400 Bad Request - 500 Internal server error (e.g Cannot connect to acquiring bank |
  | /payments/{paymentId} | GET | Retrieve payment details by ID |  | PaymentDetailsResponse | - 200 OK - 404 Not Found |                                                                       
**PaymentRequest**

| Field | Type | Description | Required |
| --- | --- | --- | --- |
| CardHolderName | string | Card holder of the card | Yes |
| cardNumber | string | Card number of the card | Yes |
| expiryMonth | integer | Expiry month of the card | Yes |
| expiryYear | integer | Expiry year of the card | Yes |
| amount | integer | Payment amount | Yes |
| currency | string | Currency code (e.g., USD) | Yes |
| cvv | string | CVV code of the card | Yes |
| MerchantId | UUID |  |  |
| Reference | string |  |  |
| PaymentType | string | Regular as default |  |

**PaymentResponse**

| Field | Type | Description |
| --- | --- | --- |
| paymentId | UUID | ID of the processed payment |
| status | string | Payment status (e.g., Authorized, Declined) |
| ResponseCode |  |  |
| ResponseMessage |  |  |

**PaymentDetailsResponse**

| Field | Type | Description |  |
| --- | --- | --- | --- |
| CardHolderName | string | Card holder |  |
| maskedCardNumber | string | Masked card number for security |  |
| CVV | string | CVV code of the card |  |
| ExpiryDate | string | Expiry date of the card. e.g: 12/2025 |  |
| status | string | Payment status (e.g., Authorized, Declined) |  |

## How to use the APIs

#### 1. Spin up Docker to build the environment for testing. Default port number is “8000”.

```jsx
docker compose build
docker compose up
```

#### 2. Request a payment

API Documentation
https://documenter.getpostman.com/view/5565880/2s9Xxtwue5

Two ways you can test the APIs:

1. If you have Postman, simply import the API collection from PaymentGateway.postman_collection.json
2. Run request using CURL via terminal

💡 You need to request a payment before retrieve a payment therefore to generate a payment id from from the response body.


## API with different payment scenarios

****POST - Request a payment with Authorised status****

http://localhost:8000/payments

Request Body

```jsx
curl -H 'Content-Type: application/json' \
      -d '{
			  "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
			  "cardSource": {
			    "name": "Happy Shopper",
			    "number": "1234123412347777",
			    "cvv": "123",
			    "expiryMonth": 10,
			    "expiryYear": 2220
			  },
			  "currency": "USD",
			  "amount": 100,
			  "paymentType": "Regular",
			  "reference": "string"
			}' \
    http://localhost:8000/payments
```

****POST - Request a payment with invalid request****

http://localhost:8000/payments

Request Body: json

```
curl -H 'Content-Type: application/json' \
      -d '{
			  "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
			  "cardSource": {
			    "name": "Happy Shopper",
			    "number": "1234123412347777",
			    "cvv": "invalid CVV",
			    "expiryMonth": 10,
			    "expiryYear": 2220
			  },
			  "currency": "USD",
			  "amount": 100,
			  "paymentType": "Regular",
			  "reference": "string"
			}' \
    http://localhost:8000/payments
```

****POST - Request a payment with Declined status****

http://localhost:8000/payments

Request Body

```

curl -H 'Content-Type: application/json' \
      -d '{
			  "merchantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
			  "cardSource": {
			    "name": "Happy Shopper",
			    "number": "1234123412347777",
			    "cvv": "123",
			    "expiryMonth": 10,
			    "expiryYear": 2220
			  },
			  "currency": "USD",
			  "amount": 77779999,
			  "paymentType": "Regular",
			  "reference": "string"
			}' \
    http://localhost:8000/payments
```

****GET - Retrieve a payment successful - http://localhost:8000/payments/{paymentId}****

```
curl http://localhost:8000/payments/PaymentIdFromPreviousMadePayment
```

****GET - Retrieve a not exist payment****

```jsx
curl http://localhost:8000/payments/PaymentIdFromPreviousMadePayment)b0787f37-60f5-4063-a242-95b56770e4d3
```

## Infra

Cloud provider uses Azure as an example for creating a cloud environment.

Create CI/CD pipeline to **automate software delivery process**. The pipeline builds code, runs tests, security check (CI), and safely deploys a new version of the application (CD).

1. **Azure Kubernetes Service (AKS):** Kubernetes is a powerful platform for container orchestration. It provides features for automating deployment, scaling, and managing containerized applications. Using AKS, we can take full advantage of these features, while also leveraging Azure's added benefits such as Azure Dev Spaces for easier development and testing in the cloud, and Azure Policy for enforcing governance and compliance.
2. **Azure Virtual Machines:** Azure VMs provide a flexible, scalable hosting solution for our application. They're ideal for applications that require a high level of control over the server environment.
3. **Azure DevOps:** This is a complete DevOps toolchain from Microsoft. It includes Azure Pipelines for CI/CD, which can automate building, testing, and deploying your application. Azure DevOps also includes features for source control, work tracking, and artifact storage, making it a one-stop-shop for DevOps.
4. **Azure Container Registry:** Storing your Docker images in Azure Container Registry makes it easy to deploy those images to Azure Kubernetes Service. It's also secure, as it enables you to keep your images private and control who can pull them.
5. **Snyk:** This tool helps to identify and fix vulnerabilities in your dependencies. Incorporating Snyk into your CI/CD pipeline helps to ensure that your application doesn't ship with known security vulnerabilities, which is critical in a payment system where security is a top priority.
6. **SonarQube:** SonarQube is a tool for automatic code review, it can identify bugs and vulnerabilities, and it also has features for tracking code quality over time. Ensuring high code quality is key to maintaining a reliable, maintainable application.

## Improvement

### Securability

| Problem                                                                                                                                                                                  | Solution                                                                                      |
|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| eavesdropping                                                                                                                                                                            | HTTPS                                                                                         |
| Data tampering                                                                                                                                                                           | Enforce encryption and integrity monitoring, can consider using HSM for storing card info     |
| Distributed dential-of-service attack DDOS                                                                                                                                               | Rate limiting and firewall                                                                    |
| Card Theft                                                                                                                                                                               | Tokenisation. Instead of using read card numbers, token are stored and used for payment       |
| PCI compliance                                                                                                                                                                           | PCI DSS is an information security standard for organisation that handle branded credit cards |
| Fraud                                                                                                                                                                                    | Address verification, card verification value (CVV), user behavior                            |
| Authentication and authorization are vital cross-cutting concerns for ensuring secure access and protecting sensitive data within the payment gateway. | Utilise established authentication protocols (e.g., OAuth, JWT) and authorization frameworks (e.g., IdentityServer, OAuth2) to handle these concerns. |

### Reliability and fault tolerance:

Failed payments need to be carefully handled by
- Retry
- Circuit breaker
- Dead letter queue

### Enhance Testing:
- Acceptance tests
- Performance tests
- Smoke tests
- Health check tests

### Create Observability tools

- Monitoring
- Alerting