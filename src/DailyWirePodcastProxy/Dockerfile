FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DailyWirePodcastProxy/DailyWirePodcastProxy.csproj", "DailyWirePodcastProxy/"]
RUN dotnet restore "src/DailyWirePodcastProxy/DailyWirePodcastProxy.csproj"
COPY . .
WORKDIR "/src/DailyWirePodcastProxy"
RUN dotnet build "DailyWirePodcastProxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DailyWirePodcastProxy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DailyWirePodcastProxy.dll"]
