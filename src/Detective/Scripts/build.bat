echo "Start build"
@REM mkdir build
@REM dotnet build "Domain/Domain.csproj" -c Release -o build
@REM dotnet build "Logic/Logic.csproj" -c Release -o build
@REM dotnet build "DataAccess/DataAccess.csproj" -c Release -o build
@REM dotnet build "Server/Server.csproj" -c Release -o build

@REM copy Server\appsettings.json build\

dotnet publish "Server/Server.csproj" -c Release -o build