# UserService/UserService.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Contracts/Shared.Contracts.csproj", "Shared/Shared.Contracts/"]
COPY ["UserService/UserService.API/UserService.API.csproj",                       "UserService/UserService.API/"]
COPY ["UserService/UserService.Application/UserService.Application.csproj",       "UserService/UserService.Application/"]
COPY ["UserService/UserService.Domain/UserService.Domain.csproj",                 "UserService/UserService.Domain/"]
COPY ["UserService/UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService/UserService.Infrastructure/"]

RUN dotnet restore "UserService/UserService.API/UserService.API.csproj"

COPY . .

WORKDIR "/src/UserService/UserService.API"
RUN dotnet build "UserService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]