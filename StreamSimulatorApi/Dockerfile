FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["StreamSimulatorApi/StreamSimulatorApi.csproj", "StreamSimulatorApi/"]
RUN dotnet restore "StreamSimulatorApi/StreamSimulatorApi.csproj"
COPY . .
WORKDIR "/src/StreamSimulatorApi"
RUN dotnet build "StreamSimulatorApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StreamSimulatorApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StreamSimulatorApi.dll"]