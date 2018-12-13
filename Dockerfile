FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app
COPY ./Dodo.HttpClientExtensions .
COPY ./NuGet.config ./
RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet pack -c Release --no-restore --no-build
RUN mv bin/Release/*.nupkg ./

ENTRYPOINT ["dotnet", "nuget", "push", "*.nupkg", "--source", "https://www.myget.org/F/dodopizza/"]