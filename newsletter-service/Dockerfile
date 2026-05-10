FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InkWell.Newsletter.csproj", "./"]
RUN dotnet restore "InkWell.Newsletter.csproj"
COPY . .
RUN dotnet build "InkWell.Newsletter.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "InkWell.Newsletter.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5046
ENTRYPOINT ["dotnet", "InkWell.Newsletter.dll"]
