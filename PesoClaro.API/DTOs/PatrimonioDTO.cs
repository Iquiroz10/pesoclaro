namespace PesoClaro.API.DTOs;

public class PatrimonioDTO
{
    public decimal PatrimonioNeto { get; set; }
    public decimal TotalActivos { get; set; }
    public decimal TotalDeudas { get; set; }
    public decimal TotalLiquidez { get; set; }
    public decimal TotalInversiones { get; set; }
    public decimal TotalAhorros { get; set; }
    public decimal RendimientoMensualTotal { get; set; }
    public decimal CostoMensualDeudas { get; set; }
    public decimal RendimientoNeto { get; set; }
    public List<ConsejoDTO> Consejos { get; set; } = new();
}

public class ConsejoDTO
{
    public string Tipo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public decimal ImpactoMensual { get; set; }
}