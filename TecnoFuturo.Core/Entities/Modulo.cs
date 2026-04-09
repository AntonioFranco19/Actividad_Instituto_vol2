namespace TecnoFuturo.Core.Entities;

public class Modulo
{
    public string CicloFormativoId { get; set; } = null!;
    public int ModuloId { get; set; }
    public string? Nombre { get; set; }
    public int Horas { get; set; }
    public string? ProfesorNif { get; set; }
}