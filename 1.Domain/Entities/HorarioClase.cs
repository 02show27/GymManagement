namespace GymManagement.Domain.Entities;

public class HorarioClase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClaseId { get; set; }
    public string Instructor { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public int CupoMaximo { get; set; }

    public Clase Clase { get; set; } = null!;
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}