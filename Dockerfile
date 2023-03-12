FROM ubuntu:latest

# Install support tools
RUN apt update && apt install unzip wget gnupg2 dotnet-sdk-6.0 -y

# Install Chrome for IGraphqlClient
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update
RUN apt --fix-broken install
RUN apt-get install google-chrome-stable -y

# Download and place DailyWirePodcastProxy release
#RUN wget https://github.com/fpnewton/DailyWirePodcastProxy/releases/download/v0.1.3/linux-x64.zip -O /tmp/linux-x64.zip
#RUN unzip /tmp/linux-x64.zip -d /tmp
#RUN mv /tmp/linux-x64/ /opt/dwpp

# Copy source files
COPY build /opt/build
COPY src /opt/src

# Build
RUN /opt/build/build-arch.sh linux-x64
RUN ls /opt/build/
RUN ls /opt

# Configure non-root user
RUN adduser dwpp

# Set permissions
RUN chown -R dwpp:root /opt/publish/linux-x64

# Change user
USER dwpp

# Set working directory
WORKDIR /opt/publish/linux-x64

# Expose port
EXPOSE 9473

# Run DailyWirePodcastProxy
ENTRYPOINT ["./DailyWirePodcastProxy"]
