namespace PesoClaro.API.DTOs;

public class CrearCuentaDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public decimal TasaAnual { get; set; }
    public string TipoTasa { get; set; } = string.Empty;
    public int? DiaDeCorteMensual { get; set; }
    public string Notas { get; set; } = string.Empty;
}

public class ActualizarSaldoDTO
{
    public decimal NuevoSaldo { get; set; }
}

public class CuentaResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public decimal TasaAnual { get; set; }
    public string TipoTasa { get; set; } = string.Empty;
    public int? DiaDeCorteMensual { get; set; }
    public string Notas { get; set; } = string.Empty;
    public decimal RendimientoAnual { get; set; }
    public decimal RendimientoMensual { get; set; }
    public decimal RendimientoDiario { get; set; }
    public DateTime FechaCreacion { get; set; }
}