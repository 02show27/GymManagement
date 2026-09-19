using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Repositories;

public class SocioRepository : ISocioRepository
{
    private readonly ApplicationDbContext _context;

    public SocioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Socio> AgregarAsync(Socio socio)
    {
        await _context.Socios.AddAsync(socio);
        await _context.SaveChangesAsync();
        return socio;
    }

    public async Task<Socio?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Socios
            .Include(s => s.Membresias)
            .Include(s => s.Reservas)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Socio?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Socios
            .FirstOrDefaultAsync(s => s.Email == email);
    }
}