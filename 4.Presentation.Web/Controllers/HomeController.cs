using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymManagement.Presentation.Web.Models;

namespace GymManagement.Presentation.Web.Controllers;
[AllowAnonymous]
public class HomeController : Controller
{
    // GET: / (Página principal pública de inicio)
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Home/Dashboard (Dashboard público / métricas generales)
    public IActionResult Dashboard()
    {
        var model = new HomeDashboardViewModel
        {
            TotalSociosActivos = 320,
            ClasesProgramadasHoy = 8,
            ReservasConfirmadasMes = 1240,
            OcupacionPromedioPorcentaje = 87.5m,
            ProximasClases = new List<ClaseResumenDto>
            {
                new ClaseResumenDto
                {
                    HorarioClaseId = Guid.NewGuid(),
                    NombreClase = "Spinning Matutino",
                    Instructor = "Carlos Mendoza",
                    FechaHora = DateTime.Today.AddHours(8),
                    CupoMaximo = 15,
                    InscritosConfirmados = 15,
                    EnListaEspera = 3
                },
                new ClaseResumenDto
                {
                    HorarioClaseId = Guid.NewGuid(),
                    NombreClase = "CrossFit Avanzado",
                    Instructor = "Andrea Ruiz",
                    FechaHora = DateTime.Today.AddHours(10),
                    CupoMaximo = 20,
                    InscritosConfirmados = 14,
                    EnListaEspera = 0
                },
                new ClaseResumenDto
                {
                    HorarioClaseId = Guid.NewGuid(),
                    NombreClase = "Yoga / Pilates Core",
                    Instructor = "Mariana Silva",
                    FechaHora = DateTime.Today.AddHours(18),
                    CupoMaximo = 12,
                    InscritosConfirmados = 12,
                    EnListaEspera = 1
                }
            }
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}