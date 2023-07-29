# Set the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 80

# Copy the .csproj and restore dependencies
COPY PaymentGateway.API/PaymentGateway.API.csproj PaymentGateway.API/
RUN dotnet restore PaymentGateway.API/PaymentGateway.API.csproj

COPY PaymentGateway.API.Tests/PaymentGateway.API.Tests.csproj PaymentGateway.API.Tests/
RUN dotnet restore PaymentGateway.API.Tests/PaymentGateway.API.Tests.csproj

COPY PaymentGateway.Domain/PaymentGateway.Domain.csproj PaymentGateway.Domain/
RUN dotnet restore PaymentGateway.Domain/PaymentGateway.Domain.csproj

COPY PaymentGateway.Domain.Tests/PaymentGateway.Domain.Tests.csproj PaymentGateway.Domain.Tests/
RUN dotnet restore PaymentGateway.Domain.Tests/PaymentGateway.Domain.Tests.csproj

COPY PaymentGateway.Infrastructure/PaymentGateway.Infrastructure.csproj PaymentGateway.Infrastructure/
RUN dotnet restore PaymentGateway.Infrastructure/PaymentGateway.Infrastructure.csproj

COPY PaymentGateway.Infrastructure.Tests/PaymentGateway.Infrastructure.Tests.csproj PaymentGateway.Infrastructure.Tests/
RUN dotnet restore PaymentGateway.Infrastructure.Tests/PaymentGateway.Infrastructure.Tests.csproj

COPY AcquiringBankSimulator/AcquiringBankSimulator.csproj AcquiringBankSimulator/
RUN dotnet restore AcquiringBankSimulator/AcquiringBankSimulator.csproj

# Copy the solution file
COPY Checkout.PaymentGateway.sln ./
RUN dotnet restore Checkout.PaymentGateway.sln

# Copy the rest of the files and build the project
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PaymentGateway.API.dll"]