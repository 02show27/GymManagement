using GymManagement.Domain.Enums;

namespace GymManagement.Domain.Entities;

public class Membresia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SocioId { get; set; }
    public string TipoPlan { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoMembresia Estado { get; set; } = EstadoMembresia.ACTIVA;

    public Socio Socio { get; set; } = null!;
}