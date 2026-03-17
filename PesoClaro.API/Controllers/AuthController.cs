using Microsoft.AspNetCore.Mvc;
using PesoClaro.API.DTOs;
using PesoClaro.API.Services;

namespace PesoClaro.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { mensaje = "Todos los campos son requeridos." });
        }

        var resultado = await _authService.RegistrarAsync(dto);

        if (resultado == null)
            return Conflict(new { mensaje = "El email ya está registrado." });

        return Ok(resultado);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { mensaje = "Email y contraseña son requeridos." });
        }

        var resultado = await _authService.LoginAsync(dto);

        if (resultado == null)
            return Unauthorized(new { mensaje = "Credenciales incorrectas." });

        return Ok(resultado);
    }
}