FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InkWell.Notification.csproj", "./"]
RUN dotnet restore "InkWell.Notification.csproj"
COPY . .
RUN dotnet build "InkWell.Notification.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "InkWell.Notification.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5131
ENTRYPOINT ["dotnet", "InkWell.Notification.dll"]
