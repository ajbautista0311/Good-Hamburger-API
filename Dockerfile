# ===================================
# Build stage
# ===================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/GoodHamburger.API/GoodHamburger.API.csproj", "API/"]
COPY ["src/GoodHamburger.Application/GoodHamburger.Application.csproj", "Application/"]
COPY ["src/GoodHamburger.Domain/GoodHamburger.Domain.csproj", "Domain/"]
COPY ["src/GoodHamburger.Infrastructure/GoodHamburger.Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "API/GoodHamburger.API.csproj"

# Copy all source code
COPY src/ .

# Build the application
WORKDIR "/src/GoodHamburger.API"
RUN dotnet build "GoodHamburger.API.csproj" -c Release -o /app/build

# ===================================
# Publish stage
# ===================================
FROM build AS publish
RUN dotnet publish "GoodHamburger.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===================================
# Runtime stage
# ===================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5000

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the application
ENTRYPOINT ["dotnet", "GoodHamburger.API.dll"]