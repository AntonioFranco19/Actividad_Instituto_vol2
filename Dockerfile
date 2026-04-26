FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar y restaurar
COPY ["TecnoFuturo.Console/TecnoFuturo.Console.csproj", "TecnoFuturo.Console/"]
COPY ["TecnoFuturo.Core/TecnoFuturo.Core.csproj", "TecnoFuturo.Core/"]
COPY ["TecnoFuturo.Data/TecnoFuturo.Data.csproj", "TecnoFuturo.Data/"]
COPY ["TecnoFuturo.InMemory/TecnoFuturo.InMemory.csproj", "TecnoFuturo.InMemory/"]
RUN dotnet restore "TecnoFuturo.Console/TecnoFuturo.Console.csproj"

# Compilar
COPY . .
WORKDIR "/src/TecnoFuturo.Console"
RUN dotnet publish "./TecnoFuturo.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa Final
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 1. Crear las carpetas ANTES de declarar el volumen
RUN mkdir -p /app/data /app/backup && chmod -R 777 /app/data /app/backup

# Declarar los volúmenes
VOLUME ["/app/data", "/app/backup"]

ENTRYPOINT ["dotnet", "TecnoFuturo.Console.dll"]