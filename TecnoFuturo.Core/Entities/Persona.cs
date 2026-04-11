using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;

public abstract class Persona : IInfoDetallada
{
    public string Nif { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Email { get; set; }
    public string? Direccíon { get; set; }
    public string? Telefono { get; set; }
    public virtual string ObtenerFicha()
    {
        return " ";
    }
}