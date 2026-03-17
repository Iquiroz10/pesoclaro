using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesoClaro.API.Services;

namespace PesoClaro.API.Controllers;

[ApiController]
[Route("api/patrimonio")]
[Authorize]
public class PatrimonioController : ControllerBase
{
    private readonly PatrimonioService _patrimonioService;

    public PatrimonioController(PatrimonioService patrimonioService)
    {
        _patrimonioService = patrimonioService;
    }

    private int ObtenerUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("sub");
        return int.Parse(claim!.Value);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPatrimonio()
    {
        var usuarioId = ObtenerUsuarioId();
        var patrimonio = await _patrimonioService.ObtenerPatrimonioAsync(usuarioId);
        return Ok(patrimonio);
    }
}