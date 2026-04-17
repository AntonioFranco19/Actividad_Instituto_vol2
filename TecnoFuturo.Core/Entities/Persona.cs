using MessagePack;
using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;
[MessagePackObject]
[Union(1, typeof(Alumno))]
[Union(2, typeof(Profesor))]
public abstract class Persona : IInfoDetallada
{
    [Key(0)]
    public string Nif { get; set; } = null!;
    [Key(1)]
    public string Nombre { get; set; } = null!;
    [Key(2)]
    public string? Email { get; set; }
    [Key(3)]
    public string? Direccíon { get; set; }
    [Key(4)]
    public string? Telefono { get; set; }
    public virtual string ObtenerFicha()
    {
        return " ";
    }
}