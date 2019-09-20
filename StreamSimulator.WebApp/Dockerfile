FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["StreamSimulator.WebApp/StreamSimulator.WebApp.csproj", "StreamSimulator.WebApp/"]
RUN dotnet restore "StreamSimulator.WebApp/StreamSimulator.WebApp.csproj"
COPY . .
WORKDIR "/src/StreamSimulator.WebApp"
RUN dotnet build "StreamSimulator.WebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StreamSimulator.WebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StreamSimulator.WebApp.dll"]