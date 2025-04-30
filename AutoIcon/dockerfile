# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY AutoIcon/*.csproj ./AutoIcon/
COPY AutoIcon.Contracts/*.csproj ./AutoIcon.Contracts/

RUN dotnet restore

COPY . .
RUN dotnet publish AutoIcon -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AutoIcon.dll"]
