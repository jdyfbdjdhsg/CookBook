FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем всё содержимое репозитория в контейнер
COPY . .

# Восстанавливаем зависимости, указывая точный путь к файлу проекта
RUN dotnet restore "CookBooksl/CookBooksl.csproj"

# Переходим в папку, где лежит проект (проверьте, что на конце именно L, а не 1)
WORKDIR "/src/CookBooksl"

# Собираем проект
RUN dotnet build "CookBooksl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CookBooksl.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookBooksl.dll"]
