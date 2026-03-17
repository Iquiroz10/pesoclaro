using PesoClaro.API.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    // Navegación — un usuario tiene muchas cuentas
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    
}