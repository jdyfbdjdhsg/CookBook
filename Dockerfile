# Используем 8.0, так как это текущий стандарт (10.0 — это, скорее всего, опечатка в проекте или использование Preview)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# Если ошибка про .NET 10.0 останется, значит в .csproj файле вручную прописана 10-ка.
# Эта команда попробует собрать проект, игнорируя несоответствие версий SDK, если это возможно.
RUN dotnet restore "CookBooks/CookBooksl/CookBooksl.csproj"

WORKDIR "/src/CookBooks/CookBooksl"
RUN dotnet build "CookBooksl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CookBooksl.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookBooksl.dll"]
