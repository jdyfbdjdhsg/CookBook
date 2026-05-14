FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Ищем любой файл .csproj и копируем его, сохраняя структуру
COPY . .
RUN dotnet restore "CookBooksl/CookBooksl.csproj"

WORKDIR "/src/CookBooksl"
RUN dotnet build "CookBooksl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CookBooksl.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookBooksl.dll"]
