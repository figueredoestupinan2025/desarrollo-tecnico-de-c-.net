# Sistema de Reservas - Fondo XYZ

## Descripción
Sistema de reservas desarrollado en .NET Core MVC para el Fondo XYZ, que permite a los asociados reservar sedes recreativas y apartamentos.

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

## Instrucciones de Instalación y Ejecución

### 1. Requisitos Previos
- .NET SDK 9.0
- SQL Server (o LocalDB incluido en Visual Studio)
- Visual Studio 2022 (opcional) o Visual Studio Code

### 2. Configurar la Base de Datos
1. Abrir SQL Server Management Studio (SSMS) o Azure Data Studio
2. Ejecutar el script **DatabaseScript.sql** para crear la base de datos y tablas
3. Ejecutar el script **StoredProcedures.sql** para crear los procedimientos almacenados

### 3. Configurar la Cadena de Conexión
Verificar que la cadena de conexión en `appsettings.json` sea correcta:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ReservasFondoXYZ;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

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

## Funcionalidades Principales
- **Registro y Autenticación de Usuarios**: Mediante ASP.NET Core Identity
- **Consulta de Disponibilidad**: Buscar habitaciones disponibles por fechas y número de personas
- **Gestión de Reservas**: Crear, ver y consultar reservas
- **Página Principal**: Lista de sedes recreativas y apartamentos con pestañas

## Datos Iniciales
La base de datos se crea con los siguientes datos de ejemplo:
- 8 sitios (6 sedes recreativas y 2 apartamentos)
- Alojamientos y habitaciones para Villeta, Medellín y Santa Marta
- Temporadas para 2026
- Tarifas según la información proporcionada

## Autor
Prueba técnica desarrollada para el cargo de Desarrollador de Aplicaciones en Microsoft .NET.
