FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

COPY ./NuGet.config /root/.nuget/NuGet/NuGet.Config

COPY ./Dodo.HttpClientExtensions ./Dodo.HttpClientExtensions
COPY ./Dodo.HttpClientExtensions.Tests ./Dodo.HttpClientExtensions.Tests

ARG versionSuffix

RUN dotnet restore ./Dodo.HttpClientExtensions/Dodo.HttpClientExtensions.csproj
RUN dotnet build --version-suffix "$versionSuffix" --no-restore --configuration Release ./Dodo.HttpClientExtensions/Dodo.HttpClientExtensions.csproj
RUN dotnet test --configuration Release ./Dodo.HttpClientExtensions.Tests/Dodo.HttpClientExtensions.Tests.csproj
RUN dotnet pack --version-suffix "$versionSuffix" --no-restore --no-build --configuration Release ./Dodo.HttpClientExtensions/Dodo.HttpClientExtensions.csproj

RUN mv ./Dodo.HttpClientExtensions/bin/Release/*.nupkg ./

ENTRYPOINT ["dotnet", "nuget", "push", "*.nupkg", "--source", "https://www.myget.org/F/dodopizza/"]
