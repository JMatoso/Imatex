FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 10000

FROM ubuntu:latest AS leptonica-builder
RUN apt install tesseract-ocr
RUN apt install libleptonica-dev

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Imatex.Web/Imatex.Web.csproj", "Imatex.Web/"]
RUN dotnet restore "./Imatex.Web/Imatex.Web.csproj"
COPY . .
WORKDIR "/src/Imatex.Web"
RUN dotnet build "./Imatex.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=leptonica-builder /leptonica-build/leptonica-1.82.0/src/.libs/leptonica-1.82.0.so /app/publish/x64/
ENTRYPOINT ["dotnet", "Imatex.Web.dll"]
