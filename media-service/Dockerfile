FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InkWell.Media.csproj", "./"]
RUN dotnet restore "InkWell.Media.csproj"
COPY . .
RUN dotnet build "InkWell.Media.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "InkWell.Media.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5015
ENTRYPOINT ["dotnet", "InkWell.Media.dll"]
