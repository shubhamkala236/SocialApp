# AuthService/AuthService.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared contracts first
COPY ["Shared/Shared.Contracts/Shared.Contracts.csproj", "Shared/Shared.Contracts/"]

# Copy service projects
COPY ["AuthService/AuthService.API/AuthService.API.csproj",                         "AuthService/AuthService.API/"]
COPY ["AuthService/AuthService.Application/AuthService.Application.csproj",         "AuthService/AuthService.Application/"]
COPY ["AuthService/AuthService.Domain/AuthService.Domain.csproj",                   "AuthService/AuthService.Domain/"]
COPY ["AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj",   "AuthService/AuthService.Infrastructure/"]

RUN dotnet restore "AuthService/AuthService.API/AuthService.API.csproj"

COPY . .

WORKDIR "/src/AuthService/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]