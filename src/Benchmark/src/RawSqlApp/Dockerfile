FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/RawSqlApp/RawSqlApp.csproj", "src/RawSqlApp/"]
RUN dotnet restore "src/RawSqlApp/RawSqlApp.csproj"
COPY . .
WORKDIR "/src/src/RawSqlApp"
RUN dotnet build "RawSqlApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RawSqlApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RawSqlApp.dll"]