FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CookBooksl.csproj", "."]
RUN dotnet restore "./CookBooksl.csproj"
COPY . .
RUN dotnet build "CookBooksl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CookBooksl.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookBooksl.dll"]