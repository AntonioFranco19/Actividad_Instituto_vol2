namespace TecnoFuturo.Core.Entities;

public class CicloFormativo
{
    public int CentroId { get; set; }
    public string CicloFormativoId { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public Turno Turno { get; set; }
}