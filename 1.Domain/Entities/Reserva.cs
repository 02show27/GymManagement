using GymManagement.Domain.Enums;

namespace GymManagement.Domain.Entities;

public class Reserva
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HorarioClaseId { get; set; }
    public Guid SocioId { get; set; }
    public EstadoReserva Estado { get; set; } = EstadoReserva.CONFIRMADA;
    public int? PosicionListaEspera { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public HorarioClase HorarioClase { get; set; } = null!;
    public Socio Socio { get; set; } = null!;
    public Asistencia? Asistencia { get; set; }
}