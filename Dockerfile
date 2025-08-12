FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./ ./

# Restore NuGet packages
RUN dotnet restore DailyWirePodcastProxy.sln
# Build the solution
RUN dotnet build DailyWirePodcastProxy.sln -c Release --no-restore
# Publish the Web API startup project
RUN dotnet publish \
           src/PodcastProxy.Web/PodcastProxy.Web.csproj \
        -c Release \
        --output publish \
        --force \
        --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# copy published bits
COPY --from=build /src/publish ./ 

EXPOSE 9473
ENV ASPNETCORE_URLS=http://0.0.0.0:9473

ENTRYPOINT ["dotnet", "PodcastProxy.Web.dll"]
