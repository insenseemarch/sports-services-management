#!/bin/bash

DB_DIR="/Users/tuyetnhu/Documents/LEARN/HQT/sports-services-management/database"
DATA_DIR="$DB_DIR/DATA"
SCHEMA_DIR="$DB_DIR/TRUNGTAMTHETHAO"

SERVER="localhost,1433"
USER="sa"
PASS="YourStrong@Passw0rd"

echo "Starting SQL Server database setup..."
echo "=================================="

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
for i in {1..60}; do
  if sqlcmd -S "$SERVER" -U "$USER" -P "$PASS" -Q "SELECT @@VERSION" >/dev/null 2>&1; then
    echo "SQL Server is ready!"
    break
  fi
  echo "Attempt $i/60 - waiting..."
  sleep 2
done

# Run schema files in order
echo ""
echo "Running schema setup files..."
sqlcmd -S "$SERVER" -U "$USER" -P "$PASS" -i "$SCHEMA_DIR/TongHop.sql"
echo "✓ TongHop.sql completed"

sqlcmd -S "$SERVER" -U "$USER" -P "$PASS" -i "$SCHEMA_DIR/Function+SP+Trigger.sql"
echo "✓ Function+SP+Trigger.sql completed"

sqlcmd -S "$SERVER" -U "$USER" -P "$PASS" -i "$SCHEMA_DIR/Partition+IndexTable.sql"
echo "✓ Partition+IndexTable.sql completed"

sqlcmd -S "$SERVER" -U "$USER" -P "$PASS" -i "$SCHEMA_DIR/Security&Decentralization.sql"
echo "✓ Security&Decentralization.sql completed"

# Load sample data
echo ""
echo "Loading sample data..."
sqlcmd -S "$SERVER" -U "$USER" -P "$PASS" -i "$DATA_DIR/Data.sql"
echo "✓ Data.sql completed"

echo ""
echo "=================================="
echo "Database setup completed successfully!"
echo "You can now start the web application."
