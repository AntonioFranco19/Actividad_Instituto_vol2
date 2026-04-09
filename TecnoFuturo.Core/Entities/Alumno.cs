namespace TecnoFuturo.Core.Entities;

public class Alumno : Persona
{
    public int CentroId { get; set; }
    public string CicloFormativoId { get; set; } = null!;
}