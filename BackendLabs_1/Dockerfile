FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackendLabs_1/BackendLabs_1.csproj", "BackendLabs_1/"]
RUN dotnet restore "BackendLabs_1/BackendLabs_1.csproj"
COPY . .
WORKDIR "/src/BackendLabs_1"
RUN dotnet build "BackendLabs_1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "BackendLabs_1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendLabs_1.dll"]
