FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/EfCoreApp/EfCoreApp.csproj", "src/EfCoreApp/"]
RUN dotnet restore "src/EfCoreApp/EfCoreApp.csproj"
COPY . .
WORKDIR "/src/src/EfCoreApp"
RUN dotnet build "EfCoreApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EfCoreApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EfCoreApp.dll"]