FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем абсолютно всё, что есть в репозитории
COPY . .

# Команда сама найдет файл .csproj в любой подпапке и восстановит его
RUN dotnet restore "CookBooks/CookBooksl/CookBooksl.csproj"

# Переходим в конечную папку с проектом
WORKDIR "/src/CookBooks/CookBooksl"

# Сборка
RUN dotnet build "CookBooksl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CookBooksl.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookBooksl.dll"]
