FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore ./src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj

RUN dotnet publish ./src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
