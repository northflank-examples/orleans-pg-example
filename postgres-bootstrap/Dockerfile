# Use an official lightweight image
FROM alpine:latest

# Install PostgreSQL client and bash
RUN apk update && \
    apk add --no-cache postgresql-client bash

# Create a directory for scripts
WORKDIR /scripts

# Copy your SQL script into the container
COPY orleanspg.sql .

# Copy the entrypoint script
COPY entrypoint.sh .

# Make the entrypoint script executable
RUN chmod +x entrypoint.sh

# Define the entrypoint
ENTRYPOINT ["./entrypoint.sh"]
