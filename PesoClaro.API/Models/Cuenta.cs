namespace PesoClaro.API.Models;

public class Cuenta
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    // Liquidez | Inversion | Deuda | Ahorro
    public decimal Saldo { get; set; }
    public decimal TasaAnual { get; set; }
    public string TipoTasa { get; set; } = string.Empty;
    // Rendimiento | Interes
    public int? DiaDeCorteMensual { get; set; }
    public string Notas { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool Activa { get; set; } = true;

    // Relación con Usuario
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

     public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}