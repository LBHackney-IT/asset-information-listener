FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./AssetInformationListener.sln ./
COPY ./AssetInformationListener/AssetInformationListener.csproj ./AssetInformationListener/
COPY ./AssetInformationListener.Tests/AssetInformationListener.Tests.csproj ./AssetInformationListener.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./AssetInformationListener/AssetInformationListener.csproj
RUN dotnet restore ./AssetInformationListener.Tests/AssetInformationListener.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out AssetInformationListener.Tests/AssetInformationListener.Tests.csproj

CMD dotnet test
