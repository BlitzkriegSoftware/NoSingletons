############################################################
# Dockerfile to build Customer API 
# container image, based on aspnetcore image
############################################################
FROM microsoft/dotnet

# Labels
LABEL Author="Stuart Williams <stuartw@magenic.com>"
LABEL Version="1.2"

HEALTHCHECK NONE

# Variables
ENV WDIR /app

ARG source

# Create app directory
RUN mkdir -p ${WDIR}
WORKDIR ${WDIR}

# Bundle app source
COPY ${source:-bin/linux-x64} .

# Verify files are there
RUN find ${WDIR} -type f -follow -print

# Start the app
CMD ["/app/Demo-RedisElector-Singleton", "/app/KubeConfig.json"]