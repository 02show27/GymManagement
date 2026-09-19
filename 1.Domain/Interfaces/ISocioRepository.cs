using GymManagement.Domain.Entities;

namespace GymManagement.Domain.Interfaces;

public interface ISocioRepository
{
    Task<Socio> AgregarAsync(Socio socio);
    Task<Socio?> ObtenerPorIdAsync(Guid id);
    Task<Socio?> ObtenerPorEmailAsync(string email);
}