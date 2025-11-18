FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Устанавливаем curl для healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Server.csproj", "./"]
RUN dotnet restore "Server.csproj"
COPY . .
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Создаем health check endpoint
RUN echo 'using Microsoft.AspNetCore.Builder; using Microsoft.AspNetCore.Http; var builder = WebApplication.CreateBuilder(args); builder.Services.AddControllers(); var app = builder.Build(); app.MapGet("/health", () => Results.Ok("Healthy")); app.MapControllers(); app.Run("http://0.0.0.0:5000");' > Program.cs

ENTRYPOINT ["dotnet", "Server.dll"]