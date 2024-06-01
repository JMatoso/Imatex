FROM ubuntu:latest AS base

USER root
WORKDIR /app

RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    libtesseract-dev \
    libleptonica-dev && \
    rm -rf /var/lib/apt/lists/*

EXPOSE 10000

USER app

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Imatex.Web/Imatex.Web.csproj", "Imatex.Web/"]

RUN dotnet restore "./Imatex.Web/Imatex.Web.csproj"

COPY . .

WORKDIR "/src/Imatex.Web"

RUN dotnet build "./Imatex.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./Imatex.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Imatex.Web.dll"]