namespace GymManagement.Application.DTOs;

public class ClienteDashboardViewModel
{
    public string NombreSocio { get; set; } = string.Empty;
    public string EmailSocio { get; set; } = string.Empty;
    public string EstadoMembresia { get; set; } = "SIN_MEMBRESIA";
    public string TipoPlan { get; set; } = "Pase Ocasional";
    public decimal PrecioPlan { get; set; } = 0;
    public DateTime? FechaInicioMembresia { get; set; }
    public DateTime? FechaVencimientoMembresia { get; set; }
    public int ClasesAsistidasMes { get; set; }
    public int ReservasActivasCount { get; set; }

    public List<ClaseDisponibleDto> ClasesDisponibles { get; set; } = new();
    public List<ReservaSocioDto> MisReservas { get; set; } = new();
    public List<PagoSocioDto> HistorialPagos { get; set; } = new();
}

public class PagoSocioDto
{
    public Guid PagoId { get; set; }
    public string Concepto { get; set; } = string.Empty; // "Plan Mensual Pro", "Pase Suelto Spinning"
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string MetodoPago { get; set; } = "Tarjeta de Crédito/Débito";
    public string Estado { get; set; } = "COMPLETADO";
}

public class ClaseDisponibleDto
{
    public Guid HorarioClaseId { get; set; }
    public string NombreDisciplina { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public int CupoMaximo { get; set; }
    public int CuposOcupados { get; set; }
    public int CuposDisponibles => Math.Max(0, CupoMaximo - CuposOcupados);
    public bool EstaLleno => CuposDisponibles == 0;
}

public class ReservaSocioDto
{
    public Guid ReservaId { get; set; }
    public string NombreClase { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public string EstadoReserva { get; set; } = string.Empty;
    public int? PosicionListaEspera { get; set; }
    public string QrCodeBase64 { get; set; } = string.Empty;
}