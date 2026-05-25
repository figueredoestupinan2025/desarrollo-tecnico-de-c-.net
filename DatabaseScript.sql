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
-- Tabla: AspNetRoleClaims (Identity)
-- =============================================
CREATE TABLE AspNetRoleClaims (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id)
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

-- Alojamientos para El Placer - Fusagasugá
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(2, 2, 'Alojamiento 1', 'Tiene dos habitaciones, baño y Televisor, una con cama doble y una sencilla, la otra con una cama sencilla.', 5, 2, 1),
(2, 2, 'Alojamiento 2', 'Tiene dos habitaciones, baño y Televisor, una con cama doble, la otra con 4 camas sencillas.', 6, 2, 1),
(2, 2, 'Alojamiento 3', 'Tiene una habitación con cama doble y 2 camas sencillas, baño y Televisor.', 4, 1, 1),
(2, 2, 'Alojamiento 4', 'Tiene dos habitaciones, baño y Televisor, una con cama doble y una sencilla, la otra con una cama sencilla.', 4, 2, 1),
(2, 3, 'Cabaña 5', 'Sala de estar con sofá cama y Televisor, baño, habitación con cama doble y una cama sencilla, cocineta equipada y nevera, terraza comedor.', 4, 1, 1),
(2, 3, 'Cabaña 6', 'Sala de estar con sofá cama y Televisor, baño, habitación con cama doble y una cama sencilla, cocineta equipada y nevera, terraza comedor.', 4, 1, 1),
(2, 3, 'Cabaña 7', 'Sala de estar con sofá cama y Televisor, baño, habitación con cama doble y una cama sencilla, cocineta equipada y nevera, terraza comedor.', 4, 1, 1),
(2, 3, 'Cabaña 8', 'Sala de estar con sofá cama y Televisor, baño, habitación con cama doble y una cama sencilla, cocineta equipada y nevera, terraza comedor.', 4, 1, 1);
GO

-- Alojamientos para Gonzalo Morante - Chinchiná
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(3, 2, 'Alojamiento 1', 'Tiene cocineta, baño, Televisor y 2 habitaciones. La 1 con dos camas sencillas, mas dos adicionales. La 2 con una cama doble y una sencilla.', 6, 2, 1),
(3, 2, 'Alojamiento 2', 'Tiene cocineta, baño, Televisor y 2 habitaciones. La 1 con una cama doble, mas una auxiliar doble. La 2 con dos camas sencillas, mas dos auxiliares.', 6, 2, 1),
(3, 2, 'Alojamiento 4', 'Tiene cocineta, baño, Televisor y una habitación con cama doble y una cama sencilla.', 3, 1, 1),
(3, 3, 'Alojamiento 3 (Tipo A)', 'Cocineta, dos baños, sala comedor, Televisor y dos habitaciones. La 1 con cama doble. La 2 con dos camas sencillas, mas dos auxiliares.', 5, 2, 1),
(3, 3, 'Alojamiento 5 (Tipo B)', 'Cocineta, baño, sala con sofá, Televisor una habitación con cama doble y una cama sencilla.', 3, 1, 1),
(3, 3, 'Alojamiento 6 (Tipo B)', 'Cocineta, baño, sala con sofá, Televisor, una habitación con cama doble y una cama sencilla.', 3, 1, 1);
GO

-- Alojamientos para Tablones - Palmira
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(4, 2, 'Alojamiento 1', 'Una habitación, con cama doble y un camarote. Televisor, baño, cocineta con nevera, comedor.', 4, 1, 1),
(4, 2, 'Alojamiento 2', 'Una habitación, con cama doble y un camarote. Televisor, baño y cocineta con nevera, comedor.', 4, 1, 1),
(4, 2, 'Alojamiento 3', 'Dos habitaciones. La habitación 1 con cama doble y un camarote. La habitación 2 con dos camarotes. Sala de estar con Televisor, baño y cocineta.', 6, 2, 1),
(4, 2, 'Alojamiento 4', 'Dos habitaciones. La habitación 1 con cama doble y un camarote. La habitación 2 con dos camarotes. Sala de estar con Televisor, baño y cocineta.', 6, 2, 1);
GO

