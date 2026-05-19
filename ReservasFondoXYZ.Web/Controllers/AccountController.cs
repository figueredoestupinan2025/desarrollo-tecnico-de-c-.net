using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace ReservasFondoXYZ.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailService _emailService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger,
        IEmailService emailService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.NroDocumento);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Clave, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario autenticado correctamente.");
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Cuenta de usuario bloqueada.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
                return View(model);
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Usuario cerró sesión.");
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.NroDocumento,
                Email = model.Email,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                DocumentoIdentidad = model.NroDocumento,
                Telefono = model.Telefono
            };

            var result = await _userManager.CreateAsync(user, model.Clave);
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario creó una nueva cuenta con contraseña.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, protocol: Request.Scheme);

            await _emailService.SendEmailAsync(
                to: user.Email,
                subject: "Restablecer Contraseña - Fondo XYZ",
                message: $"Por favor restablece tu contraseña haciendo clic <a href='{callbackUrl}'>aquí</a>.");

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? token = null, string? email = null)
    {
        if (token == null || email == null)
        {
            return BadRequest("Un token y un email son necesarios para restablecer la contraseña.");
        }

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }
}

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
