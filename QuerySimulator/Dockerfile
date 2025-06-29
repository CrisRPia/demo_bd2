# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Since the build context is now the QuerySimulator directory,
# we can copy files from the current path.
COPY *.csproj .
RUN dotnet restore

# Copy the rest of the source code and build the application
COPY . .
RUN dotnet publish -c Release -o /app/out

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "QuerySimulator.dll"]
