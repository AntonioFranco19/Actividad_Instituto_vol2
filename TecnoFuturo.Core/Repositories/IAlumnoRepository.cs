using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface IAlumnoRepository
{
    IReadOnlyList<Alumno> ObtenerAlumnos();
    IReadOnlyList<Alumno> ObtenerAlumnosPorCicloFormativo(string cicloFormativoId);
    IReadOnlyList<Alumno> ObtenerAlumnosPorCentro(int centroId);
    Alumno? ObtenerAlumnoPorNif(string nif);
    Alumno InsertarAlumno(Alumno alumno);
    Alumno ModificarAlumno(Alumno alumno);
    bool BorrarAlumno(string nif);
}