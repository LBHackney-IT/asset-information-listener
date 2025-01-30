FROM mcr.microsoft.com/dotnet/sdk:6.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

ENV PATH="$PATH:/root/.dotnet/tools"

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
