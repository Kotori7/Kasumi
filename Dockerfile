# based on https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY Kasumi.sln .
COPY nuget.config .
COPY Kasumi/Kasumi.csproj ./Kasumi/
RUN dotnet restore

COPY Kasumi/. ./Kasumi/
WORKDIR /src/Kasumi
RUN dotnet publish -c Release -o /output --no-restore --no-cache

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /output ./
ENTRYPOINT ["dotnet", "Kasumi.dll"]