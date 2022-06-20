FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LogosAPI.csproj", "./"]
RUN dotnet restore "./LogosAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LogosAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LogosAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "LogosAPI.dll"]