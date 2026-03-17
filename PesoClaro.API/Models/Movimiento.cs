namespace PesoClaro.API.Models;

public class Movimiento
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    // Ingreso | Gasto | Transferencia
    public decimal Monto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    // Relación con Cuenta
    public int CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;

    // Relación con Usuario
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}