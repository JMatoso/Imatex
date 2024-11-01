FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

RUN apt-get update
RUN apt-get install -y git cmake build-essential
RUN mkdir leptonica
RUN git clone --depth 1 --branch 1.82.0 https://github.com/DanBloomberg/leptonica.git /leptonica 
RUN cd leptonica

WORKDIR /leptonica
RUN mkdir build
WORKDIR /leptonica/build
RUN cmake ..

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore 
COPY . .
WORKDIR "/src/Imatex.Web"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Imatex.Web/runtimes ./runtimes
ENTRYPOINT ["dotnet", "Imatex.Web.dll"]