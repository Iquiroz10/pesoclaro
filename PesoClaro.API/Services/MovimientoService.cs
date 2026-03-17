using Microsoft.EntityFrameworkCore;
using PesoClaro.API.Data;
using PesoClaro.API.DTOs;
using PesoClaro.API.Models;

namespace PesoClaro.API.Services;

public class MovimientoService
{
    private readonly PesoClaroContext _context;

    public MovimientoService(PesoClaroContext context)
    {
        _context = context;
    }

    public async Task<MovimientoResponseDTO?> CrearAsync(
        int usuarioId, CrearMovimientoDTO dto)
    {
        // Verificar que la cuenta pertenece al usuario
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.Id == dto.CuentaId
                && c.UsuarioId == usuarioId
                && c.Activa);

        if (cuenta == null) return null;

        var movimiento = new Movimiento
        {
            Tipo        = dto.Tipo,
            Monto       = dto.Monto,
            Categoria   = dto.Categoria,
            Descripcion = dto.Descripcion,
            Fecha       = dto.Fecha,
            CuentaId    = dto.CuentaId,
            UsuarioId   = usuarioId
        };

        // Actualizar saldo de la cuenta automáticamente
        if (dto.Tipo == "Ingreso")
            cuenta.Saldo += dto.Monto;
        else if (dto.Tipo == "Gasto")
            cuenta.Saldo -= dto.Monto;

        _context.Movimientos.Add(movimiento);
        await _context.SaveChangesAsync();

        return new MovimientoResponseDTO
        {
            Id           = movimiento.Id,
            Tipo         = movimiento.Tipo,
            Monto        = movimiento.Monto,
            Categoria    = movimiento.Categoria,
            Descripcion  = movimiento.Descripcion,
            Fecha        = movimiento.Fecha,
            NombreCuenta = cuenta.Nombre
        };
    }

    public async Task<ResumenMovimientosDTO> ObtenerResumenAsync(
        int usuarioId, int mes, int anio)
    {
        var movimientos = await _context.Movimientos
            .Include(m => m.Cuenta)
            .Where(m => m.UsuarioId == usuarioId
                && m.Fecha.Month == mes
                && m.Fecha.Year == anio)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        var totalIngresos = movimientos
            .Where(m => m.Tipo == "Ingreso")
            .Sum(m => m.Monto);

        var totalGastos = movimientos
            .Where(m => m.Tipo == "Gasto")
            .Sum(m => m.Monto);

        var gastosPorCategoria = movimientos
            .Where(m => m.Tipo == "Gasto")
            .GroupBy(m => m.Categoria)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(g.Sum(m => m.Monto), 2)
            );

        return new ResumenMovimientosDTO
        {
            TotalIngresos      = Math.Round(totalIngresos, 2),
            TotalGastos        = Math.Round(totalGastos, 2),
            Diferencia         = Math.Round(totalIngresos - totalGastos, 2),
            GastosPorCategoria = gastosPorCategoria,
            Movimientos        = movimientos.Select(m => new MovimientoResponseDTO
            {
                Id           = m.Id,
                Tipo         = m.Tipo,
                Monto        = m.Monto,
                Categoria    = m.Categoria,
                Descripcion  = m.Descripcion,
                Fecha        = m.Fecha,
                NombreCuenta = m.Cuenta.Nombre
            }).ToList()
        };
    }
}