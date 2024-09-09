FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 45066

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["paas/btm.paas.csproj", "paas/"]
COPY ["shared/btm.shared.csproj", "shared/"]
RUN dotnet restore "./paas/btm.paas.csproj"
COPY . .
WORKDIR "/src/paas"
RUN dotnet build "./btm.paas.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./btm.paas.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "btm.paas.dll"]
