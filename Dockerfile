# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all projects
COPY ["FinanceDashboard.API/FinanceDashboard.API.csproj", "FinanceDashboard.API/"]
COPY ["FinanceDashboard.Core/FinanceDashboard.Core.csproj", "FinanceDashboard.Core/"]
COPY ["FinanceDashboard.Infrastructure/FinanceDashboard.Infrastructure.csproj", "FinanceDashboard.Infrastructure/"]

RUN dotnet restore "FinanceDashboard.API/FinanceDashboard.API.csproj"

COPY . .
RUN dotnet publish "FinanceDashboard.API/FinanceDashboard.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FinanceDashboard.API.dll"]