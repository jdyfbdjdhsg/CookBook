# Слой для запуска
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Слой для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# ВАЖНО: Указываем путь к файлу внутри папки CookBooksl
COPY ["CookBooksl/CookBooksl.csproj", "CookBooksl/"]
RUN dotnet restore "CookBooksl/CookBooksl.csproj"

# Копируем всё остальное
COPY . .
WORKDIR "/src/CookBooksl"
RUN dotnet build "CookBooksl.csproj" -c Release -o /app/build

# Публикация
FROM build AS publish
RUN dotnet publish "CookBooksl.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный слой
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookBooksl.dll"]
