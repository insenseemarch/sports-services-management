#!/bin/bash
# Script to apply PATCH_AllowMaintenanceStatusUpdate.sql

echo "Applying PATCH: Allow Maintenance Status Update..."

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -d TRUNGTAMTHETHAO -i database/TRUNGTAMTHETHAO/PATCH_AllowMaintenanceStatusUpdate.sql

echo "PATCH applied successfully!"
