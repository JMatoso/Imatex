# Etapa base com .NET Runtime e instalação do Tesseract
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Temporariamente troque para root para instalar as dependências
USER root

# Atualizar e instalar dependências do Tesseract
RUN apt-get update && \
    apt-get install -y tesseract-ocr libtesseract-dev libleptonica-dev

# Instalação de Leptonica (dependência do Tesseract)
RUN apt-get install -y git cmake build-essential
RUN mkdir /leptonica
RUN git clone --depth 1 --branch 1.82.0 https://github.com/DanBloomberg/leptonica.git /leptonica
WORKDIR /leptonica
RUN mkdir build
WORKDIR /leptonica/build
RUN cmake ..
RUN make
RUN make install

# Volte para o usuário app após a instalação
USER app

# Etapa build com .NET SDK para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
WORKDIR "/src/Imatex.Web"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa publish para gerar a publicação final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

# Etapa final com .NET Runtime e aplicação publicada
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/Imatex.Web/runtimes ./runtimes

# Definir o ponto de entrada
ENTRYPOINT ["dotnet", "Imatex.Web.dll"]
