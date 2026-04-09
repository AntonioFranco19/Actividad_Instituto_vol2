namespace TecnoFuturo.Core.Entities;

public class Centro
{
    public int CentroId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
}