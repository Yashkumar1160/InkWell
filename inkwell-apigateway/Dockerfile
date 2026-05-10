FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InkWell.Gateway.csproj", "./"]
RUN dotnet restore "InkWell.Gateway.csproj"
COPY . .
RUN dotnet build "InkWell.Gateway.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "InkWell.Gateway.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5032
ENTRYPOINT ["dotnet", "InkWell.Gateway.dll"]
