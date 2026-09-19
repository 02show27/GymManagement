using GymManagement.Domain.Entities;
using GymManagement.Domain.Interfaces;
using GymManagement.Infrastructure.Persistence;
using GymManagement.Infrastructure.Repositories;
using GymManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Entity Framework Core con PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Registrar Repositorios de Infraestructura y Servicios
builder.Services.AddScoped<ISocioRepository, SocioRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IHorarioClaseRepository, HorarioClaseRepository>();
builder.Services.AddScoped<IQrGeneratorService, QrGeneratorService>();
builder.Services.AddScoped<IReportePdfService, ReportePdfService>();
// 3. Configurar ASP.NET Core Identity con Soporte de Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Políticas de contraseñas
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 4. Configurar redirección de cookies de sesión
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Seeder para inicializar Migraciones, Roles, Administrador y Clases de Prueba
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // A. Crear Roles por defecto
        string[] roleNames = { "Administrador", "Cliente" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // B. Crear Admin por defecto si no existe
        var adminEmail = "admin@gymmanagement.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                NombreCompleto = "Administrador del Sistema",
                Telefono = "70000000",
                EmailConfirmed = true
            };
            var createAdmin = await userManager.CreateAsync(admin, "Admin123!");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Administrador");
            }
        }

        // C. Sembrado Inicial de Clases y Horarios
        if (!context.Clases.Any())
        {
            var claseSpinning = new Clase 
            { 
                Id = Guid.NewGuid(), 
                Nombre = "Spinning Matutino", 
                Descripcion = "Ciclismo bajo en impacto", 
                DuracionMinutos = 50 
            };
            
            var claseCrossfit = new Clase 
            { 
                Id = Guid.NewGuid(), 
                Nombre = "CrossFit Intensivo", 
                Descripcion = "Entrenamiento funcional de alta intensidad", 
                DuracionMinutos = 60 
            };

            await context.Clases.AddRangeAsync(claseSpinning, claseCrossfit);

            var horario1 = new HorarioClase 
            { 
                Id = Guid.NewGuid(), 
                ClaseId = claseSpinning.Id, 
                Instructor = "Carlos Mendoza", 
                FechaHoraInicio = DateTime.UtcNow.AddDays(1).AddHours(8), 
                CupoMaximo = 2 
            };

            var horario2 = new HorarioClase 
            { 
                Id = Guid.NewGuid(), 
                ClaseId = claseCrossfit.Id, 
                Instructor = "Andrea Ruiz", 
                FechaHoraInicio = DateTime.UtcNow.AddDays(1).AddHours(19), 
                CupoMaximo = 1 
            };

            await context.HorariosClases.AddRangeAsync(horario1, horario2);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error durante la migración o inicialización de la base de datos.");
    }
}

// 6. Configurar Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();