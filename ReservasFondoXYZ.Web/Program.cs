using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservasFondoXYZ.Business.Services;
using ReservasFondoXYZ.Data.Data;
using ReservasFondoXYZ.Data.Models;
using ReservasFondoXYZ.Web.Services.Payments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var saPassword = builder.Configuration["MSSQL_SA_PASSWORD"];
if (!string.IsNullOrWhiteSpace(saPassword) && connectionString.Contains("${MSSQL_SA_PASSWORD}", StringComparison.Ordinal))
{
    connectionString = connectionString.Replace("${MSSQL_SA_PASSWORD}", saPassword, StringComparison.Ordinal);
}

if (connectionString.Contains("${MSSQL_SA_PASSWORD}", StringComparison.Ordinal))
{
    throw new InvalidOperationException("La cadena de conexiÃ³n contiene ${MSSQL_SA_PASSWORD} pero no se encontrÃ³ la variable de entorno MSSQL_SA_PASSWORD.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Registrar servicios de la capa de negocio
builder.Services.AddScoped<ISitioService, SitioService>();
builder.Services.AddScoped<IAlojamientoService, AlojamientoService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITarifaService, TarifaService>();
builder.Services.AddScoped<ITemporadaService, TemporadaService>();

builder.Services.Configure<PaymentOptions>(builder.Configuration.GetSection(PaymentOptions.SectionName));
builder.Services.AddScoped<IPaymentGateway>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PaymentOptions>>().Value;
    return string.Equals(options.Provider, "Mock", StringComparison.OrdinalIgnoreCase)
        ? new MockPaymentGateway()
        : ActivatorUtilities.CreateInstance<UnconfiguredPaymentGateway>(sp);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { "Admin", "Usuario" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = app.Configuration["DefaultAdmin:Email"] ?? "admin@reservas.local";
    var adminDocument = app.Configuration["DefaultAdmin:Document"] ?? "999999999";
    var adminPassword = app.Configuration["DefaultAdmin:Password"] ?? "Admin123!";

    var admin = await userManager.FindByNameAsync(adminDocument);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminDocument,
            Email = adminEmail,
            Nombre = "Administrador",
            Apellido = "Sistema",
            DocumentoIdentidad = adminDocument,
            Telefono = "3000000000"
        };

        await userManager.CreateAsync(admin, adminPassword);
    }

    if (admin != null && !await userManager.IsInRoleAsync(admin, "Admin"))
    {
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
