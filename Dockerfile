# --- Stage 1: Build the application ---
FROM ://microsoft.com AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code and build/publish the release
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# --- Stage 2: Final runtime image ---
FROM ://microsoft.com AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose port 8080 (the default for .NET 8 container images)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Define the entry point for the container
ENTRYPOINT ["dotnet", "HelloWorldApp.dll"]
