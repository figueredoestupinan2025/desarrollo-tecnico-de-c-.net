using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Tests;

public class ReservaServiceTests
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        return new ApplicationDbContext(options);
    }

    private static async Task<ApplicationDbContext> CreateSqliteContextAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();
        return context;
    }


    private static async Task<(ApplicationDbContext context, ReservaService service, int habitacionId)> SeedBasicAsync()
    {
        var context = CreateInMemoryContext();

        var sitio = new Sitio { Id = 1, Nombre = "Sitio", TipoSitioId = 1, CapacidadTotal = 10, Activo = true };
        var alojamiento = new Alojamiento { Id = 1, SitioId = 1, Sitio = sitio, TipoAlojamientoId = 1, Nombre = "Aloj", CapacidadMaxima = 10, NumeroHabitaciones = 1, Activo = true };
        var habitacion = new Habitacion { Id = 1, AlojamientoId = 1, Alojamiento = alojamiento, Numero = "101", CapacidadMaxima = 4, Activo = true };

        context.Sitios.Add(sitio);
        context.Alojamientos.Add(alojamiento);
        context.Habitaciones.Add(habitacion);

        await context.SaveChangesAsync();

        var service = new ReservaService(context, NullLogger<ReservaService>.Instance);
        return (context, service, habitacion.Id);
    }

    [Fact]
    public async Task HabitacionesDisponiblesAsync_Allows_SameDay_Checkout_Checkin()
    {
        var (context, service, habitacionId) = await SeedBasicAsync();

        var reserva = new Reserva
        {
            Id = 1,
            UsuarioId = "user",
            SitioId = 1,
            AlojamientoId = 1,
            EstadoReservaId = 2,
            FechaInicio = new DateTime(2026, 6, 10),
            FechaFin = new DateTime(2026, 6, 12),
            NumeroPersonas = 2,
            NumeroHabitaciones = 1,
            TarifaTotal = 100000,
            FechaReserva = DateTime.UtcNow
        };

        context.Reservas.Add(reserva);
        context.ReservasHabitaciones.Add(new ReservaHabitacion
        {
            Id = 1,
            ReservaId = reserva.Id,
            Reserva = reserva,
            HabitacionId = habitacionId
        });
        await context.SaveChangesAsync();

        var disponible = await service.HabitacionesDisponiblesAsync(
            habitacionesIds: new[] { habitacionId },
            fechaInicio: new DateTime(2026, 6, 12),
            fechaFin: new DateTime(2026, 6, 14));

        Assert.True(disponible);
    }

    [Fact]
    public async Task HabitacionesDisponiblesAsync_Detects_Overlapping_Range()
    {
        var (context, service, habitacionId) = await SeedBasicAsync();

        var reserva = new Reserva
        {
            Id = 1,
            UsuarioId = "user",
            SitioId = 1,
            AlojamientoId = 1,
            EstadoReservaId = 2,
            FechaInicio = new DateTime(2026, 6, 10),
            FechaFin = new DateTime(2026, 6, 12),
            NumeroPersonas = 2,
            NumeroHabitaciones = 1,
            TarifaTotal = 100000,
            FechaReserva = DateTime.UtcNow
        };

        context.Reservas.Add(reserva);
        context.ReservasHabitaciones.Add(new ReservaHabitacion
        {
            Id = 1,
            ReservaId = reserva.Id,
            Reserva = reserva,
            HabitacionId = habitacionId
        });
        await context.SaveChangesAsync();

        var disponible = await service.HabitacionesDisponiblesAsync(
            habitacionesIds: new[] { habitacionId },
            fechaInicio: new DateTime(2026, 6, 11),
            fechaFin: new DateTime(2026, 6, 13));

        Assert.False(disponible);
    }

    [Fact]
    public async Task ObtenerHabitacionesDisponiblesPorPersonasAsync_Filters_By_Capacity()
    {
        var (context, service, habitacionId) = await SeedBasicAsync();

        var result = await service.ObtenerHabitacionesDisponiblesPorPersonasAsync(
            fechaInicio: new DateTime(2026, 6, 20),
            fechaFin: new DateTime(2026, 6, 22),
            numeroPersonas: 5);

        Assert.DoesNotContain(result, h => h.Id == habitacionId);
    }

    [Fact]
    public async Task CalcularTarifaTotalAsync_Fallback_Computes_Based_On_Nochen_Habitaciones_Y_Adicionales()
    {
        await using var context = await CreateSqliteContextAsync();

        context.Tarifas.Add(new Tarifa
        {
            SitioId = 1,
            AlojamientoId = null,
            TipoTemporadaId = 1,
            NumeroPersonasMin = 1,
            NumeroPersonasMax = 6,
            NumeroHabitaciones = 2,
            PrecioBase = 70000,
            PrecioPersonaAdicional = 16000,
            EsTarifaEspecial = false,
            Descripcion = "Test",
            Activo = true
        });
        await context.SaveChangesAsync();

        var service = new ReservaService(context, NullLogger<ReservaService>.Instance);

        var total = await service.CalcularTarifaTotalAsync(
            sitioId: 1,
            numeroHabitaciones: 2,
            numeroPersonas: 6,
            alojamientoId: null,
            tipoTemporadaId: 1,
            fechaInicio: new DateTime(2026, 7, 1),
            fechaFin: new DateTime(2026, 7, 4));

        Assert.Equal(420000m, total);
    }

    [Fact]
    public async Task CalcularTarifaTotalAsync_Fallback_Returns_Zero_When_No_Tarifa_Matches()
    {
        await using var context = await CreateSqliteContextAsync();
        var service = new ReservaService(context, NullLogger<ReservaService>.Instance);

        var total = await service.CalcularTarifaTotalAsync(
            sitioId: 999,
            numeroHabitaciones: 1,
            numeroPersonas: 2,
            alojamientoId: null,
            tipoTemporadaId: 1,
            fechaInicio: new DateTime(2026, 7, 1),
            fechaFin: new DateTime(2026, 7, 2));

        Assert.Equal(0m, total);
    }

    [Fact]
    public async Task CalcularTarifaTotalAsync_Fallback_When_NumeroPersonas_Exceeds_Max_Uses_Highest_Band_And_Additionals()
    {
        await using var context = await CreateSqliteContextAsync();

        const int sitioId = 1000;
        const int tipoTemporadaId = 1000;

        context.Sitios.Add(new Sitio { Id = sitioId, Nombre = "S", TipoSitioId = 1, CapacidadTotal = 99, Activo = true });
        context.TiposTemporada.Add(new TipoTemporada { Id = tipoTemporadaId, Nombre = "Baja" });

        context.Tarifas.AddRange(
            new Tarifa
            {
                SitioId = sitioId,
                AlojamientoId = null,
                TipoTemporadaId = tipoTemporadaId,
                NumeroPersonasMin = 1,
                NumeroPersonasMax = 4,
                NumeroHabitaciones = 1,
                PrecioBase = 70000,
                PrecioPersonaAdicional = 16000,
                EsTarifaEspecial = false,
                Descripcion = "1-4",
                Activo = true
            },
            new Tarifa
            {
                SitioId = sitioId,
                AlojamientoId = null,
                TipoTemporadaId = tipoTemporadaId,
                NumeroPersonasMin = 5,
                NumeroPersonasMax = 6,
                NumeroHabitaciones = 1,
                PrecioBase = 90000,
                PrecioPersonaAdicional = 16000,
                EsTarifaEspecial = false,
                Descripcion = "5-6",
                Activo = true
            }
        );

        await context.SaveChangesAsync();

        var service = new ReservaService(context, NullLogger<ReservaService>.Instance);

        var total = await service.CalcularTarifaTotalAsync(
            sitioId: sitioId,
            numeroHabitaciones: 1,
            numeroPersonas: 8,
            alojamientoId: null,
            tipoTemporadaId: tipoTemporadaId,
            fechaInicio: new DateTime(2026, 7, 1),
            fechaFin: new DateTime(2026, 7, 3)); // 2 noches

        // Usa banda 5-6 y cobra 2 personas adicionales: (90000 + 2*16000) * 2 noches * 1 hab
        Assert.Equal(244000m, total);
    }

    [Fact]
    public async Task ObtenerTarifasAsync_Fallback_Filters_By_Range_And_Prefers_NonEspecial_First()
    {
        await using var context = CreateInMemoryContext();

        var sitio = new Sitio { Id = 100, Nombre = "S", TipoSitioId = 1, CapacidadTotal = 10, Activo = true };
        var tipoTemporada = new TipoTemporada { Id = 100, Nombre = "Baja" };
        context.Sitios.Add(sitio);
        context.TiposTemporada.Add(tipoTemporada);

        context.Tarifas.AddRange(
            new Tarifa
            {
                SitioId = 100,
                Sitio = sitio,
                AlojamientoId = null,
                TipoTemporadaId = 100,
                TipoTemporada = tipoTemporada,
                NumeroPersonasMin = 1,
                NumeroPersonasMax = 4,
                NumeroHabitaciones = 1,
                PrecioBase = 70000,
                PrecioPersonaAdicional = 16000,
                EsTarifaEspecial = false,
                Descripcion = "Base",
                Activo = true
            },
            new Tarifa
            {
                SitioId = 100,
                Sitio = sitio,
                AlojamientoId = null,
                TipoTemporadaId = 100,
                TipoTemporada = tipoTemporada,
                NumeroPersonasMin = 1,
                NumeroPersonasMax = 4,
                NumeroHabitaciones = 1,
                PrecioBase = 27000,
                PrecioPersonaAdicional = 11000,
                EsTarifaEspecial = true,
                Descripcion = "Especial",
                Activo = true
            },
            new Tarifa
            {
                SitioId = 100,
                Sitio = sitio,
                AlojamientoId = null,
                TipoTemporadaId = 100,
                TipoTemporada = tipoTemporada,
                NumeroPersonasMin = 5,
                NumeroPersonasMax = 6,
                NumeroHabitaciones = 1,
                PrecioBase = 90000,
                PrecioPersonaAdicional = 16000,
                EsTarifaEspecial = false,
                Descripcion = "FueraDeRango",
                Activo = true
            }
        );
        await context.SaveChangesAsync();

        var service = new ReservaService(context, NullLogger<ReservaService>.Instance);

        var tarifas = await service.ObtenerTarifasAsync(
            sitioId: 100,
            tipoTemporadaId: 100,
            numeroPersonas: 2,
            alojamientoId: null);

        Assert.Contains(tarifas, t => !t.EsTarifaEspecial);
        Assert.False(tarifas[0].EsTarifaEspecial);
        Assert.Contains(tarifas, t => t.Descripcion == "Especial");
        Assert.DoesNotContain(tarifas, t => t.Descripcion == "FueraDeRango");
    }

    [Fact]
    public async Task HabitacionesDisponiblesAsync_Returns_False_When_Inactive_Room_In_List()
    {
        var (context, service, habitacionId) = await SeedBasicAsync();

        var room = await context.Habitaciones.FirstAsync(h => h.Id == habitacionId);
        room.Activo = false;
        await context.SaveChangesAsync();

        var disponible = await service.HabitacionesDisponiblesAsync(
            habitacionesIds: new[] { habitacionId },
            fechaInicio: new DateTime(2026, 6, 12),
            fechaFin: new DateTime(2026, 6, 14));

        Assert.False(disponible);
    }
}
