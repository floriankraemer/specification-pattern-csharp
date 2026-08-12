FROM mcr.microsoft.com/dotnet/sdk:8.0

# Create a dotnet user with the same UID/GID as the host user to avoid permission issues
RUN groupadd -g 1000 dotnet && \
    useradd -u 1000 -g 1000 -m -s /bin/bash dotnet

WORKDIR /app

RUN chown -R dotnet:dotnet /app

USER dotnet

# Keep container running
CMD ["tail", "-f", "/dev/null"]
