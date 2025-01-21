# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /app

# Copy the project files
COPY . ./

WORKDIR KubeClient
# Restore the project dependencies
RUN dotnet restore

# Build the project
RUN dotnet build -c Release --no-restore

# Publish the project
RUN dotnet publish -c Release -o /app/publish --no-build

# Use the .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS runtime

# Set the working directory
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "KubeClient.dll"]
