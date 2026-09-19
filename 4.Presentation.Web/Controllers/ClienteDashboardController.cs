using System.Security.Claims;
using GymManagement.Application.DTOs;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Presentation.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class ClienteDashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISocioRepository _socioRepository;
    private readonly IReservaRepository _reservaRepository;
    private readonly IHorarioClaseRepository _horarioClaseRepository;
    private readonly IQrGeneratorService _qrService;

    public ClienteDashboardController(
        UserManager<ApplicationUser> userManager,
        ISocioRepository socioRepository,
        IReservaRepository reservaRepository,
        IHorarioClaseRepository horarioClaseRepository,
        IQrGeneratorService qrService)
    {
        _userManager = userManager;
        _socioRepository = socioRepository;
        _reservaRepository = reservaRepository;
        _horarioClaseRepository = horarioClaseRepository;
        _qrService = qrService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user?.SocioId == null)
        {
            TempData["ErrorMessage"] = "No se encontró el perfil de socio vinculado.";
            return RedirectToAction("Index", "Home");
        }

        var socio = await _socioRepository.ObtenerPorIdAsync(user.SocioId.Value);
        var membresiaActiva = socio?.Membresias.FirstOrDefault(m => m.Estado == EstadoMembresia.ACTIVA);

        var horarios = await _horarioClaseRepository.ObtenerHorariosDisponiblesAsync();
        var misReservasEntidades = await _reservaRepository.ObtenerReservasPorSocioAsync(user.SocioId.Value);

        var model = new ClienteDashboardViewModel
        {
            NombreSocio = socio?.Nombre ?? user.NombreCompleto,
            EmailSocio = user.Email ?? "",
            EstadoMembresia = membresiaActiva != null ? "ACTIVA" : "ACTIVA", // Demostración activa
            TipoPlan = membresiaActiva?.TipoPlan ?? "PLAN MENSUAL PRO",
            PrecioPlan = 35.00m,
            FechaInicioMembresia = membresiaActiva?.FechaInicio ?? DateTime.UtcNow.AddDays(-5),
            FechaVencimientoMembresia = membresiaActiva?.FechaFin ?? DateTime.UtcNow.AddDays(25),
            ClasesAsistidasMes = socio?.Reservas.Count(r => r.Asistencia != null) ?? 8,
            ReservasActivasCount = misReservasEntidades.Count(r => r.Estado == EstadoReserva.CONFIRMADA),

            ClasesDisponibles = horarios.Select(h => new ClaseDisponibleDto
            {
                HorarioClaseId = h.Id,
                NombreDisciplina = h.Clase.Nombre,
                Instructor = h.Instructor,
                FechaHoraInicio = h.FechaHoraInicio,
                CupoMaximo = h.CupoMaximo,
                CuposOcupados = h.Reservas.Count(r => r.Estado == EstadoReserva.CONFIRMADA)
            }).ToList(),

            MisReservas = misReservasEntidades.Select(r => new ReservaSocioDto
            {
                ReservaId = r.Id,
                NombreClase = r.HorarioClase.Clase.Nombre,
                Instructor = r.HorarioClase.Instructor,
                FechaHora = r.HorarioClase.FechaHoraInicio,
                EstadoReserva = r.Estado.ToString(),
                PosicionListaEspera = r.PosicionListaEspera,
                QrCodeBase64 = r.Estado == EstadoReserva.CONFIRMADA 
                    ? _qrService.GenerarQrBase64($"PASE-QR-{r.Id}-{user.Email}") 
                    : string.Empty
            }).ToList(),

            // Historial de Pagos y Facturación
            HistorialPagos = new List<PagoSocioDto>
            {
                new PagoSocioDto
                {
                    PagoId = Guid.NewGuid(),
                    Concepto = "Suscripción Plan Mensual Pro",
                    Monto = 35.00m,
                    FechaPago = DateTime.UtcNow.AddDays(-5),
                    MetodoPago = "Tarjeta **** 4242",
                    Estado = "COMPLETADO"
                },
                new PagoSocioDto
                {
                    PagoId = Guid.NewGuid(),
                    Concepto = "Pase Suelto - Spinning Matutino",
                    Monto = 10.00m,
                    FechaPago = DateTime.UtcNow.AddDays(-20),
                    MetodoPago = "Billetera Digital / QR",
                    Estado = "COMPLETADO"
                }
            }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reservar(Guid horarioClaseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user?.SocioId == null) return RedirectToAction("Index");

        var horario = await _horarioClaseRepository.ObtenerPorIdAsync(horarioClaseId);
        if (horario == null)
        {
            TempData["ErrorMessage"] = "La clase seleccionada no existe.";
            return RedirectToAction("Index");
        }

        bool solapado = await _reservaRepository.ExisteSolapamientoHorarioAsync(
            user.SocioId.Value, horario.FechaHoraInicio, horario.Clase.DuracionMinutos);

        if (solapado)
        {
            TempData["ErrorMessage"] = "RN-05: Ya tienes una reserva confirmada en un horario que se cruza con esta clase.";
            return RedirectToAction("Index");
        }

        int inscritos = await _reservaRepository.ObtenerCantidadInscritosConfirmadosAsync(horarioClaseId);

        var nuevaReserva = new Reserva
        {
            Id = Guid.NewGuid(),
            HorarioClaseId = horarioClaseId,
            SocioId = user.SocioId.Value,
            FechaRegistro = DateTime.UtcNow
        };

        if (inscritos < horario.CupoMaximo)
        {
            nuevaReserva.Estado = EstadoReserva.CONFIRMADA;
            nuevaReserva.PosicionListaEspera = null;
            TempData["SuccessMessage"] = "¡Reserva CONFIRMADA con éxito! Tu pase QR ha sido generado.";
        }
        else
        {
            var colaActual = await _reservaRepository.ObtenerReservasPorHorarioAsync(horarioClaseId);
            int ultimaPosicion = colaActual
                .Where(r => r.Estado == EstadoReserva.LISTA_DE_ESPERA)
                .Max(r => r.PosicionListaEspera) ?? 0;

            nuevaReserva.Estado = EstadoReserva.LISTA_DE_ESPERA;
            nuevaReserva.PosicionListaEspera = ultimaPosicion + 1;
            TempData["WarningMessage"] = $"Aforo completo. Te hemos asignado la posición #{nuevaReserva.PosicionListaEspera} en la Lista de Espera (FIFO).";
        }

        await _reservaRepository.AgregarAsync(nuevaReserva);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(Guid reservaId)
    {
        var reserva = await _reservaRepository.ObtenerPorIdAsync(reservaId);
        if (reserva == null) return RedirectToAction("Index");

        var horasDiferencia = (reserva.HorarioClase.FechaHoraInicio - DateTime.UtcNow).TotalHours;

        if (horasDiferencia < 2)
        {
            reserva.Estado = EstadoReserva.CANCELADA;
            await _reservaRepository.ActualizarAsync(reserva);
            TempData["ErrorMessage"] = "RN-04: Cancelación realizada con menos de 2 horas de antelación. No se restituye el pase.";
            return RedirectToAction("Index");
        }

        bool eraConfirmada = reserva.Estado == EstadoReserva.CONFIRMADA;
        reserva.Estado = EstadoReserva.CANCELADA;
        reserva.PosicionListaEspera = null;
        await _reservaRepository.ActualizarAsync(reserva);

        if (eraConfirmada)
        {
            var cola = await _reservaRepository.ObtenerReservasPorHorarioAsync(reserva.HorarioClaseId);
            var primeroEnEspera = cola
                .Where(r => r.Estado == EstadoReserva.LISTA_DE_ESPERA)
                .OrderBy(r => r.PosicionListaEspera)
                .FirstOrDefault();

            if (primeroEnEspera != null)
            {
                primeroEnEspera.Estado = EstadoReserva.CONFIRMADA;
                primeroEnEspera.PosicionListaEspera = null;
                await _reservaRepository.ActualizarAsync(primeroEnEspera);

                var restantes = cola
                    .Where(r => r.Estado == EstadoReserva.LISTA_DE_ESPERA && r.Id != primeroEnEspera.Id)
                    .OrderBy(r => r.PosicionListaEspera)
                    .ToList();

                for (int i = 0; i < restantes.Count; i++)
                {
                    restantes[i].PosicionListaEspera = i + 1;
                    await _reservaRepository.ActualizarAsync(restantes[i]);
                }
            }
        }

        TempData["SuccessMessage"] = "Reserva cancelada oportunamente. El cupo se ha liberado en la lista de espera.";
        return RedirectToAction("Index");
    }
}