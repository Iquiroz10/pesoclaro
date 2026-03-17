using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesoClaro.API.DTOs;
using PesoClaro.API.Services;

namespace PesoClaro.API.Controllers;

[ApiController]
[Route("api/cuentas")]
[Authorize]
public class CuentasController : ControllerBase
{
    private readonly CuentaService _cuentaService;

    public CuentasController(CuentaService cuentaService)
    {
        _cuentaService = cuentaService;
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("sub");
        return int.Parse(claim!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCuentaDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) ||
            string.IsNullOrWhiteSpace(dto.Tipo))
        {
            return BadRequest(new { mensaje = "Nombre y tipo son requeridos." });
        }

        var usuarioId = ObtenerUsuarioId();
        var resultado = await _cuentaService.CrearAsync(usuarioId, dto);
        return Ok(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var usuarioId = ObtenerUsuarioId();
        var cuentas = await _cuentaService.ObtenerTodasAsync(usuarioId);
        return Ok(cuentas);
    }

    [HttpPut("{id}/saldo")]
    public async Task<IActionResult> ActualizarSaldo(
        int id, [FromBody] ActualizarSaldoDTO dto)
    {
        var usuarioId = ObtenerUsuarioId();
        var resultado = await _cuentaService
            .ActualizarSaldoAsync(id, usuarioId, dto);

        if (resultado == null)
            return NotFound(new { mensaje = "Cuenta no encontrada." });

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var usuarioId = ObtenerUsuarioId();
        var eliminada = await _cuentaService.EliminarAsync(id, usuarioId);

        if (!eliminada)
            return NotFound(new { mensaje = "Cuenta no encontrada." });

        return Ok(new { mensaje = "Cuenta eliminada correctamente." });
    }
}