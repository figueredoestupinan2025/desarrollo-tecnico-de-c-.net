param(
  [string]$BaseUrl = "http://localhost:5241",
  [string]$Documento = "999999999",
  [string]$Password = "Admin123!"
)

$ErrorActionPreference = "Stop"

Write-Host "Smoke test -> $BaseUrl"

$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Host "1) GET /"
$homeResponse = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/" -WebSession $session
if ($homeResponse.StatusCode -ne 200) { throw "Home returned $($homeResponse.StatusCode)" }

Write-Host "2) Login"
$loginPage = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/Account/Login" -WebSession $session
$token = [regex]::Match($loginPage.Content, 'name="__RequestVerificationToken"\s+type="hidden"\s+value="([^"]+)"').Groups[1].Value
if ([string]::IsNullOrWhiteSpace($token)) { throw "Could not find __RequestVerificationToken in login page" }

$form = @{
  NroDocumento = $Documento
  Clave = $Password
  RememberMe = "false"
  __RequestVerificationToken = $token
}

$loginResult = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/Account/Login" -Method POST -WebSession $session -Body $form -MaximumRedirection 0 -ErrorAction SilentlyContinue
if ($loginResult.StatusCode -ne 302) { throw "Login POST expected 302, got $($loginResult.StatusCode)" }

Write-Host "3) GET /Reserva/Disponibilidad (authorized)"
$disp = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/Reserva/Disponibilidad" -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
if ($disp.StatusCode -ne 200) { throw "Disponibilidad expected 200, got $($disp.StatusCode)" }

Write-Host "4) GET /api/ReservasApi/disponibilidad (authorized JSON)"
$api = Invoke-RestMethod -Uri "$BaseUrl/api/ReservasApi/disponibilidad?fechaInicio=2026-07-01&fechaFin=2026-07-03&numeroPersonas=2" -WebSession $session
if ($null -eq $api) { throw "API returned null" }

$count = if ($api -is [System.Array]) { $api.Count } else { 1 }
Write-Host "OK - habitaciones disponibles: $count"

Write-Host "5) Reserva end-to-end (pago mock)"
$first = if ($api -is [System.Array]) { $api[0] } else { $api }
if ($null -eq $first) { throw "No availability to reserve" }

$sitioId = $first.sitioId
$alojamientoId = $first.alojamientoId
$habitacionId = $first.id

$crearUrl = "$BaseUrl/Reserva/Crear?sitioId=$sitioId&alojamientoId=$alojamientoId&fechaInicio=2026-07-01&fechaFin=2026-07-03&numeroPersonas=2&habitacionesIds=$habitacionId"
$pagoPage = Invoke-WebRequest -UseBasicParsing -Uri $crearUrl -WebSession $session
if ($pagoPage.StatusCode -ne 200) { throw "Pago page returned $($pagoPage.StatusCode)" }

$pagoToken = [regex]::Match($pagoPage.Content, 'name="__RequestVerificationToken"\s+type="hidden"\s+value="([^"]+)"').Groups[1].Value
if ([string]::IsNullOrWhiteSpace($pagoToken)) { throw "Could not find __RequestVerificationToken in pago page" }

$post = @{
  SitioId = "$sitioId"
  AlojamientoId = "$alojamientoId"
  FechaInicio = "2026-07-01"
  FechaFin = "2026-07-03"
  NumeroPersonas = "2"
  NumeroHabitaciones = "1"
  HabitacionId = "$habitacionId"
  HabitacionesIds = "$habitacionId"
  TarifaTotal = ([regex]::Match($pagoPage.Content, 'name="TarifaTotal"[^>]*value="([^"]+)"').Groups[1].Value)
  NumeroTarjeta = "4111111111111111"
  NombreTitular = "SMOKE TEST"
  FechaExpiracion = "12/30"
  CVV = "123"
  __RequestVerificationToken = $pagoToken
}

if ([string]::IsNullOrWhiteSpace($post.TarifaTotal)) { throw "Could not parse TarifaTotal hidden value" }

$pagoPost = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/Reserva/Pago" -Method POST -WebSession $session -Body $post -MaximumRedirection 0 -ErrorAction SilentlyContinue
if ($pagoPost.StatusCode -ne 302) { throw "Pago POST expected 302, got $($pagoPost.StatusCode)" }

$confirmUrl = $pagoPost.Headers.Location
if (-not $confirmUrl) { throw "Missing redirect location after pago POST" }
if ($confirmUrl -notmatch '^https?://') { $confirmUrl = "$BaseUrl$confirmUrl" }

$confirm = Invoke-WebRequest -UseBasicParsing -Uri $confirmUrl -WebSession $session
if ($confirm.StatusCode -ne 200) { throw "Confirmacion returned $($confirm.StatusCode)" }
if ($confirm.Content -notmatch 'Reserva Confirmada') { throw "Confirmacion page did not contain expected text" }

Write-Host "OK - reserva creada y confirmada"

Write-Host "6) Validar Mis Reservas + Detalles"
$idMatch = [regex]::Match($confirmUrl, '(?:[?&]id=(\d+)\b|/(\d+)\b)')
$reservaId = if ($idMatch.Success) { if ($idMatch.Groups[1].Success) { $idMatch.Groups[1].Value } else { $idMatch.Groups[2].Value } } else { "" }
if ([string]::IsNullOrWhiteSpace($reservaId)) { throw "Could not parse reserva Id from confirmacion redirect URL: $confirmUrl" }

$mis = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/Reserva/MisReservas" -WebSession $session
if ($mis.StatusCode -ne 200) { throw "MisReservas returned $($mis.StatusCode)" }
if ($mis.Content -notmatch 'Mis Reservas') { throw "MisReservas did not return the expected page (possible redirect to login)" }
if ($mis.Content -notmatch "<td[^>]*>\s*$reservaId\s*</td>") { throw "MisReservas page does not list reserva #$reservaId" }

$det = Invoke-WebRequest -UseBasicParsing -Uri "$BaseUrl/Reserva/Detalles?id=$reservaId" -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
if ($det.StatusCode -ne 200) { throw "Detalles returned $($det.StatusCode)" }

Write-Host "OK - Mis Reservas y Detalles cargan (reserva #$reservaId)"