-- Alojamientos para Manguruma - Santa Fe de Antioquia
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(5, 2, 'Alojamiento 1', 'Una cama doble y un camarote. Baño y terraza. Televisor.', 4, 1, 1),
(5, 2, 'Alojamiento 2', 'Una cama doble, un camarote y un sofá- cama. Baño y terraza. Televisor.', 5, 1, 1),
(5, 2, 'Alojamiento 3', 'Una cama doble, un camarote y un sofá- cama. Baño y terraza. Televisor.', 5, 1, 1),
(5, 2, 'Alojamiento Nuevo 1', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 2', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 3', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 4', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 5', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 6', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 7', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1),
(5, 2, 'Alojamiento Nuevo 8', 'Una habitación que tiene dos camas gemelas y un camarote; baño, terraza - comedor y cocina. Nevera y televisor.', 4, 1, 1);
GO

-- Alojamientos para Federman - Bogotá
INSERT INTO Alojamiento (SitioId, TipoAlojamientoId, Nombre, Descripcion, CapacidadMaxima, NumeroHabitaciones, Activo) VALUES 
(6, 1, 'Habitación 1', 'Habitación para alojamiento de asociados.', 2, 1, 1),
(6, 1, 'Habitación 2', 'Habitación para alojamiento de asociados.', 2, 1, 1),
(6, 1, 'Habitación 3', 'Habitación para alojamiento de asociados.', 2, 1, 1),
(6, 1, 'Habitación 4', 'Habitación para alojamiento de asociados.', 2, 1, 1);
GO

-- Habitaciones para El Placer - Fusagasugá
DECLARE @AlojamientoFusaga INT;
SELECT TOP 1 @AlojamientoFusaga = Id FROM Alojamiento WHERE SitioId = 2;
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoFusaga, '101', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '102', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '103', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '104', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '105', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '106', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '107', 'Habitación con 4 personas', 4, 1),
(@AlojamientoFusaga, '108', 'Habitación con 4 personas', 4, 1);
GO

-- Habitaciones para Gonzalo Morante - Chinchiná
DECLARE @AlojamientoChinchina INT;
SELECT TOP 1 @AlojamientoChinchina = Id FROM Alojamiento WHERE SitioId = 3;
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoChinchina, '201', 'Habitación con 4 personas', 4, 1),
(@AlojamientoChinchina, '202', 'Habitación con 4 personas', 4, 1),
(@AlojamientoChinchina, '203', 'Habitación con 4 personas', 4, 1),
(@AlojamientoChinchina, '204', 'Habitación con 4 personas', 4, 1),
(@AlojamientoChinchina, '205', 'Habitación con 4 personas', 4, 1),
(@AlojamientoChinchina, '206', 'Habitación con 4 personas', 4, 1),
(@AlojamientoChinchina, '207', 'Habitación con 4 personas', 4, 1);
GO

-- Habitaciones para Tablones - Palmira
DECLARE @AlojamientoPalmira INT;
SELECT TOP 1 @AlojamientoPalmira = Id FROM Alojamiento WHERE SitioId = 4;
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoPalmira, '301', 'Habitación con 4 personas', 4, 1),
(@AlojamientoPalmira, '302', 'Habitación con 4 personas', 4, 1),
(@AlojamientoPalmira, '303', 'Habitación con 4 personas', 4, 1),
(@AlojamientoPalmira, '304', 'Habitación con 4 personas', 4, 1),
(@AlojamientoPalmira, '305', 'Habitación con 4 personas', 4, 1),
(@AlojamientoPalmira, '306', 'Habitación con 4 personas', 4, 1);
GO

-- Habitaciones para Manguruma - Santa Fe de Antioquia
DECLARE @AlojamientoManguruma INT;
SELECT TOP 1 @AlojamientoManguruma = Id FROM Alojamiento WHERE SitioId = 5;
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoManguruma, '401', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '402', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '403', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '404', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '405', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '406', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '407', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '408', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '409', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '410', 'Habitación con 4 personas', 4, 1),
(@AlojamientoManguruma, '411', 'Habitación con 4 personas', 4, 1);
GO

