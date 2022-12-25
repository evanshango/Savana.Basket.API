FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Savana.Basket.API.csproj", "Savana.Basket.API/"]
RUN dotnet restore "Savana.Basket.API/Savana.Basket.API.csproj"
WORKDIR "/src/Savana.Basket.API"
COPY . .
RUN dotnet build "Savana.Basket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Savana.Basket.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Savana.Basket.API.dll"]
