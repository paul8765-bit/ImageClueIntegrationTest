#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ImageClueIntegrationTest.csproj", ""]
RUN dotnet restore ImageClueIntegrationTest.csproj
COPY . .
WORKDIR "/src/."
RUN dotnet build ImageClueIntegrationTest.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ImageClueIntegrationTest.csproj -c Release -o /app/publish

# Download the Chrome Driver for Selenium
WORKDIR "/usr/bin/"
RUN apt-get update
RUN apt-get install wget
RUN wget https://chromedriver.storage.googleapis.com/80.0.3987.106/chromedriver_linux64.zip
RUN apt-get install unzip
RUN unzip chromedriver_linux64.zip
RUN apt-get install -yq chromium

WORKDIR "/src/."
# Run unit tests
RUN dotnet test --verbosity normal

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageClueIntegrationTest.dll"]
#CMD tail -f /dev/null