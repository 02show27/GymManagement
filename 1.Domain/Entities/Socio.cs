namespace GymManagement.Domain.Entities;

public class Socio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}