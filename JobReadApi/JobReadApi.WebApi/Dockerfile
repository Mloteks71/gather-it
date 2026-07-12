# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends wget \
    && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JobReadApi.WebApi/JobReadApi.WebApi.csproj", "JobReadApi.WebApi/"]
COPY ["JobReadApi.Application/JobReadApi.Application.csproj", "JobReadApi.Application/"]
COPY ["JobReadApi.Infrastructure/JobReadApi.Infrastructure.csproj", "JobReadApi.Infrastructure/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "./JobReadApi.WebApi/JobReadApi.WebApi.csproj"
COPY . .
WORKDIR "/src/JobReadApi.WebApi"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet build "./JobReadApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "./JobReadApi.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobReadApi.WebApi.dll"]
