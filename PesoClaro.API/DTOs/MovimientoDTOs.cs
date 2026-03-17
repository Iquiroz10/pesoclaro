namespace PesoClaro.API.DTOs;

public class CrearMovimientoDTO
{
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public int CuentaId { get; set; }
}

public class MovimientoResponseDTO
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string NombreCuenta { get; set; } = string.Empty;
}

public class ResumenMovimientosDTO
{
    public decimal TotalIngresos { get; set; }
    public decimal TotalGastos { get; set; }
    public decimal Diferencia { get; set; }
    public List<MovimientoResponseDTO> Movimientos { get; set; } = new();
    public Dictionary<string, decimal> GastosPorCategoria { get; set; } = new();
}