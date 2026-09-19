using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly ApplicationDbContext _context;

    public ReservaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Reserva?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Reservas
            .Include(r => r.HorarioClase)
            .ThenInclude(h => h.Clase)
            .Include(r => r.Socio)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Reserva>> ObtenerReservasPorSocioAsync(Guid socioId)
    {
        return await _context.Reservas
            .Include(r => r.HorarioClase)
            .ThenInclude(h => h.Clase)
            .Where(r => r.SocioId == socioId && r.Estado != EstadoReserva.CANCELADA)
            .OrderByDescending(r => r.HorarioClase.FechaHoraInicio)
            .ToListAsync();
    }

    public async Task<List<Reserva>> ObtenerReservasPorHorarioAsync(Guid horarioClaseId)
    {
        return await _context.Reservas
            .Where(r => r.HorarioClaseId == horarioClaseId && r.Estado != EstadoReserva.CANCELADA)
            .OrderBy(r => r.FechaRegistro)
            .ToListAsync();
    }

    public async Task<int> ObtenerCantidadInscritosConfirmadosAsync(Guid horarioClaseId)
    {
        return await _context.Reservas
            .CountAsync(r => r.HorarioClaseId == horarioClaseId && r.Estado == EstadoReserva.CONFIRMADA);
    }

    public async Task<bool> ExisteSolapamientoHorarioAsync(Guid socioId, DateTime fechaHoraInicio, int duracionMinutos)
    {
        var fechaFin = fechaHoraInicio.AddMinutes(duracionMinutos);

        return await _context.Reservas
            .Include(r => r.HorarioClase)
            .ThenInclude(h => h.Clase)
            .Where(r => r.SocioId == socioId && r.Estado == EstadoReserva.CONFIRMADA)
            .AnyAsync(r => r.HorarioClase.FechaHoraInicio < fechaFin &&
                           r.HorarioClase.FechaHoraInicio.AddMinutes(r.HorarioClase.Clase.DuracionMinutos) > fechaHoraInicio);
    }

    public async Task AgregarAsync(Reserva reserva)
    {
        await _context.Reservas.AddAsync(reserva);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Reserva reserva)
    {
        _context.Reservas.Update(reserva);
        await _context.SaveChangesAsync();
    }
}