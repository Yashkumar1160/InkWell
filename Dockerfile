FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["InkWell.Auth.csproj", "./"]
RUN dotnet restore "InkWell.Auth.csproj"

# Copy the rest of the code and build
COPY . .
RUN dotnet build "InkWell.Auth.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "InkWell.Auth.csproj" -c Release -o /app/publish

# Final stage: Run the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 5058
ENTRYPOINT ["dotnet", "InkWell.Auth.dll"]
