FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 5053
# should be a comma-delimited list
ENV CLUSTER_SEEDS "[]"
ENV CLUSTER_IP ""
ENV CLUSTER_PORT "5053"

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["lighthouse/lighthouse.csproj", "lighthouse/"]
RUN dotnet restore "./lighthouse/lighthouse.csproj"
COPY . .
WORKDIR "/src/lighthouse"
RUN dotnet build "./lighthouse.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./lighthouse.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "lighthouse.dll"]