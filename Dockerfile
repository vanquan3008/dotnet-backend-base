FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/MyProject.Api/MyProject.Api.csproj src/MyProject.Api/
COPY src/MyProject.Application/MyProject.Application.csproj src/MyProject.Application/
COPY src/MyProject.Domain/MyProject.Domain.csproj src/MyProject.Domain/
COPY src/MyProject.Infrastructure/MyProject.Infrastructure.csproj src/MyProject.Infrastructure/
RUN dotnet restore src/MyProject.Api/MyProject.Api.csproj

COPY src/ src/
RUN dotnet publish src/MyProject.Api/MyProject.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyProject.Api.dll"]
