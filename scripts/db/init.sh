#!/usr/bin/env bash
set -euo pipefail

SERVER="mssql"
USER="sa"
PASS="${MSSQL_SA_PASSWORD}"

SQLCMD="$(command -v sqlcmd || true)"
if [ -z "${SQLCMD}" ]; then
  for p in /opt/mssql-tools18/bin/sqlcmd /opt/mssql-tools/bin/sqlcmd; do
    if [ -x "$p" ]; then
      SQLCMD="$p"
      break
    fi
  done
fi

if [ -z "${SQLCMD}" ]; then
  echo "sqlcmd not found in container."
  exit 1
fi

echo "Waiting for SQL Server to accept connections..."
for i in {1..60}; do
  if "${SQLCMD}" -S "${SERVER}" -U "${USER}" -P "${PASS}" -C -Q "SELECT 1" >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

echo "Running DatabaseScript.sql..."
"${SQLCMD}" -S "${SERVER}" -U "${USER}" -P "${PASS}" -C -b -V 16 -i /init/DatabaseScript.sql

echo "Running StoredProcedures.sql..."
"${SQLCMD}" -S "${SERVER}" -U "${USER}" -P "${PASS}" -C -b -V 16 -i /init/StoredProcedures.sql

echo "DB initialized."
