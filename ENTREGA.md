# Entrega – Prueba Técnica (Reservas Fondo XYZ)

## Resumen
Aplicación MVC en .NET (arquitectura en capas) para consultar disponibilidad, calcular tarifa y gestionar reservas, usando Microsoft SQL Server y procedimientos almacenados.

## Requisitos vs implementación

- Analizar funcionalidad: Implementado en el flujo MVC y documentado en `README.md`.
- BD relacional en SQL Server: `DatabaseScript.sql`.
- SP habitaciones disponibles por fechas: `StoredProcedures.sql` (`ObtenerHabitacionesDisponiblesPorFechas`).
- SP habitaciones disponibles por fechas + personas: `StoredProcedures.sql` (`ObtenerHabitacionesDisponiblesPorFechasYPersonas`).
- SP consultar tarifas: `StoredProcedures.sql` (`ObtenerTarifas`).
- SP calcular tarifa total: `StoredProcedures.sql` (`CalcularTarifaTotal`).
- Formularios web (Razor): implementados en `ReservasFondoXYZ.Web/Views`.
- Registro de usuarios: `Account/Register`.
- Login (solo autenticados): `Account/Login` + `[Authorize]` en reservas.
- Recuperación de contraseña por SMTP: `Account/ForgotPassword` (requiere configurar `SmtpSettings`).
- CRUD necesarios: Sitios/Alojamientos/Habitaciones/Temporadas/Tarifas (rol Admin).
- Consultar disponibilidad: `Reserva/Disponibilidad` (usa SP vía capa negocio; fallback LINQ).
- Guardar reservas en BD: `Reserva/Pago` (pago MOCK) y persistencia de `Reserva` + `ReservaHabitacion`.

## Pago en línea
El requerimiento menciona “pagar en línea”. En esta entrega el flujo de pago es **en línea dentro del módulo**, con **gateway configurable** y modo **Mock** por defecto (sin pasarela externa).
- Visible en la pantalla `Reserva/Pago`.
- Configuración: `ReservasFondoXYZ.Web/appsettings.json` -> `Payment:Provider` (por defecto `Mock`).
- Punto de extensión: cambiar `Payment:Provider` y conectar un gateway real (la app rechaza pagos si el proveedor no está configurado).

## Cómo ejecutar (camino feliz)
Ver `DEMO.md`.

## Anexo (PDF escaneado)
El anexo parece requerir OCR para validar requisitos adicionales. Ver `ANEXO_VALIDACION.md` y `scripts/ocr/README.md`.

## Evidencia rápida
- Build: `dotnet build ReservasFondoXYZ.sln -c Release`
- Tests: `dotnet test .\ReservasFondoXYZ.Tests\ReservasFondoXYZ.Tests.csproj -c Release`
