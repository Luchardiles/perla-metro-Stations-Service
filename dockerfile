FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo del proyecto
COPY ["*.csproj", "./"]
RUN dotnet restore

# Copiar código fuente
COPY . .

# Build y publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Variables por defecto
ENV ASPNETCORE_URLS=http://+:8080
ENV MYSQL_CONNECTION_STRING="Server=trolley.proxy.rlwy.net;Port=58674;Database=railway;Uid=root;Pwd=kjhXaiJxIEvIdXUXuUTdcMbiLxQTISOu;SslMode=Preferred;"


# Punto de entrada
ENTRYPOINT ["dotnet", "perla-metro-Stations-Service.dll"]