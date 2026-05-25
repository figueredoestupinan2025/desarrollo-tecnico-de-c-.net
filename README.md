# Sistema de Reservas - Fondo XYZ

## Descripción
Sistema de reservas desarrollado en .NET Core MVC para el Fondo XYZ, que permite a los asociados reservar sedes recreativas y apartamentos.

## Documentos de apoyo
- Demo reproducible paso a paso: `DEMO.md`
- Validación del anexo (requiere OCR): `ANEXO_VALIDACION.md`

## Arquitectura del Proyecto
El proyecto sigue una arquitectura en capas:

1. **ReservasFondoXYZ.Data** - Capa de Datos
   - Entidades del modelo de datos
   - ApplicationDbContext (Entity Framework Core)
   - DTOs para procedimientos almacenados

2. **ReservasFondoXYZ.Business** - Capa de Negocio
   - Servicios que implementan la lógica de negocio
   - Interfaces para los servicios
   - Integración con procedimientos almacenados

3. **ReservasFondoXYZ.Web** - Capa de Presentación (MVC)
   - Controladores
   - Vistas con Razor
   - **ViewModels separados de los controladores** en la carpeta `ViewModels`
   - Configuración de Identity para autenticación

## Tecnologías Utilizadas
- .NET Core 9.0
- ASP.NET Core MVC
- Entity Framework Core 9.0
- Microsoft SQL Server (LocalDB)
- ASP.NET Core Identity
- Bootstrap 5

## Estructura de la Base de Datos
La base de datos contiene las siguientes tablas principales:
- **TipoSitio**: Tipos de sitios (Sede Recreativa, Apartamento)
- **Sitio**: Sedes recreativas y apartamentos
- **TipoAlojamiento**: Tipos de alojamiento
- **Alojamiento**: Alojamientos dentro de cada sitio
- **Habitacion**: Habitaciones de cada alojamiento
- **TipoTemporada**: Tipos de temporada (Baja, Alta, Tarifa Especial)
- **Temporada**: Periodos de temporada
- **Tarifa**: Tarifas según sitio, temporada, alojamiento y personas
- **EstadoReserva**: Estados de reserva (Pendiente, Confirmada, Cancelada, Completada)
- **Reserva**: Reservas realizadas por los usuarios
- **ReservaHabitacion**: Relación entre reservas y habitaciones
- **AspNetUsers, AspNetRoles, etc.**: Tablas de Identity para seguridad

## Procedimientos Almacenados
Se implementaron los siguientes procedimientos almacenados:

1. **ObtenerHabitacionesDisponiblesPorFechas**: Encuentra habitaciones disponibles en un rango de fechas
2. **ObtenerHabitacionesDisponiblesPorFechasYPersonas**: Encuentra habitaciones disponibles por fechas y capacidad
3. **ObtenerTarifas**: Consulta tarifas según sitio, temporada, personas y alojamiento
4. **CalcularTarifaTotal**: Calcula la tarifa total a cancelar
5. **ObtenerTemporadaPorFecha**: Obtiene el tipo de temporada para una fecha

### Refinamiento de la Condición de Solapamiento de Fechas
La condición de solapamiento de fechas se ha refinado para manejar correctamente los casos de **mismo día de entrada/salida**:
```sql
-- Condición estándar y correcta
AND (@FechaInicio < r.FechaFin AND @FechaFin > r.FechaInicio)
```
Esto permite que una habitación esté disponible si un huésped sale el día X y otro llega el mismo día X.

## Consumo de Procedimientos Almacenados desde C#
Todos los procedimientos almacenados se consumen desde la capa de negocio usando `FromSqlRaw` y `ExecuteSqlRawAsync`, con parámetros tipados (`SqlParameter`) para prevenir SQL injection:
- `ObtenerHabitacionesDisponiblesAsync`: Llama a `ObtenerHabitacionesDisponiblesPorFechas`
- `ObtenerHabitacionesDisponiblesPorPersonasAsync`: Llama a `ObtenerHabitacionesDisponiblesPorFechasYPersonas`
- `ObtenerTarifasAsync`: Llama a `ObtenerTarifas`
- `CalcularTarifaTotalAsync`: Llama a `CalcularTarifaTotal` con parámetro OUTPUT
- `ObtenerTipoTemporadaPorFechaAsync`: Llama a `ObtenerTemporadaPorFecha` con parámetro OUTPUT

## Instrucciones de Instalación y Ejecución

### Estado de validación
- Solución compilada con `dotnet build ReservasFondoXYZ.sln`.
- Resultado actual: **0 errores y 0 advertencias**.
- La aplicación arranca en ASP.NET Core MVC; para usar las pantallas que consultan datos se debe tener disponible SQL Server o LocalDB y ejecutar los scripts SQL incluidos.
- Usuario administrador inicial:
  - Documento: `999999999`
  - Contraseña: `Admin123!`
  - Rol: `Admin`

### 1. Requisitos Previos
- .NET SDK 9.0
- SQL Server (o LocalDB incluido en Visual Studio)
- Visual Studio 2022 (opcional) o Visual Studio Code

### 2. Configurar la Base de Datos
1. Abrir SQL Server Management Studio (SSMS) o Azure Data Studio
2. Ejecutar el script **DatabaseScript.sql** para crear la base de datos y tablas
3. Ejecutar el script **StoredProcedures.sql** para crear los procedimientos almacenados

