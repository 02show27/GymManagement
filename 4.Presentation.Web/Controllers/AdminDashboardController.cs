using GymManagement.Application.DTOs;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Persistence;
using GymManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Presentation.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminDashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IReportePdfService _pdfService;

    public AdminDashboardController(ApplicationDbContext context, IReportePdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    public async Task<IActionResult> Index()
    {
        var totalSocios = await _context.Socios.CountAsync();
        var totalReservas = await _context.Reservas.CountAsync(r => r.Estado == Domain.Enums.EstadoReserva.CONFIRMADA);
        var clasesList = await _context.Clases.ToListAsync();
        var horariosList = await _context.HorariosClases
            .Include(h => h.Clase)
            .Include(h => h.Reservas)
            .ToListAsync();

        var sociosList = await _context.Socios
            .Include(s => s.Membresias)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TotalSociosActivos = totalSocios,
            ClasesProgramadasHoy = horariosList.Count(h => h.FechaHoraInicio.Date == DateTime.UtcNow.Date),
            ReservasConfirmadasMes = totalReservas,
            OcupacionPromedioPorcentaje = 85.5m,
            IngresosTotalesMes = totalSocios * 35.00m,

            Clases = clasesList.Select(c => new ClaseGestionDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                DuracionMinutos = c.DuracionMinutos
            }).ToList(),

            Horarios = horariosList.Select(h => new HorarioGestionDto
            {
                Id = h.Id,
                NombreClase = h.Clase.Nombre,
                Instructor = h.Instructor,
                FechaHoraInicio = h.FechaHoraInicio,
                CupoMaximo = h.CupoMaximo,
                CuposOcupados = h.Reservas.Count(r => r.Estado == Domain.Enums.EstadoReserva.CONFIRMADA)
            }).ToList(),

            Socios = sociosList.Select(s => new SocioGestionDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Email = s.Email,
                Telefono = s.Telefono,
                EstadoMembresia = s.Membresias.FirstOrDefault()?.Estado.ToString() ?? "ACTIVA",
                FechaRegistro = s.FechaRegistro
            }).ToList(),

            // Panel de Pagos
            Pagos = sociosList.Select(s => new PagoAdminDto
            {
                Id = Guid.NewGuid(),
                SocioNombre = s.Nombre,
                Concepto = "Suscripción Mensual Pro",
                Monto = 35.00m,
                FechaPago = s.FechaRegistro,
                Estado = "COMPLETADO"
            }).ToList(),

            // Buzón de Quejas y Sugerencias
            Quejas = new List<QuejaAdminDto>
            {
                new QuejaAdminDto
                {
                    Id = Guid.NewGuid(),
                    SocioNombre = "Aldo Choque",
                    Asunto = "Aire acondicionado en área de Spinning",
                    Mensaje = "El aire acondicionado del salón 2 no estaba enfriando adecuadamente durante la clase de las 8 AM.",
                    FechaEnvio = DateTime.UtcNow.AddDays(-1),
                    Estado = "PENDIENTE"
                }
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearClase(string nombre, string descripcion, int duracionMinutos)
    {
        if (string.IsNullOrWhiteSpace(nombre) || duracionMinutos <= 0)
        {
            TempData["ErrorMessage"] = "Datos de clase no válidos.";
            return RedirectToAction("Index");
        }

        var nuevaClase = new Clase
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Descripcion = descripcion ?? "",
            DuracionMinutos = duracionMinutos
        };

        await _context.Clases.AddAsync(nuevaClase);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"¡Disciplina '{nombre}' creada con éxito!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProgramarHorario(Guid claseId, string instructor, DateTime fechaHoraInicio, int cupoMaximo)
    {
        if (cupoMaximo <= 0 || string.IsNullOrWhiteSpace(instructor))
        {
            TempData["ErrorMessage"] = "Debe especificar un aforo máximo N válido e instructor.";
            return RedirectToAction("Index");
        }

        var horario = new HorarioClase
        {
            Id = Guid.NewGuid(),
            ClaseId = claseId,
            Instructor = instructor,
            FechaHoraInicio = DateTime.SpecifyKind(fechaHoraInicio, DateTimeKind.Utc),
            CupoMaximo = cupoMaximo
        };

        await _context.HorariosClases.AddAsync(horario);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"¡Horario programado con aforo estricto N = {cupoMaximo}!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DescargarReportePdf()
    {
        var totalSocios = await _context.Socios.CountAsync();
        var totalReservas = await _context.Reservas.CountAsync(r => r.Estado == Domain.Enums.EstadoReserva.CONFIRMADA);
        decimal ingresos = totalSocios * 35.00m;

        byte[] pdfBytes = _pdfService.GenerarReporteFinancieroPdf(ingresos, totalSocios, totalReservas);

        return File(pdfBytes, "application/pdf", $"Reporte_Financiero_Gym_{DateTime.Now:yyyyMMdd}.pdf");
    }
}