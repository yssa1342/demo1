# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY WallpaperApi.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create upload directories
RUN mkdir -p wwwroot/uploads/wallpapers wwwroot/uploads/thumbnails

# Expose ports
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "WallpaperApi.dll"]
