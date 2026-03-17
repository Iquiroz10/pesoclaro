using Microsoft.EntityFrameworkCore;
using PesoClaro.API.Data;
using PesoClaro.API.DTOs;

namespace PesoClaro.API.Services;

public class PatrimonioService
{
    private readonly PesoClaroContext _context;

    public PatrimonioService(PesoClaroContext context)
    {
        _context = context;
    }

    public async Task<PatrimonioDTO> ObtenerPatrimonioAsync(int usuarioId)
    {
        var cuentas = await _context.Cuentas
            .Where(c => c.UsuarioId == usuarioId && c.Activa)
            .ToListAsync();

        // ── Totales por tipo ───────────────────────────────
        var totalLiquidez = cuentas
            .Where(c => c.Tipo == "Liquidez")
            .Sum(c => c.Saldo);

        var totalInversiones = cuentas
            .Where(c => c.Tipo == "Inversion")
            .Sum(c => c.Saldo);

        var totalAhorros = cuentas
            .Where(c => c.Tipo == "Ahorro")
            .Sum(c => c.Saldo);

        var totalDeudas = cuentas
            .Where(c => c.Tipo == "Deuda")
            .Sum(c => c.Saldo);

        var totalActivos = totalLiquidez + totalInversiones + totalAhorros;

        // ── Rendimientos y costos ──────────────────────────
        var rendimientoMensual = cuentas
            .Where(c => c.TipoTasa == "Rendimiento" && c.Activa)
            .Sum(c => c.Saldo * c.TasaAnual / 100 / 12);

        var costoMensualDeudas = cuentas
            .Where(c => c.TipoTasa == "Interes" && c.Activa)
            .Sum(c => c.Saldo * c.TasaAnual / 100 / 12);

        // ── Consejos automáticos ───────────────────────────
        var consejos = GenerarConsejos(cuentas.Select(c => new CuentaResponseDTO
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Tipo = c.Tipo,
            Saldo = c.Saldo,
            TasaAnual = c.TasaAnual,
            TipoTasa = c.TipoTasa,
            DiaDeCorteMensual = c.DiaDeCorteMensual,
            RendimientoMensual = c.TipoTasa == "Rendimiento"
                ? Math.Round(c.Saldo * c.TasaAnual / 100 / 12, 2)
                : -Math.Round(c.Saldo * c.TasaAnual / 100 / 12, 2)
        }).ToList());

        return new PatrimonioDTO
        {
            TotalLiquidez      = Math.Round(totalLiquidez, 2),
            TotalInversiones   = Math.Round(totalInversiones, 2),
            TotalAhorros       = Math.Round(totalAhorros, 2),
            TotalDeudas        = Math.Round(totalDeudas, 2),
            TotalActivos       = Math.Round(totalActivos, 2),
            PatrimonioNeto     = Math.Round(totalActivos - totalDeudas, 2),
            RendimientoMensualTotal = Math.Round(rendimientoMensual, 2),
            CostoMensualDeudas = Math.Round(costoMensualDeudas, 2),
            RendimientoNeto    = Math.Round(rendimientoMensual - costoMensualDeudas, 2),
            Consejos           = consejos
        };
    }

    private List<ConsejoDTO> GenerarConsejos(List<CuentaResponseDTO> cuentas)
    {
        var consejos = new List<ConsejoDTO>();

        // Consejo 1: deuda con tasa mayor que rendimientos
        var deudas = cuentas.Where(c => c.TipoTasa == "Interes").ToList();
        var rendimientos = cuentas.Where(c => c.TipoTasa == "Rendimiento").ToList();

        foreach (var deuda in deudas)
        {
            var cuentaConMayorRendimiento = rendimientos
                .Where(r => r.Saldo > 0 && r.TasaAnual < deuda.TasaAnual)
                .OrderByDescending(r => r.TasaAnual)
                .FirstOrDefault();

            if (cuentaConMayorRendimiento != null)
            {
                var saldoAplicable = Math.Min(cuentaConMayorRendimiento.Saldo, deuda.Saldo);
                var ahorro = Math.Round((deuda.TasaAnual - cuentaConMayorRendimiento.TasaAnual) / 100 / 12 * saldoAplicable, 2);

                consejos.Add(new ConsejoDTO
                {
                    Tipo = "Deuda",
                    Mensaje = $"Usar ${saldoAplicable} de {cuentaConMayorRendimiento.Nombre} para " +
                    $"abonar a {deuda.Nombre} te ahorraría ${ahorro} pesos netos cada mes.",
                    ImpactoMensual = ahorro
                });
            }
        }

        // Consejo 2: dinero parado a 0%
        var dineroParado = cuentas
            .Where(c => c.TipoTasa == "Rendimiento" && c.TasaAnual == 0 && c.Saldo > 0)
            .ToList();

        foreach (var cuenta in dineroParado)
        {
            consejos.Add(new ConsejoDTO
            {
                Tipo = "Oportunidad",
                Mensaje = $"Tienes ${cuenta.Saldo} en {cuenta.Nombre} al 0% de rendimiento. " +
                          $"Moverlo a una cuenta con rendimiento podría generarte dinero extra cada mes.",
                ImpactoMensual = 0
            });
        }

        // Consejo 3: alerta de corte próximo
        var hoy = DateTime.Now.Day;
        var cuentasConCorte = cuentas
            .Where(c => c.DiaDeCorteMensual.HasValue &&
                        c.DiaDeCorteMensual.Value >= hoy &&
                        c.DiaDeCorteMensual.Value <= hoy + 5)
            .ToList();

        foreach (var cuenta in cuentasConCorte)
        {
            var diasRestantes = cuenta.DiaDeCorteMensual!.Value - hoy;
            consejos.Add(new ConsejoDTO
            {
                Tipo = "Alerta",
                Mensaje = $"Tu {cuenta.Nombre} vence en {diasRestantes} días " +
                          $"(día {cuenta.DiaDeCorteMensual}). Saldo actual: ${cuenta.Saldo}.",
                ImpactoMensual = Math.Abs(cuenta.RendimientoMensual)
            });
        }

        return consejos;
    }
}