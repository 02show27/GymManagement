using GymManagement.Domain.Entities;

namespace GymManagement.Domain.Interfaces;

public interface IHorarioClaseRepository
{
    Task<List<HorarioClase>> ObtenerHorariosDisponiblesAsync();
    Task<HorarioClase?> ObtenerPorIdAsync(Guid id);
}