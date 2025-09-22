# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["EShop.Web/EShop.Web.csproj", "EShop.Web/"]
COPY ["EShop.Service/EShop.Service.csproj", "EShop.Service/"]
COPY ["EShop.Repository/EShop.Repository.csproj", "EShop.Repository/"]
COPY ["EShop.Domain/EShop.Domain.csproj", "EShop.Domain/"]

# Restore dependencies
RUN dotnet restore "EShop.Web/EShop.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/EShop.Web"
RUN dotnet build "EShop.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShop.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "EShop.Web.dll"]