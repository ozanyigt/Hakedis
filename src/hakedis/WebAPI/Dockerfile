FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Çok katmanlı mimari kullanıyorsan bağımlılıkların çözülmesi için 
# Context'i bir üst klasörden (src) başlatıp dosyaları kopyalıyoruz
COPY . .
RUN dotnet restore "hakedis/WebAPI/WebAPI.csproj"

WORKDIR "/src/hakedis/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]