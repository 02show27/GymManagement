namespace GymManagement.Domain.Entities;

public class Asistencia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ReservaId { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;

    public Reserva Reserva { get; set; } = null!;
}