FROM ubuntu:latest

# Install support tools
RUN apt update && apt install wget gnupg2 -y

# Install Chrome for IGraphqlClient
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update
RUN apt --fix-broken install
RUN apt-get install google-chrome-stable -y

# Copy source files
COPY publish/linux-x64 /opt/publish/linux-x64

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
ENTRYPOINT ["./opt/publish/linux-64/DailyWirePodcastProxy"]
