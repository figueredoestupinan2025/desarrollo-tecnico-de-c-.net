-- =============================================
-- Procedimientos Almacenados
-- Sistema de Reservas Fondo XYZ
-- =============================================

USE ReservasFondoXYZ;
GO

-- =============================================
-- SP: ObtenerHabitacionesDisponiblesPorFechas
-- Descripción: Encuentra habitaciones disponibles en un rango de fechas
-- =============================================
CREATE OR ALTER PROCEDURE ObtenerHabitacionesDisponiblesPorFechas
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        h.Id,
        h.Numero,
        h.Descripcion,
        h.CapacidadMaxima,
        a.Id AS AlojamientoId,
        a.Nombre AS AlojamientoNombre,
        s.Id AS SitioId,
        s.Nombre AS SitioNombre,
        ts.Nombre AS TipoSitio
    FROM Habitacion h
    INNER JOIN Alojamiento a ON h.AlojamientoId = a.Id
    INNER JOIN Sitio s ON a.SitioId = s.Id
    INNER JOIN TipoSitio ts ON s.TipoSitioId = ts.Id
    WHERE h.Activo = 1
      AND a.Activo = 1
      AND s.Activo = 1
      AND h.Id NOT IN (
          SELECT rh.HabitacionId
          FROM ReservaHabitacion rh
          INNER JOIN Reserva r ON rh.ReservaId = r.Id
          WHERE r.EstadoReservaId IN (1, 2)
            AND (
                (@FechaInicio BETWEEN r.FechaInicio AND DATEADD(DAY, -1, r.FechaFin))
                OR (@FechaFin BETWEEN DATEADD(DAY, 1, r.FechaInicio) AND r.FechaFin)
                OR (r.FechaInicio BETWEEN @FechaInicio AND DATEADD(DAY, -1, @FechaFin))
            )
      )
    ORDER BY s.Nombre, a.Nombre, h.Numero;
END
GO

-- =============================================
-- SP: ObtenerHabitacionesDisponiblesPorFechasYPersonas
-- Descripción: Encuentra habitaciones disponibles en un rango de fechas y con capacidad para N personas
-- =============================================
CREATE OR ALTER PROCEDURE ObtenerHabitacionesDisponiblesPorFechasYPersonas
    @FechaInicio DATE,
    @FechaFin DATE,
    @NumeroPersonas INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        h.Id,
        h.Numero,
        h.Descripcion,
        h.CapacidadMaxima,
        a.Id AS AlojamientoId,
        a.Nombre AS AlojamientoNombre,
        s.Id AS SitioId,
        s.Nombre AS SitioNombre,
        ts.Nombre AS TipoSitio
    FROM Habitacion h
    INNER JOIN Alojamiento a ON h.AlojamientoId = a.Id
    INNER JOIN Sitio s ON a.SitioId = s.Id
    INNER JOIN TipoSitio ts ON s.TipoSitioId = ts.Id
    WHERE h.Activo = 1
      AND a.Activo = 1
      AND s.Activo = 1
      AND h.CapacidadMaxima >= @NumeroPersonas
      AND h.Id NOT IN (
          SELECT rh.HabitacionId
          FROM ReservaHabitacion rh
          INNER JOIN Reserva r ON rh.ReservaId = r.Id
          WHERE r.EstadoReservaId IN (1, 2)
            AND (
                (@FechaInicio BETWEEN r.FechaInicio AND DATEADD(DAY, -1, r.FechaFin))
                OR (@FechaFin BETWEEN DATEADD(DAY, 1, r.FechaInicio) AND r.FechaFin)
                OR (r.FechaInicio BETWEEN @FechaInicio AND DATEADD(DAY, -1, @FechaFin))
            )
      )
    ORDER BY s.Nombre, a.Nombre, h.Numero;
END
GO