-- Habitaciones para Federman - Bogotá
DECLARE @AlojamientoFederman INT;
SELECT TOP 1 @AlojamientoFederman = Id FROM Alojamiento WHERE SitioId = 6;
INSERT INTO Habitacion (AlojamientoId, Numero, Descripcion, CapacidadMaxima, Activo) VALUES 
(@AlojamientoFederman, '1', 'Habitación 1 con 2 camas sencillas', 2, 1),
(@AlojamientoFederman, '2', 'Habitación 2 con 2 camas sencillas', 2, 1),
(@AlojamientoFederman, '3', 'Habitación 3 con 2 camas sencillas', 2, 1),
(@AlojamientoFederman, '4', 'Habitación 4 con 2 camas sencillas', 2, 1);
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

-- Tarifas para El Placer - Fusagasugá
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(2, NULL, 1, 1, 4, 1, 70000.00, 16000.00, 0, 'Alojamiento 1 habitación/noche 1-4 personas', 1),
(2, NULL, 1, 1, 4, 2, 90000.00, 16000.00, 0, 'Alojamiento 2 habitaciones/noche 1-4 personas', 1),
(2, NULL, 3, 1, 4, 1, 27000.00, 11000.00, 1, 'Tarifa especial 1 habitación 1-4 personas', 1),
(2, NULL, 3, 1, 4, 2, 37000.00, 11000.00, 1, 'Tarifa especial 2 habitaciones 1-4 personas', 1);
GO

-- Tarifas para Gonzalo Morante - Chinchiná
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(3, NULL, 1, 1, 4, 1, 70000.00, 16000.00, 0, 'Alojamiento 1 habitación/noche 1-4 personas', 1),
(3, NULL, 1, 1, 4, 2, 90000.00, 16000.00, 0, 'Alojamiento 2 habitaciones/noche 1-4 personas', 1),
(3, NULL, 3, 1, 4, 1, 27000.00, 11000.00, 1, 'Tarifa especial 1 habitación 1-4 personas', 1),
(3, NULL, 3, 1, 4, 2, 37000.00, 11000.00, 1, 'Tarifa especial 2 habitaciones 1-4 personas', 1);
GO

-- Tarifas para Tablones - Palmira
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(4, NULL, 1, 1, 4, 1, 70000.00, 16000.00, 0, 'Alojamiento 1 habitación/noche 1-4 personas', 1),
(4, NULL, 1, 1, 4, 2, 90000.00, 16000.00, 0, 'Alojamiento 2 habitaciones/noche 1-4 personas', 1),
(4, NULL, 3, 1, 4, 1, 27000.00, 11000.00, 1, 'Tarifa especial 1 habitación 1-4 personas', 1),
(4, NULL, 3, 1, 4, 2, 37000.00, 11000.00, 1, 'Tarifa especial 2 habitaciones 1-4 personas', 1);
GO

-- Tarifas para Manguruma - Santa Fe de Antioquia
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(5, NULL, 1, 1, 4, 1, 70000.00, 16000.00, 0, 'Alojamiento 1 habitación/noche 1-4 personas', 1),
(5, NULL, 1, 1, 4, 2, 90000.00, 16000.00, 0, 'Alojamiento 2 habitaciones/noche 1-4 personas', 1),
(5, NULL, 3, 1, 4, 1, 27000.00, 11000.00, 1, 'Tarifa especial 1 habitación 1-4 personas', 1),
(5, NULL, 3, 1, 4, 2, 37000.00, 11000.00, 1, 'Tarifa especial 2 habitaciones 1-4 personas', 1);
GO

-- Tarifas para Federman - Bogotá
INSERT INTO Tarifa (SitioId, AlojamientoId, TipoTemporadaId, NumeroPersonasMin, NumeroPersonasMax, NumeroHabitaciones, PrecioBase, PrecioPersonaAdicional, EsTarifaEspecial, Descripcion, Activo) VALUES 
(6, NULL, 1, 1, 2, 1, 50000.00, NULL, 0, 'Habitación/noche para 1-2 personas', 1),
(6, NULL, 2, 1, 2, 1, 65000.00, NULL, 0, 'Habitación/noche alta temporada para 1-2 personas', 1);
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
