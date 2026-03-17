using Microsoft.EntityFrameworkCore;
using PesoClaro.API.Data;
using PesoClaro.API.DTOs;
using PesoClaro.API.Models;

namespace PesoClaro.API.Services;

public class CuentaService
{
    private readonly PesoClaroContext _context;

    public CuentaService(PesoClaroContext context)
    {
        _context = context;
    }

    public async Task<CuentaResponseDTO> CrearAsync(int usuarioId, CrearCuentaDTO dto)
    {
        var cuenta = new Cuenta
        {
            Nombre = dto.Nombre,
            Tipo = dto.Tipo,
            Saldo = dto.Saldo,
            TasaAnual = dto.TasaAnual,
            TipoTasa = dto.TipoTasa,
            DiaDeCorteMensual = dto.DiaDeCorteMensual,
            Notas = dto.Notas,
            UsuarioId = usuarioId
        };

        _context.Cuentas.Add(cuenta);
        await _context.SaveChangesAsync();

        return MapearACuentaResponse(cuenta);
    }

    public async Task<List<CuentaResponseDTO>> ObtenerTodasAsync(int usuarioId)
    {
        var cuentas = await _context.Cuentas
            .Where(c => c.UsuarioId == usuarioId && c.Activa)
            .OrderBy(c => c.Tipo)
            .ToListAsync();

        return cuentas.Select(MapearACuentaResponse).ToList();
    }

    public async Task<CuentaResponseDTO?> ActualizarSaldoAsync(
        int cuentaId, int usuarioId, ActualizarSaldoDTO dto)
    {
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.Id == cuentaId && c.UsuarioId == usuarioId && c.Activa);

        if (cuenta == null) return null;

        cuenta.Saldo = dto.NuevoSaldo;
        await _context.SaveChangesAsync();

        return MapearACuentaResponse(cuenta);
    }

    public async Task<bool> EliminarAsync(int cuentaId, int usuarioId)
    {
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.Id == cuentaId && c.UsuarioId == usuarioId && c.Activa);

        if (cuenta == null) return false;

        cuenta.Activa = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private CuentaResponseDTO MapearACuentaResponse(Cuenta cuenta)
    {
        var rendimientoAnual = cuenta.TipoTasa == "Rendimiento"
            ? cuenta.Saldo * cuenta.TasaAnual / 100
            : -(cuenta.Saldo * cuenta.TasaAnual / 100);

        return new CuentaResponseDTO
        {
            Id = cuenta.Id,
            Nombre = cuenta.Nombre,
            Tipo = cuenta.Tipo,
            Saldo = cuenta.Saldo,
            TasaAnual = cuenta.TasaAnual,
            TipoTasa = cuenta.TipoTasa,
            DiaDeCorteMensual = cuenta.DiaDeCorteMensual,
            Notas = cuenta.Notas,
            RendimientoAnual = Math.Round(rendimientoAnual, 2),
            RendimientoMensual = Math.Round(rendimientoAnual / 12, 2),
            RendimientoDiario = Math.Round(rendimientoAnual / 365, 2),
            FechaCreacion = cuenta.FechaCreacion
        };
    }
}