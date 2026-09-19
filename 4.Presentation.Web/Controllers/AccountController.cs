using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Presentation.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Presentation.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ISocioRepository _socioRepository;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ISocioRepository socioRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _socioRepository = socioRepository;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "AdminDashboard");
            }
            return RedirectToAction("Index", "ClienteDashboard");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Administrador"))
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "ClienteDashboard");
            }
        }

        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido. Verifique sus credenciales.");
        return View(model);
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register (Registro vinculado con la entidad Socio)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 1. Crear el Socio en el Dominio
        var nuevoSocio = new Socio
        {
            Id = Guid.NewGuid(),
            Nombre = model.NombreCompleto,
            Email = model.Email,
            Telefono = model.Telefono,
            FechaRegistro = DateTime.UtcNow
        };

        await _socioRepository.AgregarAsync(nuevoSocio);

        // 2. Crear la entidad ApplicationUser asociada
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            NombreCompleto = model.NombreCompleto,
            Telefono = model.Telefono,
            FechaRegistro = DateTime.UtcNow,
            SocioId = nuevoSocio.Id // Enlace explicito con la tabla Socios
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("Cliente"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Cliente"));
            }

            await _userManager.AddToRoleAsync(user, "Cliente");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "ClienteDashboard");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}