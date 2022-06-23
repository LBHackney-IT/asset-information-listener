FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

RUN apt-get update && apt-get install -y openjdk-11-jdk
RUN dotnet tool install --global dotnet-sonarscanner --version 5.6.0
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_asset-information-listener" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"


# Copy csproj and restore as distinct layers
COPY ./AssetInformationListener.sln ./
COPY ./AssetInformationListener/AssetInformationListener.csproj ./AssetInformationListener/
COPY ./AssetInformationListener.Tests/AssetInformationListener.Tests.csproj ./AssetInformationListener.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./AssetInformationListener/AssetInformationListener.csproj
RUN dotnet restore ./AssetInformationListener.Tests/AssetInformationListener.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out AssetInformationListener/AssetInformationListener.csproj
RUN dotnet build -c debug -o out AssetInformationListener.Tests/AssetInformationListener.Tests.csproj

CMD dotnet test
RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