-- =============================================
-- SP: ObtenerTarifas
-- Descripción: Consulta las tarifas de acuerdo con el sitio, la temporada, el número de personas y el alojamiento
-- =============================================
CREATE OR ALTER PROCEDURE ObtenerTarifas
    @SitioId INT,
    @TipoTemporadaId INT,
    @NumeroPersonas INT,
    @AlojamientoId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        t.Id,
        t.SitioId,
        s.Nombre AS SitioNombre,
        t.AlojamientoId,
        a.Nombre AS AlojamientoNombre,
        t.TipoTemporadaId,
        tt.Nombre AS TipoTemporadaNombre,
        t.NumeroPersonasMin,
        t.NumeroPersonasMax,
        t.NumeroHabitaciones,
        t.PrecioBase,
        t.PrecioPersonaAdicional,
        t.EsTarifaEspecial,
        t.Descripcion
    FROM Tarifa t
    INNER JOIN Sitio s ON t.SitioId = s.Id
    INNER JOIN TipoTemporada tt ON t.TipoTemporadaId = tt.Id
    LEFT JOIN Alojamiento a ON t.AlojamientoId = a.Id
    WHERE t.Activo = 1
      AND t.SitioId = @SitioId
      AND t.TipoTemporadaId = @TipoTemporadaId
      AND @NumeroPersonas BETWEEN t.NumeroPersonasMin AND t.NumeroPersonasMax
      AND (t.AlojamientoId IS NULL OR t.AlojamientoId = @AlojamientoId)
    ORDER BY t.EsTarifaEspecial, t.PrecioBase;
END
GO

-- =============================================
-- SP: CalcularTarifaTotal
-- Descripción: Calcula la tarifa total a cancelar de acuerdo con los parámetros
-- =============================================
CREATE OR ALTER PROCEDURE CalcularTarifaTotal
    @SitioId INT,
    @NumeroHabitaciones INT,
    @NumeroPersonas INT,
    @AlojamientoId INT = NULL,
    @TipoTemporadaId INT,
    @FechaInicio DATE,
    @FechaFin DATE,
    @TarifaTotal DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NumeroNoches INT;
    DECLARE @PrecioBase DECIMAL(18,2);
    DECLARE @PrecioPersonaAdicional DECIMAL(18,2);
    DECLARE @NumeroPersonasBase INT;
    DECLARE @PersonasAdicionales INT;
    
    SET @NumeroNoches = DATEDIFF(DAY, @FechaInicio, @FechaFin);
    
    IF @NumeroNoches <= 0
    BEGIN
        SET @TarifaTotal = 0;
        RETURN;
    END
    
    SELECT TOP 1
        @PrecioBase = t.PrecioBase,
        @PrecioPersonaAdicional = ISNULL(t.PrecioPersonaAdicional, 0),
        @NumeroPersonasBase = t.NumeroPersonasMax
    FROM Tarifa t
    WHERE t.Activo = 1
      AND t.SitioId = @SitioId
      AND t.TipoTemporadaId = @TipoTemporadaId
      AND t.NumeroHabitaciones = @NumeroHabitaciones
      AND @NumeroPersonas BETWEEN t.NumeroPersonasMin AND t.NumeroPersonasMax
      AND (t.AlojamientoId IS NULL OR t.AlojamientoId = @AlojamientoId)
    ORDER BY t.EsTarifaEspecial, t.PrecioBase;
    
    IF @PrecioBase IS NULL
    BEGIN
        SET @TarifaTotal = 0;
        RETURN;
    END
    
    SET @PersonasAdicionales = CASE 
        WHEN @NumeroPersonas > @NumeroPersonasBase THEN @NumeroPersonas - @NumeroPersonasBase
        ELSE 0
    END;
    
    SET @TarifaTotal = (@PrecioBase + (@PersonasAdicionales * @PrecioPersonaAdicional)) * @NumeroNoches;
END
GO

-- =============================================
-- SP: ObtenerTemporadaPorFecha
-- Descripción: Obtiene el tipo de temporada para una fecha específica
-- =============================================
CREATE OR ALTER PROCEDURE ObtenerTemporadaPorFecha
    @Fecha DATE,
    @TipoTemporadaId INT OUTPUT,
    @NombreTemporada NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1
        @TipoTemporadaId = tt.Id,
        @NombreTemporada = tt.Nombre
    FROM Temporada t
    INNER JOIN TipoTemporada tt ON t.TipoTemporadaId = tt.Id
    WHERE t.Activo = 1
      AND @Fecha BETWEEN t.FechaInicio AND t.FechaFin
    ORDER BY tt.Id DESC;
    
    IF @TipoTemporadaId IS NULL
    BEGIN
        SELECT @TipoTemporadaId = Id, @NombreTemporada = Nombre
        FROM TipoTemporada
        WHERE Nombre = 'Baja Temporada';
    END
END
GO

PRINT 'Procedimientos almacenados creados exitosamente!';
GO
