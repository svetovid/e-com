FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 45077

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web/btm.web.app/btm.web.app.csproj", "web/btm.web.app/"]
COPY ["web/btm.web.app.Client/btm.web.app.Client.csproj", "web/btm.web.app.Client/"]
COPY ["shared/btm.shared.csproj", "shared/"]
RUN dotnet restore "./web/btm.web.app/btm.web.app.csproj"
COPY . .
WORKDIR "/src/web/btm.web.app"
RUN dotnet build "./btm.web.app.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./btm.web.app.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "btm.web.app.dll"]