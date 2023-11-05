FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ServerStatisticsCollectionService/ServerStatisticsCollectionService.csproj", "ServerStatisticsCollectionService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["MessageBroker/MessageBroker.csproj", "MessageBroker/"]
RUN dotnet restore "ServerStatisticsCollectionService/ServerStatisticsCollectionService.csproj"
COPY . .
WORKDIR "/src/ServerStatisticsCollectionService"
RUN dotnet build "ServerStatisticsCollectionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerStatisticsCollectionService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerStatisticsCollectionService.dll"]
