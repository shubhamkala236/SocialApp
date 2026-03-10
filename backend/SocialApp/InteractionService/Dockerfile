# InteractionService/InteractionService.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Contracts/Shared.Contracts.csproj", "Shared/Shared.Contracts/"]
COPY ["InteractionService/InteractionService.API/InteractionService.API.csproj",                       "InteractionService/InteractionService.API/"]
COPY ["InteractionService/InteractionService.Application/InteractionService.Application.csproj",       "InteractionService/InteractionService.Application/"]
COPY ["InteractionService/InteractionService.Domain/InteractionService.Domain.csproj",                 "InteractionService/InteractionService.Domain/"]
COPY ["InteractionService/InteractionService.Infrastructure/InteractionService.Infrastructure.csproj", "InteractionService/InteractionService.Infrastructure/"]

RUN dotnet restore "InteractionService/InteractionService.API/InteractionService.API.csproj"

COPY . .

WORKDIR "/src/InteractionService/InteractionService.API"
RUN dotnet build "InteractionService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InteractionService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InteractionService.API.dll"]