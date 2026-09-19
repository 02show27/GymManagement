using Microsoft.AspNetCore.Identity;

namespace GymManagement.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public Guid? SocioId { get; set; }
}