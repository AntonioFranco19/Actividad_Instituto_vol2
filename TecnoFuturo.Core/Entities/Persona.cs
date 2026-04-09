namespace TecnoFuturo.Core.Entities;

public abstract class Persona
{
    public string Nif { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Email { get; set; }
    public string? Direccíon { get; set; }
    public string? Telefono { get; set; }
}