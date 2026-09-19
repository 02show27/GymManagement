using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets para las entidades del negocio
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Membresia> Membresias => Set<Membresia>();
    public DbSet<Clase> Clases => Set<Clase>();
    public DbSet<HorarioClase> HorariosClases => Set<HorarioClase>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Mapeo explícito de tablas
        builder.Entity<Socio>().ToTable("Socios");
        builder.Entity<Membresia>().ToTable("Membresias");
        builder.Entity<Clase>().ToTable("Clases");
        builder.Entity<HorarioClase>().ToTable("HorariosClases");
        builder.Entity<Reserva>().ToTable("Reservas");
        builder.Entity<Asistencia>().ToTable("Asistencias");
    }
}