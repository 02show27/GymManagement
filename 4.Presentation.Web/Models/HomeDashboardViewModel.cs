namespace GymManagement.Presentation.Web.Models;

public class HomeDashboardViewModel
{
    public int TotalSociosActivos { get; set; }
    public int ClasesProgramadasHoy { get; set; }
    public int ReservasConfirmadasMes { get; set; }
    public decimal OcupacionPromedioPorcentaje { get; set; }

    public List<ClaseResumenDto> ProximasClases { get; set; } = new();
}

public class ClaseResumenDto
{
    public Guid HorarioClaseId { get; set; }
    public string NombreClase { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public int CupoMaximo { get; set; }
    public int InscritosConfirmados { get; set; }
    public int EnListaEspera { get; set; }
}