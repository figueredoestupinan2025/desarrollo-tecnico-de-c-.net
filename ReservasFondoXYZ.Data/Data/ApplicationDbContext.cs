using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Data.Dtos;
using ReservasFondoXYZ.Data.Models;

namespace ReservasFondoXYZ.Data.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TipoSitio> TiposSitio { get; set; }
    public DbSet<Sitio> Sitios { get; set; }
    public DbSet<TipoAlojamiento> TiposAlojamiento { get; set; }
    public DbSet<Alojamiento> Alojamientos { get; set; }
    public DbSet<Habitacion> Habitaciones { get; set; }
    public DbSet<TipoTemporada> TiposTemporada { get; set; }
    public DbSet<Temporada> Temporadas { get; set; }
    public DbSet<Tarifa> Tarifas { get; set; }
    public DbSet<EstadoReserva> EstadosReserva { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<ReservaHabitacion> ReservasHabitaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HabitacionDisponibleDto>().HasNoKey();
        modelBuilder.Entity<TarifaDto>().HasNoKey();

        modelBuilder.Entity<TipoSitio>(entity =>
        {
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasData(
                new TipoSitio { Id = 1, Nombre = "Sede Recreativa" },
                new TipoSitio { Id = 2, Nombre = "Apartamento" }
            );
        });

        modelBuilder.Entity<TipoAlojamiento>(entity =>
        {
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasData(
                new TipoAlojamiento { Id = 1, Nombre = "Habitación" },
                new TipoAlojamiento { Id = 2, Nombre = "Alojamiento" },
                new TipoAlojamiento { Id = 3, Nombre = "Cabaña" },
                new TipoAlojamiento { Id = 4, Nombre = "Apartamento" }
            );
        });

        modelBuilder.Entity<TipoTemporada>(entity =>
        {
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasData(
                new TipoTemporada { Id = 1, Nombre = "Baja Temporada" },
                new TipoTemporada { Id = 2, Nombre = "Alta Temporada" },
                new TipoTemporada { Id = 3, Nombre = "Tarifa Especial" }
            );
        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasData(
                new EstadoReserva { Id = 1, Nombre = "Pendiente" },
                new EstadoReserva { Id = 2, Nombre = "Confirmada" },
                new EstadoReserva { Id = 3, Nombre = "Cancelada" },
                new EstadoReserva { Id = 4, Nombre = "Completada" }
            );
        });

        modelBuilder.Entity<Sitio>(entity =>
        {
            entity.HasIndex(e => e.TipoSitioId);
            entity.HasData(
                new Sitio { Id = 1, Nombre = "Villeta", TipoSitioId = 1, Ubicacion = "Villeta, Cundinamarca", Descripcion = "Sede recreativa con 8 habitaciones, cada una con cama doble y camarote, baño, nevera, televisor y terraza cubierta.", CapacidadTotal = 32, Activo = true },
                new Sitio { Id = 2, Nombre = "El Placer - Fusagasugá", TipoSitioId = 1, Ubicacion = "Fusagasugá, Cundinamarca", Descripcion = "Sede recreativa con alojamientos y bloque de cabañas.", CapacidadTotal = 34, Activo = true },
                new Sitio { Id = 3, Nombre = "Gonzalo Morante - Chinchiná", TipoSitioId = 1, Ubicacion = "Chinchiná, Caldas", Descripcion = "Sede recreativa con alojamientos y bloque de cabañas.", CapacidadTotal = 30, Activo = true },
                new Sitio { Id = 4, Nombre = "Tablones - Palmira", TipoSitioId = 1, Ubicacion = "Palmira, Valle del Cauca", Descripcion = "Sede recreativa con alojamientos.", CapacidadTotal = 24, Activo = true },
                new Sitio { Id = 5, Nombre = "Manguruma - Santa Fe de Antioquia", TipoSitioId = 1, Ubicacion = "Santa Fe de Antioquia, Antioquia", Descripcion = "Sede recreativa con alojamientos y bloque nuevo.", CapacidadTotal = 46, Activo = true },
                new Sitio { Id = 6, Nombre = "Federman - Bogotá", TipoSitioId = 1, Ubicacion = "Bogotá, D.C.", Descripcion = "Sede recreativa con servicios completos y 4 habitaciones.", CapacidadTotal = 8, Activo = true },
                new Sitio { Id = 7, Nombre = "Suramericana - Medellín", TipoSitioId = 2, Ubicacion = "Medellín, Antioquia", Descripcion = "Apartamento con 5 habitaciones.", CapacidadTotal = 10, Activo = true },
                new Sitio { Id = 8, Nombre = "El Rodadero - Santa Marta", TipoSitioId = 2, Ubicacion = "Santa Marta, Magdalena", Descripcion = "Apartamentos turísticos.", CapacidadTotal = 20, Activo = true }
            );
        });

        modelBuilder.Entity<Alojamiento>(entity =>
        {
            entity.HasIndex(e => e.SitioId);
            entity.HasIndex(e => e.TipoAlojamientoId);
            entity.HasData(
                new Alojamiento { Id = 1, SitioId = 1, TipoAlojamientoId = 1, Nombre = "Habitaciones Villeta", Descripcion = "Ocho habitaciones cada una con una alcoba que tiene una cama doble y un camarote, baño, nevera, televisor y terraza cubierta.", CapacidadMaxima = 32, NumeroHabitaciones = 8, Activo = true },
                new Alojamiento { Id = 2, SitioId = 7, TipoAlojamientoId = 4, Nombre = "Apartamento Suramericana", Descripcion = "Apartamento con 5 habitaciones", CapacidadMaxima = 10, NumeroHabitaciones = 5, Activo = true },
                new Alojamiento { Id = 3, SitioId = 8, TipoAlojamientoId = 4, Nombre = "Apartamento 202", Descripcion = "Tiene sala comedor, cocina, 2 baños, tres habitaciones y un sitio para parqueo. Capacidad máxima: 8 personas.", CapacidadMaxima = 8, NumeroHabitaciones = 3, Activo = true },
                new Alojamiento { Id = 4, SitioId = 8, TipoAlojamientoId = 4, Nombre = "Apartamento 301", Descripcion = "Tiene Sala comedor, cocina, 1 baño, dos habitaciones y un sitio para parqueo. Capacidad máxima: 6 personas.", CapacidadMaxima = 6, NumeroHabitaciones = 2, Activo = true },
                new Alojamiento { Id = 5, SitioId = 8, TipoAlojamientoId = 4, Nombre = "Apartamento 401", Descripcion = "Tiene Sala comedor, cocina, 1 baño, dos habitaciones y un sitio para parqueo. Capacidad máxima: 6 personas.", CapacidadMaxima = 6, NumeroHabitaciones = 2, Activo = true }
            );
        });

        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.HasIndex(e => e.AlojamientoId);
            entity.HasData(
                new Habitacion { Id = 1, AlojamientoId = 1, Numero = "101", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 2, AlojamientoId = 1, Numero = "102", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 3, AlojamientoId = 1, Numero = "103", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 4, AlojamientoId = 1, Numero = "104", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 5, AlojamientoId = 1, Numero = "105", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 6, AlojamientoId = 1, Numero = "106", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 7, AlojamientoId = 1, Numero = "107", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 8, AlojamientoId = 1, Numero = "108", Descripcion = "Habitación con cama doble y camarote", CapacidadMaxima = 4, Activo = true },
                new Habitacion { Id = 9, AlojamientoId = 2, Numero = "1", Descripcion = "Habitación 1 con 2 camas sencillas y baño privado", CapacidadMaxima = 2, Activo = true },
                new Habitacion { Id = 10, AlojamientoId = 2, Numero = "2", Descripcion = "Habitación 2 con 2 camas sencillas", CapacidadMaxima = 2, Activo = true },
                new Habitacion { Id = 11, AlojamientoId = 2, Numero = "3", Descripcion = "Habitación 3 con 2 camas sencillas", CapacidadMaxima = 2, Activo = true },
                new Habitacion { Id = 12, AlojamientoId = 2, Numero = "4", Descripcion = "Habitación 4 con 2 camas sencillas", CapacidadMaxima = 2, Activo = true },
                new Habitacion { Id = 13, AlojamientoId = 2, Numero = "5", Descripcion = "Habitación 5 con 1 cama sencilla y baño privado", CapacidadMaxima = 1, Activo = true }
            );
        });

        modelBuilder.Entity<Temporada>(entity =>
        {
            entity.HasIndex(e => e.TipoTemporadaId);
            entity.HasData(
                new Temporada { Id = 1, TipoTemporadaId = 1, Nombre = "Baja Temporada 2026", FechaInicio = new DateTime(2026, 1, 1), FechaFin = new DateTime(2026, 6, 15), Descripcion = "Temporada baja", Activo = true },
                new Temporada { Id = 2, TipoTemporadaId = 2, Nombre = "Alta Temporada 2026", FechaInicio = new DateTime(2026, 6, 16), FechaFin = new DateTime(2026, 8, 31), Descripcion = "Temporada alta de verano", Activo = true },
                new Temporada { Id = 3, TipoTemporadaId = 2, Nombre = "Alta Temporada Navidad 2026", FechaInicio = new DateTime(2026, 12, 15), FechaFin = new DateTime(2027, 1, 15), Descripcion = "Temporada alta de Navidad", Activo = true },
                new Temporada { Id = 4, TipoTemporadaId = 3, Nombre = "Tarifa Especial", FechaInicio = new DateTime(2026, 1, 1), FechaFin = new DateTime(2026, 12, 31), Descripcion = "Tarifa especial para días entre semana", Activo = true }
            );
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.HasIndex(e => e.SitioId);
            entity.HasIndex(e => e.AlojamientoId);
            entity.HasIndex(e => e.TipoTemporadaId);
            entity.HasData(
                new Tarifa { Id = 1, SitioId = 1, AlojamientoId = null, TipoTemporadaId = 1, NumeroPersonasMin = 1, NumeroPersonasMax = 4, NumeroHabitaciones = 1, PrecioBase = 70000.00m, PrecioPersonaAdicional = 16000.00m, EsTarifaEspecial = false, Descripcion = "Alojamiento 1 habitación/noche 1-4 personas", Activo = true },
                new Tarifa { Id = 2, SitioId = 1, AlojamientoId = null, TipoTemporadaId = 1, NumeroPersonasMin = 1, NumeroPersonasMax = 4, NumeroHabitaciones = 2, PrecioBase = 90000.00m, PrecioPersonaAdicional = 16000.00m, EsTarifaEspecial = false, Descripcion = "Alojamiento 2 habitaciones/noche 1-4 personas", Activo = true },
                new Tarifa { Id = 3, SitioId = 1, AlojamientoId = null, TipoTemporadaId = 3, NumeroPersonasMin = 1, NumeroPersonasMax = 4, NumeroHabitaciones = 1, PrecioBase = 27000.00m, PrecioPersonaAdicional = 11000.00m, EsTarifaEspecial = true, Descripcion = "Tarifa especial 1 habitación 1-4 personas", Activo = true },
                new Tarifa { Id = 4, SitioId = 1, AlojamientoId = null, TipoTemporadaId = 3, NumeroPersonasMin = 1, NumeroPersonasMax = 4, NumeroHabitaciones = 2, PrecioBase = 37000.00m, PrecioPersonaAdicional = 11000.00m, EsTarifaEspecial = true, Descripcion = "Tarifa especial 2 habitaciones 1-4 personas", Activo = true },
                new Tarifa { Id = 5, SitioId = 7, AlojamientoId = 2, TipoTemporadaId = 1, NumeroPersonasMin = 1, NumeroPersonasMax = 1, NumeroHabitaciones = 1, PrecioBase = 63000.00m, PrecioPersonaAdicional = null, EsTarifaEspecial = false, Descripcion = "Habitación/noche una persona", Activo = true },
                new Tarifa { Id = 6, SitioId = 7, AlojamientoId = 2, TipoTemporadaId = 1, NumeroPersonasMin = 2, NumeroPersonasMax = 2, NumeroHabitaciones = 1, PrecioBase = 75000.00m, PrecioPersonaAdicional = null, EsTarifaEspecial = false, Descripcion = "Habitación/noche dos personas", Activo = true },
                new Tarifa { Id = 7, SitioId = 8, AlojamientoId = null, TipoTemporadaId = 1, NumeroPersonasMin = 1, NumeroPersonasMax = 6, NumeroHabitaciones = 2, PrecioBase = 89000.00m, PrecioPersonaAdicional = null, EsTarifaEspecial = false, Descripcion = "Baja temporada - Apto 301-401 hasta 6 personas", Activo = true },
                new Tarifa { Id = 8, SitioId = 8, AlojamientoId = null, TipoTemporadaId = 1, NumeroPersonasMin = 1, NumeroPersonasMax = 8, NumeroHabitaciones = 3, PrecioBase = 103000.00m, PrecioPersonaAdicional = null, EsTarifaEspecial = false, Descripcion = "Baja temporada - Apto 202 hasta 8 personas", Activo = true },
                new Tarifa { Id = 9, SitioId = 8, AlojamientoId = null, TipoTemporadaId = 2, NumeroPersonasMin = 1, NumeroPersonasMax = 6, NumeroHabitaciones = 2, PrecioBase = 124000.00m, PrecioPersonaAdicional = null, EsTarifaEspecial = false, Descripcion = "Alta temporada - Apto 301-401 hasta 6 personas", Activo = true },
                new Tarifa { Id = 10, SitioId = 8, AlojamientoId = null, TipoTemporadaId = 2, NumeroPersonasMin = 1, NumeroPersonasMax = 8, NumeroHabitaciones = 3, PrecioBase = 143000.00m, PrecioPersonaAdicional = null, EsTarifaEspecial = false, Descripcion = "Alta temporada - Apto 202 hasta 8 personas", Activo = true }
            );
        });

        modelBuilder.Entity<Sitio>(entity =>
        {
            entity.HasIndex(e => e.TipoSitioId);
        });

        modelBuilder.Entity<Alojamiento>(entity =>
        {
            entity.HasIndex(e => e.SitioId);
            entity.HasIndex(e => e.TipoAlojamientoId);
        });

        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.HasIndex(e => e.AlojamientoId);
        });

        modelBuilder.Entity<Temporada>(entity =>
        {
            entity.HasIndex(e => e.TipoTemporadaId);
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.HasIndex(e => e.SitioId);
            entity.HasIndex(e => e.AlojamientoId);
            entity.HasIndex(e => e.TipoTemporadaId);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.SitioId);
            entity.HasIndex(e => e.AlojamientoId);
            entity.HasIndex(e => e.EstadoReservaId);
            entity.HasIndex(e => new { e.FechaInicio, e.FechaFin });
        });

        modelBuilder.Entity<ReservaHabitacion>(entity =>
        {
            entity.HasIndex(e => e.ReservaId);
            entity.HasIndex(e => e.HabitacionId);
        });
    }
}
