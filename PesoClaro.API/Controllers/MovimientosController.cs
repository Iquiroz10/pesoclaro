using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesoClaro.API.DTOs;
using PesoClaro.API.Services;

namespace PesoClaro.API.Controllers;

[ApiController]
[Route("api/movimientos")]
[Authorize]
public class MovimientosController : ControllerBase
{
    private readonly MovimientoService _movimientoService;

    public MovimientosController(MovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("sub");
        return int.Parse(claim!.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearMovimientoDTO dto)
    {
        if (dto.Monto <= 0)
            return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });

        if (string.IsNullOrWhiteSpace(dto.Tipo) ||
            string.IsNullOrWhiteSpace(dto.Categoria))
            return BadRequest(new { mensaje = "Tipo y categoría son requeridos." });

        var usuarioId = ObtenerUsuarioId();
        var resultado = await _movimientoService.CrearAsync(usuarioId, dto);

        if (resultado == null)
            return NotFound(new { mensaje = "Cuenta no encontrada." });

        return Ok(resultado);
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> ObtenerResumen(
        [FromQuery] int mes = 0,
        [FromQuery] int anio = 0)
    {
        if (mes == 0) mes = DateTime.Now.Month;
        if (anio == 0) anio = DateTime.Now.Year;

        var usuarioId = ObtenerUsuarioId();
        var resumen = await _movimientoService
            .ObtenerResumenAsync(usuarioId, mes, anio);

        return Ok(resumen);
    }
}