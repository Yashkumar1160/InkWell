FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InkWell.Comment.csproj", "./"]
RUN dotnet restore "InkWell.Comment.csproj"
COPY . .
RUN dotnet build "InkWell.Comment.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "InkWell.Comment.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5101
ENTRYPOINT ["dotnet", "InkWell.Comment.dll"]
