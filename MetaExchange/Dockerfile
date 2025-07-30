# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY MetaExchange.Console/*.csproj ./MetaExchange.Console/
COPY MetaExchange.Console/Properties/launchSettings.json ./Properties/

# Copy the Resources folder that includes orderbooks.json
COPY MetaExchange.Console/Resources/Data ./Resources/Data

# Restore dependencies
RUN dotnet restore ./MetaExchange.Console/MetaExchange.Console.csproj

# Copy the rest of the source code
COPY . .

# Publish the app
RUN dotnet publish MetaExchange.Console -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app and Resources folder from build stage
COPY --from=build /app/publish .
COPY --from=build /src/Properties/launchSettings.json ./Properties/

# Set entry point
ENTRYPOINT ["dotnet", "MetaExchange.Console.dll"]