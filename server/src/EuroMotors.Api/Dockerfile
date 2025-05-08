# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# Create SSL directory
RUN mkdir -p /app/ssl

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/EuroMotors.Api/EuroMotors.Api.csproj", "src/EuroMotors.Api/"]
COPY ["src/EuroMotors.Application/EuroMotors.Application.csproj", "src/EuroMotors.Application/"]
COPY ["src/EuroMotors.Domain/EuroMotors.Domain.csproj", "src/EuroMotors.Domain/"]
COPY ["src/EuroMotors.Infrastructure/EuroMotors.Infrastructure.csproj", "src/EuroMotors.Infrastructure/"]
RUN dotnet restore "./src/EuroMotors.Api/EuroMotors.Api.csproj"
COPY . .
WORKDIR "/src/src/EuroMotors.Api"
RUN dotnet build "./EuroMotors.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EuroMotors.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy SSL certificates
COPY src/EuroMotors.Api/ssl/euromotors_tech.pfx /app/ssl/
COPY src/EuroMotors.Api/ssl/euromotors_tech.crt /app/ssl/
COPY src/EuroMotors.Api/ssl/euromotors_tech.ca-bundle /app/ssl/
ENTRYPOINT ["dotnet", "EuroMotors.Api.dll"]