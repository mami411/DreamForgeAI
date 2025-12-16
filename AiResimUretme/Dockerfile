# 1. Aşama: Kodları Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# 2. Aşama: Çalıştırma (Run)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# DİKKAT: Proje ismin farklıysa aşağıdaki AiResimUretme.dll kısmını düzeltmelisin.
ENTRYPOINT ["dotnet", "AiResimUretme.dll"]