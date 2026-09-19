namespace GymManagement.Application.DTOs;

public class AdminDashboardViewModel
{
    // U8: Métricas Ejecutivas
    public int TotalSociosActivos { get; set; }
    public int ClasesProgramadasHoy { get; set; }
    public int ReservasConfirmadasMes { get; set; }
    public decimal OcupacionPromedioPorcentaje { get; set; }
    public decimal IngresosTotalesMes { get; set; }

    // U6: Gestión de Clases y Horarios
// Listados Principales
    public List<ClaseGestionDto> Clases { get; set; } = new();
    public List<HorarioGestionDto> Horarios { get; set; } = new();
    public List<SocioGestionDto> Socios { get; set; } = new();
    public List<PagoAdminDto> Pagos { get; set; } = new();
    public List<QuejaAdminDto> Quejas { get; set; } = new();
}

public class ClaseGestionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
}

public class HorarioGestionDto
{
    public Guid Id { get; set; }
    public string NombreClase { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public int CupoMaximo { get; set; }
    public int CuposOcupados { get; set; }
    public decimal PorcentajeOcupacion => CupoMaximo > 0 ? (decimal)CuposOcupados / CupoMaximo * 100 : 0;
}

public class SocioGestionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string EstadoMembresia { get; set; } = "SIN_MEMBRESIA";
    public DateTime FechaRegistro { get; set; }
}
public class PagoAdminDto
{
    public Guid Id { get; set; }
    public string SocioNombre { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string Estado { get; set; } = "COMPLETADO";
}
public class QuejaAdminDto
{
    public Guid Id { get; set; }
    public string SocioNombre { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaEnvio { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
}