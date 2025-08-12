FROM ubuntu:latest

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

# Run PodcastProxy.Web
ENTRYPOINT /opt/publish/linux-x64/PodcastProxy.Web
