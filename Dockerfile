# ---------- Etape 1 : build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FirstProjectAPI/FirstProjectAPI.csproj FirstProjectAPI/
RUN dotnet restore FirstProjectAPI/FirstProjectAPI.csproj

COPY FirstProjectAPI/ FirstProjectAPI/

WORKDIR /src/FirstProjectAPI
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---------- Etape 2 : image finale ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FirstProjectAPI.dll"]