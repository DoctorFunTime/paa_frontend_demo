# Stage 1: Build all components using the Solution file
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "paa_frontend.sln"

# Point directly at the server .csproj file
RUN dotnet publish "paa_frontend/paa_frontend/paa_frontend.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 2: Final runtime execution environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "paa_frontend.dll"]