namespace GymManagement.Domain.Entities;

public class Clase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }

    public ICollection<HorarioClase> Horarios { get; set; } = new List<HorarioClase>();
}