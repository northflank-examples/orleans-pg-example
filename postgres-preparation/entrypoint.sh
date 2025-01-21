#!/bin/bash
set -e

DB_HOST=${DB_HOST:-your-db-host}
DB_PORT=${DB_PORT:-5432}
DB_USER=${DB_USER:-your-db-user}
DB_PASSWORD=${DB_PASSWORD:-your-db-password}
DB_NAME=${DB_NAME:-your-db-name}

# Export PGPASSWORD to avoid prompt
export PGPASSWORD="$DB_PASSWORD"

psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f orleanspg.sql
