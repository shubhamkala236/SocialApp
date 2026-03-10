# AIService/AIService.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Contracts/Shared.Contracts.csproj", "Shared/Shared.Contracts/"]
COPY ["AIService/AIService.API/AIService.API.csproj",                       "AIService/AIService.API/"]
COPY ["AIService/AIService.Application/AIService.Application.csproj",       "AIService/AIService.Application/"]
COPY ["AIService/AIService.Domain/AIService.Domain.csproj",                 "AIService/AIService.Domain/"]
COPY ["AIService/AIService.Infrastructure/AIService.Infrastructure.csproj", "AIService/AIService.Infrastructure/"]

RUN dotnet restore "AIService/AIService.API/AIService.API.csproj"

COPY . .

WORKDIR "/src/AIService/AIService.API"
RUN dotnet build "AIService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIService.API.dll"]