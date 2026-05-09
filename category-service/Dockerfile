FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InkWell.Category.csproj", "./"]
RUN dotnet restore "InkWell.Category.csproj"
COPY . .
RUN dotnet build "InkWell.Category.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "InkWell.Category.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5184
ENTRYPOINT ["dotnet", "InkWell.Category.dll"]
