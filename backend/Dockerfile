# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/HomeInventory.Domain/HomeInventory.Domain.csproj", "src/HomeInventory.Domain/"]
COPY ["src/HomeInventory.Application/HomeInventory.Application.csproj", "src/HomeInventory.Application/"]
COPY ["src/HomeInventory.Infrastructure/HomeInventory.Infrastructure.csproj", "src/HomeInventory.Infrastructure/"]
COPY ["src/HomeInventory.Queries/HomeInventory.Queries.csproj", "src/HomeInventory.Queries/"]
COPY ["src/HomeInventory.WebApi/HomeInventory.WebApi.csproj", "src/HomeInventory.WebApi/"]

# Restore dependencies
RUN dotnet restore "src/HomeInventory.WebApi/HomeInventory.WebApi.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/src/HomeInventory.WebApi"
RUN dotnet build "HomeInventory.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HomeInventory.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Generate EF Core migration bundle
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/efbundle

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install System.Drawing dependencies for QR code generation
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
COPY --from=publish /app/efbundle .
RUN chmod +x efbundle

ENTRYPOINT ["dotnet", "HomeInventory.WebApi.dll"]
