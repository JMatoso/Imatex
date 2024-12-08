# Base stage with .NET Runtime and Tesseract installation
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Temporarily switch to root to install dependencies
USER root

# Update and install Tesseract dependencies
RUN apt-get update && \
    apt-get install -y tesseract-ocr libtesseract-dev libleptonica-dev libc6

# Install Leptonica (dependency for Tesseract)
RUN apt-get install -y git cmake build-essential
RUN mkdir /leptonica
RUN git clone --depth 1 --branch 1.82.0 https://github.com/DanBloomberg/leptonica.git /leptonica
WORKDIR /leptonica
RUN mkdir build
WORKDIR /leptonica/build
RUN cmake ..
RUN make
RUN make install

# Update the library cache
RUN ldconfig

# Create symbolic link for libleptonica in a standard library path
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /usr/lib/x86_64-linux-gnu/libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 /usr/lib/x86_64-linux-gnu/libtesseract41.so

WORKDIR /app/x64
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 libtesseract41.so

WORKDIR /app/runtimes
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 libtesseract41.so

# Switch back to the app user after installation
USER app

# Build stage with .NET SDK to compile the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all files and restore dependencies
COPY . .
WORKDIR "/src/Imatex.Web"
RUN dotnet restore

# Build the project
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish stage to generate final application output
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false 

# Final stage with .NET Runtime and published application
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
#COPY --from=build /src/Imatex.Web/runtimes ./runtimes

# Set the entrypoint
ENTRYPOINT ["dotnet", "Imatex.Web.dll"]
