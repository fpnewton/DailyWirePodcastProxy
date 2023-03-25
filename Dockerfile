FROM alpine:latest

# Install support tools
RUN apk add --update --no-cache aspnetcore6-runtime libc6-compat chromium

# Copy source files
COPY publish/linux-x64 /opt/publish/linux-x64

# Configure non-root user
RUN adduser -D dwpp

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
