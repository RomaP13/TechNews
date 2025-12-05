# Етап 1: Збірка (Build)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копіюємо файл проекту і відновлюємо залежності
COPY ["TechNews/TechNews.csproj", "TechNews/"]
RUN dotnet restore "TechNews/TechNews.csproj"

# Копіюємо весь інший код
COPY . .
WORKDIR "/src/TechNews"

# Збираємо проект у режимі Release
RUN dotnet build "TechNews.csproj" -c Release -o /app/build

# Публікуємо проект (створюємо фінальні файли для запуску)
FROM build AS publish
RUN dotnet publish "TechNews.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Етап 2: Запуск (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Налаштовуємо змінні середовища
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "TechNews.dll"]
