-- =============================================
-- Script de Creación de Base de Datos
-- Sistema de Reservas Fondo XYZ
-- =============================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ReservasFondoXYZ')
BEGIN
    ALTER DATABASE ReservasFondoXYZ SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ReservasFondoXYZ;
END
GO

CREATE DATABASE ReservasFondoXYZ;
GO

USE ReservasFondoXYZ;
GO

-- =============================================
-- Tabla: TipoSitio
-- =============================================
CREATE TABLE TipoSitio (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- =============================================
-- Tabla: Sitio
-- =============================================
CREATE TABLE Sitio (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    TipoSitioId INT NOT NULL,
    Ubicacion NVARCHAR(200),
    Descripcion NVARCHAR(MAX),
    CapacidadTotal INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (TipoSitioId) REFERENCES TipoSitio(Id)
);
GO

-- =============================================
-- Tabla: TipoAlojamiento
-- =============================================
CREATE TABLE TipoAlojamiento (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- =============================================
-- Tabla: Alojamiento
-- =============================================
CREATE TABLE Alojamiento (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SitioId INT NOT NULL,
    TipoAlojamientoId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(MAX),
    CapacidadMaxima INT NOT NULL,
    NumeroHabitaciones INT NOT NULL DEFAULT 1,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (SitioId) REFERENCES Sitio(Id),
    FOREIGN KEY (TipoAlojamientoId) REFERENCES TipoAlojamiento(Id)
);
GO

-- =============================================
-- Tabla: Habitacion
-- =============================================
CREATE TABLE Habitacion (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AlojamientoId INT NOT NULL,
    Numero NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(MAX),
    CapacidadMaxima INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (AlojamientoId) REFERENCES Alojamiento(Id)
);
GO

-- =============================================
-- Tabla: TipoTemporada
-- =============================================
CREATE TABLE TipoTemporada (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- =============================================
-- Tabla: Temporada
-- =============================================
CREATE TABLE Temporada (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TipoTemporadaId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    Descripcion NVARCHAR(MAX),
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (TipoTemporadaId) REFERENCES TipoTemporada(Id)
);
GO

-- =============================================
-- Tabla: Tarifa
-- =============================================
CREATE TABLE Tarifa (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SitioId INT NOT NULL,
    AlojamientoId INT NULL,
    TipoTemporadaId INT NOT NULL,
    NumeroPersonasMin INT NOT NULL,
    NumeroPersonasMax INT NOT NULL,
    NumeroHabitaciones INT NOT NULL DEFAULT 1,
    PrecioBase DECIMAL(18,2) NOT NULL,
    PrecioPersonaAdicional DECIMAL(18,2) NULL,
    EsTarifaEspecial BIT NOT NULL DEFAULT 0,
    Descripcion NVARCHAR(MAX),
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (SitioId) REFERENCES Sitio(Id),
    FOREIGN KEY (AlojamientoId) REFERENCES Alojamiento(Id),
    FOREIGN KEY (TipoTemporadaId) REFERENCES TipoTemporada(Id)
);
GO

-- =============================================
-- Tabla: AspNetRoles (Identity)
-- =============================================
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL
);
GO

-- =============================================
-- Tabla: AspNetUsers (Identity)
-- =============================================
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DocumentoIdentidad NVARCHAR(50) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL
);
GO

-- =============================================
-- Tabla: AspNetUserClaims (Identity)
-- =============================================
CREATE TABLE AspNetUserClaims (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
GO

-- =============================================
-- Tabla: AspNetUserLogins (Identity)
-- =============================================
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
GO

-- =============================================
-- Tabla: AspNetUserRoles (Identity)
-- =============================================
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
);
GO

-- =============================================
-- Tabla: AspNetUserTokens (Identity)
-- =============================================
CREATE TABLE AspNetUserTokens (
    UserId NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(128) NOT NULL,
    Name NVARCHAR(128) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
GO

-- =============================================
-- Tabla: EstadoReserva
-- =============================================
CREATE TABLE EstadoReserva (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- =============================================
-- Tabla: Reserva
-- =============================================
CREATE TABLE Reserva (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId NVARCHAR(450) NOT NULL,
    SitioId INT NOT NULL,
    AlojamientoId INT NOT NULL,
    EstadoReservaId INT NOT NULL DEFAULT 1,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    NumeroPersonas INT NOT NULL,
    NumeroHabitaciones INT NOT NULL,
    TarifaTotal DECIMAL(18,2) NOT NULL,
    FechaReserva DATETIME NOT NULL DEFAULT GETDATE(),
    Observaciones NVARCHAR(MAX),
    FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (SitioId) REFERENCES Sitio(Id),
    FOREIGN KEY (AlojamientoId) REFERENCES Alojamiento(Id),
    FOREIGN KEY (EstadoReservaId) REFERENCES EstadoReserva(Id)
);
GO

-- =============================================
-- Tabla: ReservaHabitacion
-- =============================================
CREATE TABLE ReservaHabitacion (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ReservaId INT NOT NULL,
    HabitacionId INT NOT NULL,
    FOREIGN KEY (ReservaId) REFERENCES Reserva(Id),
    FOREIGN KEY (HabitacionId) REFERENCES Habitacion(Id)
);
GO

-- =============================================
-- ÍNDICES
-- =============================================
CREATE INDEX IX_Sitio_TipoSitioId ON Sitio(TipoSitioId);
CREATE INDEX IX_Alojamiento_SitioId ON Alojamiento(SitioId);
CREATE INDEX IX_Alojamiento_TipoAlojamientoId ON Alojamiento(TipoAlojamientoId);
CREATE INDEX IX_Habitacion_AlojamientoId ON Habitacion(AlojamientoId);
CREATE INDEX IX_Temporada_TipoTemporadaId ON Temporada(TipoTemporadaId);
CREATE INDEX IX_Tarifa_SitioId ON Tarifa(SitioId);
CREATE INDEX IX_Tarifa_AlojamientoId ON Tarifa(AlojamientoId);
CREATE INDEX IX_Tarifa_TipoTemporadaId ON Tarifa(TipoTemporadaId);
CREATE INDEX IX_Reserva_UsuarioId ON Reserva(UsuarioId);
CREATE INDEX IX_Reserva_SitioId ON Reserva(SitioId);
CREATE INDEX IX_Reserva_AlojamientoId ON Reserva(AlojamientoId);
CREATE INDEX IX_Reserva_EstadoReservaId ON Reserva(EstadoReservaId);
CREATE INDEX IX_Reserva_Fechas ON Reserva(FechaInicio, FechaFin);
CREATE INDEX IX_ReservaHabitacion_ReservaId ON ReservaHabitacion(ReservaId);
CREATE INDEX IX_ReservaHabitacion_HabitacionId ON ReservaHabitacion(HabitacionId);
GO

-- =============================================
-- DATOS INICIALES
-- =============================================

-- Tipos de Sitio
INSERT INTO TipoSitio (Nombre) VALUES 
('Sede Recreativa'),
('Apartamento');
GO

-- Tipos de Alojamiento
INSERT INTO TipoAlojamiento (Nombre) VALUES 
('Habitación'),
('Alojamiento'),
('Cabaña'),
('Apartamento');
GO

-- Tipos de Temporada
INSERT INTO TipoTemporada (Nombre) VALUES 
('Baja Temporada'),
('Alta Temporada'),
('Tarifa Especial');
GO

-- Estados de Reserva
INSERT INTO EstadoReserva (Nombre) VALUES 
('Pendiente'),
('Confirmada'),
('Cancelada'),
('Completada');
GO

-- Sitios
INSERT INTO Sitio (Nombre, TipoSitioId, Ubicacion, Descripcion, CapacidadTotal, Activo) VALUES 
('Villeta', 1, 'Villeta, Cundinamarca', 'Sede recreativa con 8 habitaciones, cada una con cama doble y camarote, baño, nevera, televisor y terraza cubierta.', 32, 1),
('El Placer - Fusagasugá', 1, 'Fusagasugá, Cundinamarca', 'Sede recreativa con alojamientos y bloque de cabañas.', 34, 1),
('Gonzalo Morante - Chinchiná', 1, 'Chinchiná, Caldas', 'Sede recreativa con alojamientos y bloque de cabañas.', 30, 1),
('Tablones - Palmira', 1, 'Palmira, Valle del Cauca', 'Sede recreativa con alojamientos.', 24, 1),
('Manguruma - Santa Fe de Antioquia', 1, 'Santa Fe de Antioquia, Antioquia', 'Sede recreativa con alojamientos y bloque nuevo.', 46, 1),
('Federman - Bogotá', 1, 'Bogotá, D.C.', 'Sede recreativa con servicios completos y 4 habitaciones.', 4, 1),
('Suramericana - Medellín', 2, 'Medellín, Antioquia', 'Apartamento con 5 habitaciones.', 10, 1),
('El Rodadero - Santa Marta', 2, 'Santa Marta, Magdalena', 'Apartamentos turísticos.', 20, 1);
GO

-- Alojamientos para Villeta
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(1, 1, 'Habitaciones Villeta', 'Ocho habitaciones cada una con una alcoba que tiene una cama doble y un camarote, baño, nevera, televisor y terraza cubierta.', 32, 8, 1);
GO

-- Habitaciones para Villeta
DECLARE @AlojamientoVilleta INT = SCOPE_IDENTITY();
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoVilleta, '101', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '102', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '103', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '104', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '105', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '106', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '107', 'Habitación con cama doble y camarote', 4, 1),
(@AlojamientoVilleta, '108', 'Habitación con cama doble y camarote', 4, 1);
GO

-- Alojamientos para Suramericana - Medellín
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(7, 4, 'Apartamento Suramericana', 'Apartamento con 5 habitaciones', 10, 5, 1);
GO

-- Habitaciones para Suramericana - Medellín
DECLARE @AlojamientoMedellin INT = SCOPE_IDENTITY();
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoMedellin, '1', 'Habitación 1 con 2 camas sencillas y baño privado', 2, 1),
(@AlojamientoMedellin, '2', 'Habitación 2 con 2 camas sencillas', 2, 1),
(@AlojamientoMedellin, '3', 'Habitación 3 con 2 camas sencillas', 2, 1),
(@AlojamientoMedellin, '4', 'Habitación 4 con 2 camas sencillas', 2, 1),
(@AlojamientoMedellin, '5', 'Habitación 5 con 1 cama sencilla y baño privado', 1, 1);
GO

-- Alojamientos para El Rodadero - Santa Marta
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(8, 4, 'Apartamento 202', 'Tiene sala comedor, cocina, 2 baños, tres habitaciones y un sitio para parqueo. Capacidad máxima: 8 personas.', 8, 3, 1),
(8, 4, 'Apartamento 301', 'Tiene Sala comedor, cocina, 1 baño, dos habitaciones y un sitio para parqueo. Capacidad máxima: 6 personas.', 6, 2, 1),
(8, 4, 'Apartamento 401', 'Tiene Sala comedor, cocina, 1 baño, dos habitaciones y un sitio para parqueo. Capacidad máxima: 6 personas.', 6, 2, 1);
GO

-- Temporadas
INSERT INTO Temporada (TipoTemporadaId, Nombre, FechaInicio, FechaFin, Descripcion, Activo) VALUES 
(1, 'Baja Temporada 2026', '2026-01-01', '2026-06-15', 'Temporada baja', 1),
(2, 'Alta Temporada 2026', '2026-06-16', '2026-08-31', 'Temporada alta de verano', 1),
(2, 'Alta Temporada Navidad 2026', '2026-12-15', '2027-01-15', 'Temporada alta de Navidad', 1),
(3, 'Tarifa Especial', '2026-01-01', '2026-12-31', 'Tarifa especial para días entre semana', 1);
GO

-- Tarifas
-- Tarifas para Sedes Recreativas (Baja Temporada)
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(1, NULL, 1, 1, 4, 1, 70000.00, 16000.00, 0, 'Alojamiento 1 habitación/noche 1-4 personas', 1),
(1, NULL, 1, 1, 4, 2, 90000.00, 16000.00, 0, 'Alojamiento 2 habitaciones/noche 1-4 personas', 1),
(1, NULL, 3, 1, 4, 1, 27000.00, 11000.00, 1, 'Tarifa especial 1 habitación 1-4 personas', 1),
(1, NULL, 3, 1, 4, 2, 37000.00, 11000.00, 1, 'Tarifa especial 2 habitaciones 1-4 personas', 1);
GO

-- Tarifas para Apartamentos Suramericana - Medellín
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(7, 2, 1, 1, 1, 1, 63000.00, NULL, 0, 'Habitación/noche una persona', 1),
(7, 2, 1, 2, 2, 1, 75000.00, NULL, 0, 'Habitación/noche dos personas', 1);
GO

-- Tarifas para Apartamentos El Rodadero - Santa Marta (Baja Temporada)
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(8, NULL, 1, 1, 6, 2, 89000.00, NULL, 0, 'Baja temporada - Apto 301-401 hasta 6 personas', 1),
(8, NULL, 1, 1, 8, 3, 103000.00, NULL, 0, 'Baja temporada - Apto 202 hasta 8 personas', 1),
(8, NULL, 2, 1, 6, 2, 124000.00, NULL, 0, 'Alta temporada - Apto 301-401 hasta 6 personas', 1),
(8, NULL, 2, 1, 8, 3, 143000.00, NULL, 0, 'Alta temporada - Apto 202 hasta 8 personas', 1);
GO

PRINT 'Base de datos creada exitosamente!';
GO
