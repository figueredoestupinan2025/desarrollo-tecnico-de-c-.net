using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "El Nro Documento es requerido")]
    [Display(Name = "Nro Documento")]
    public string NroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La Clave es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Clave")]
    public string Clave { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "El Nombre es requerido")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El Apellido es requerido")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El Nro Documento es requerido")]
    [Display(Name = "Nro Documento")]
    public string NroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El Correo Electrónico es requerido")]
    [EmailAddress]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El Teléfono es requerido")]
    [Phone]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La Clave es requerida")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Clave")]
    public string Clave { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Clave")]
    [Compare("Clave", ErrorMessage = "La clave y la confirmación no coinciden.")]
    public string ConfirmarClave { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El Correo Electrónico es requerido")]
    [EmailAddress]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "El Correo Electrónico es requerido")]
    [EmailAddress]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La Clave es requerida")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Clave")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Clave")]
    [Compare("Password", ErrorMessage = "La clave y la confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
