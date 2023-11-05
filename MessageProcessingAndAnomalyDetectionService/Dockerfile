FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MessageProcessingAndAnomalyDetectionService/MessageProcessingAndAnomalyDetectionService.csproj", "MessageProcessingAndAnomalyDetectionService/"]
COPY ["MessageBroker/MessageBroker.csproj", "MessageBroker/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "MessageProcessingAndAnomalyDetectionService/MessageProcessingAndAnomalyDetectionService.csproj"
COPY . .
WORKDIR "/src/MessageProcessingAndAnomalyDetectionService"
RUN dotnet build "MessageProcessingAndAnomalyDetectionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageProcessingAndAnomalyDetectionService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageProcessingAndAnomalyDetectionService.dll"]
