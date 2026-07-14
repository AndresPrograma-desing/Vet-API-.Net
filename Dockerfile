# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivo .csproj y restaurar dependencias
COPY ["vet-api-Net.csproj", "./"]
RUN dotnet restore "vet-api-Net.csproj"

# Copiar todo el código y compilar en modo Release
COPY . .
RUN dotnet build "vet-api-Net.csproj" -c Release -o /app/build

# Stage 2: Publish (Publicar la app)
FROM build AS publish
RUN dotnet publish "vet-api-Net.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime (Imagen final ligera de ejecución)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Exponer el puerto por defecto de .NET 8+
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Instalar fontconfig y fuentes del sistema (CRÍTICO para que QuestPDF genere reportes en Linux sin fallos)
RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    fonts-dejavu \
    && rm -rf /var/lib/apt/lists/*

# Copiar la aplicación compilada desde la etapa anterior
COPY --from=publish /app/publish .

# Asegurar que exista la carpeta para archivos estáticos (facturas, reportes, etc.)
RUN mkdir -p wwwroot

ENTRYPOINT ["dotnet", "vet-api-Net.dll"]
