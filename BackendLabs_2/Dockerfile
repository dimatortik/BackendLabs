FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackendLabs_2/BackendLabs_2.csproj", "BackendLabs_2/"]
RUN dotnet restore "BackendLabs_2/BackendLabs_2.csproj"
COPY . .
WORKDIR "/src/BackendLabs_2"
RUN dotnet build "BackendLabs_2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendLabs_2.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendLabs_2.dll"]
