using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PesoClaro.API.Data;
using PesoClaro.API.DTOs;
using PesoClaro.API.Models;

namespace PesoClaro.API.Services;

public class AuthService
{
    private readonly PesoClaroContext _context;
    private readonly IConfiguration _config;

    public AuthService(PesoClaroContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponseDTO?> RegistrarAsync(RegistroDTO dto)
    {
        // Verificar si el email ya existe
        var existe = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email.ToLower());

        if (existe) return null;

        // Crear el usuario con contraseña hasheada
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return GenerarToken(usuario);
    }

    public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
    {
        // Buscar usuario por email
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower() && u.Activo);

        if (usuario == null) return null;

        // Verificar contraseña
        var passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
        if (!passwordValida) return null;

        return GenerarToken(usuario);
    }

    private AuthResponseDTO GenerarToken(Usuario usuario)
    {
        var secret = _config["Jwt:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddHours(
            double.Parse(_config["Jwt:ExpirationHours"]!)
        );

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("nombre", usuario.Nombre)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiracion,
            signingCredentials: creds
        );

        return new AuthResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Nombre = usuario.Nombre,
            Email = usuario.Email
        };
    }
}