# Demo checklist (reproducible)

## 1) Base de datos (Docker recomendado)

```powershell
copy .env.example .env
# Edita .env y define MSSQL_SA_PASSWORD (password fuerte)

docker compose up -d mssql
docker compose up --no-deps db-init
```

Si `db-init` falló antes y el volumen quedó inconsistente:
```powershell
docker compose down -v
docker compose up -d mssql
docker compose up --no-deps db-init
```

## 2) Web (modo Docker)

```powershell
$env:ASPNETCORE_ENVIRONMENT="Docker"
$env:MSSQL_SA_PASSWORD=(Get-Content .env | Select-String -Pattern '^MSSQL_SA_PASSWORD=' | ForEach-Object { $_.Line.Split('=',2)[1].Trim() })
dotnet run --no-launch-profile --project .\ReservasFondoXYZ.Web\ReservasFondoXYZ.Web.csproj
```

## 3) Flujo Usuario (asociado)

1. `Account/Register`: registrar usuario (NroDocumento + email).
2. `Account/Login`: login con NroDocumento + clave.
3. `Reserva/Disponibilidad`: buscar por fechas y # personas, seleccionar habitaciones.
4. `Reserva/Pago`: confirmar (gateway `Payment:Provider` en `appsettings.json`, por defecto **Mock**), crear reserva.
5. `Reserva/MisReservas`: ver reservas del usuario.
6. `Reserva/Detalles/{id}`: ver detalle.

## 4) Flujo Admin (CRUD)

Login con el admin inicial documentado en `README.md` y validar:
- `Sitios/Index`
- `Alojamientos/Index`
- `Habitaciones/Index`
- `Temporadas/Index`
- `Tarifas/Index`

## 5) Pruebas

```powershell
dotnet test .\ReservasFondoXYZ.Tests\ReservasFondoXYZ.Tests.csproj -c Release
```
