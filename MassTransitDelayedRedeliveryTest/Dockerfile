# Use .NET 8 SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the entire project first
COPY . .  

# Restore dependencies
RUN dotnet restore

# Build and publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MassTransitDelayedRedeliveryTest.dll"]

