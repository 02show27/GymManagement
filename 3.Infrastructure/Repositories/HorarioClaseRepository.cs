using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Repositories;

public class HorarioClaseRepository : IHorarioClaseRepository
{
    private readonly ApplicationDbContext _context;

    public HorarioClaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HorarioClase>> ObtenerHorariosDisponiblesAsync()
    {
        return await _context.HorariosClases
            .Include(h => h.Clase)
            .Include(h => h.Reservas)
            .Where(h => h.FechaHoraInicio >= DateTime.UtcNow)
            .OrderBy(h => h.FechaHoraInicio)
            .ToListAsync();
    }

    public async Task<HorarioClase?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.HorariosClases
            .Include(h => h.Clase)
            .Include(h => h.Reservas)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
}