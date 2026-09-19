using GymManagement.Domain.Entities;

namespace GymManagement.Domain.Interfaces;

public interface IReservaRepository
{
    Task<Reserva?> ObtenerPorIdAsync(Guid id);
    Task<List<Reserva>> ObtenerReservasPorSocioAsync(Guid socioId);
    Task<List<Reserva>> ObtenerReservasPorHorarioAsync(Guid horarioClaseId);
    Task<int> ObtenerCantidadInscritosConfirmadosAsync(Guid horarioClaseId);
    Task<bool> ExisteSolapamientoHorarioAsync(Guid socioId, DateTime fechaHoraInicio, int duracionMinutos);
    Task AgregarAsync(Reserva reserva);
    Task ActualizarAsync(Reserva reserva);
}