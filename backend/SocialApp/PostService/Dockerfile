# PostService/PostService.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Contracts/Shared.Contracts.csproj", "Shared/Shared.Contracts/"]
COPY ["PostService/PostService.API/PostService.API.csproj",                       "PostService/PostService.API/"]
COPY ["PostService/PostService.Application/PostService.Application.csproj",       "PostService/PostService.Application/"]
COPY ["PostService/PostService.Domain/PostService.Domain.csproj",                 "PostService/PostService.Domain/"]
COPY ["PostService/PostService.Infrastructure/PostService.Infrastructure.csproj", "PostService/PostService.Infrastructure/"]

RUN dotnet restore "PostService/PostService.API/PostService.API.csproj"

COPY . .

WORKDIR "/src/PostService/PostService.API"
RUN dotnet build "PostService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PostService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PostService.API.dll"]