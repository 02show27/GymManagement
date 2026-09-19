using System.ComponentModel.DataAnnotations;

namespace GymManagement.Presentation.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme en este equipo")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}