También se puede ejecutar por consola si `sqlcmd` está disponible:
```powershell
sqlcmd -S "(localdb)\mssqllocaldb" -i DatabaseScript.sql
sqlcmd -S "(localdb)\mssqllocaldb" -i StoredProcedures.sql
```

### Alternativa recomendada (reproducible): SQL Server en Docker
1. Copiar `.env.example` a `.env` y definir `MSSQL_SA_PASSWORD` (password fuerte).
2. Levantar BD y ejecutar scripts:
```powershell
docker compose up -d mssql
docker compose up --no-deps db-init
```
3. Ejecutar la web apuntando al contenedor:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Docker"
$env:MSSQL_SA_PASSWORD=(Get-Content .env | Select-String -Pattern '^MSSQL_SA_PASSWORD=' | ForEach-Object { $_.Line.Split('=')[1] })
dotnet run --no-launch-profile --project .\\ReservasFondoXYZ.Web\\ReservasFondoXYZ.Web.csproj
```
Nota: `ReservasFondoXYZ.Web\\appsettings.Docker.json` usa `localhost,1433` (mapeo de puerto del contenedor en `docker-compose.yml`).

### Smoke test (rápido)
Con la BD y la web corriendo, ejecuta:
```powershell
powershell -ExecutionPolicy Bypass -File .\\scripts\\smoke\\smoke.ps1
```

### Alcance: pago en línea (MOCK)
El PDF menciona "pagar en línea". En esta implementación el flujo de "Pago" es **simulado (MOCK)** para fines de la prueba:
- Se calcula la tarifa total por SP.
- Se registra la reserva dejando una referencia en `Reserva.Observaciones` con el prefijo `Pago aprobado (Mock)`.

Configuración:
- `ReservasFondoXYZ.Web\\appsettings.json` -> `Payment:Provider` (por defecto `Mock`).
- Si se configura un proveedor distinto a `Mock`, el sistema rechaza el pago y muestra un mensaje indicando que el proveedor no está configurado (punto de extensión para integrar pasarela real).

### Nota sobre el anexo (PDF)
El archivo `RESERVAS_AnexoPruebaTecnica_mayo2026 (5).pdf` parece estar escaneado (sin texto seleccionable), por lo que para extraer requisitos adicionales se requiere OCR.
Recomendación: si el evaluador requiere validar ese anexo, convertirlo a texto (OCR) y contrastar requisitos adicionales contra el sistema.
Guía: `ANEXO_VALIDACION.md` y `scripts/ocr/README.md`.

### 3. Configurar la Cadena de Conexión
Verificar que la cadena de conexión en `ReservasFondoXYZ.Web\\appsettings.json` sea correcta para tu entorno:
- **SQL Server Express** (valor por defecto actual):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ReservasFondoXYZ;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

- **LocalDB (opcional)**: si prefieres LocalDB, puedes cambiar `Server` a `(localdb)\\mssqllocaldb`.

### 4. Ejecutar la Aplicación
1. Abrir una terminal en la carpeta del proyecto
2. Navegar a la carpeta `ReservasFondoXYZ.Web`
3. Ejecutar el comando:
   ```
   dotnet run
   ```
4. Abrir el navegador en la URL mostrada (generalmente https://localhost:5001 o http://localhost:5000)

### 5. Configurar SMTP (Recuperación de Contraseña)
Para habilitar la recuperación de contraseña por correo electrónico, edita la sección `SmtpSettings` en `appsettings.json`:
```json
"SmtpSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSsl": true,
  "UserName": "tu-correo@gmail.com",
  "Password": "tu-contraseña",
  "FromEmail": "no-reply@tudominio.com"
}
```

### Troubleshooting Docker (reinicio limpio)
Si `db-init` falló a mitad de ejecución y el volumen quedó en estado inconsistente, reinicia desde cero eliminando volumen:
```powershell
docker compose down -v
docker compose up -d mssql
docker compose up --no-deps db-init
```

## Funcionalidades Principales
- **Registro y Autenticación de Usuarios**: Mediante ASP.NET Core Identity
- **Consulta de Disponibilidad**: Buscar habitaciones disponibles por fechas y número de personas
- **Gestión de Reservas**: Crear, ver y consultar reservas, incluyendo selección de varias habitaciones y validación final de disponibilidad
- **Recuperación de Contraseña**: Mediante SMTP
- **CRUD administrativo**: Gestión completa de sitios, alojamientos, habitaciones, temporadas y tarifas
- **Seguridad por roles**: CRUD administrativo protegido para el rol `Admin`; reservas protegidas para usuarios autenticados
- **Página Principal**: Lista de sedes recreativas y apartamentos con pestañas

## Datos Iniciales
La base de datos se crea con los siguientes datos de ejemplo:
- 8 sitios (6 sedes recreativas y 2 apartamentos)
- Alojamientos y habitaciones para TODOS los sitios: Villeta, El Placer (Fusagasugá), Gonzalo Morante (Chinchiná), Tablones (Palmira), Manguruma (Santa Fe de Antioquia), Federman (Bogotá), Suramericana (Medellín) y El Rodadero (Santa Marta)
- Temporadas para 2026
- Tarifas para TODOS los sitios según la información proporcionada

## Autor
Prueba técnica desarrollada para el cargo de Desarrollador de Aplicaciones en Microsoft .NET.
