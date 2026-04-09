using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface IProfesorRepository
{
    IReadOnlyList<Profesor> ObtenerProfesores();
    IReadOnlyList<Profesor> ObtenerProfesoresPorCentro(int centroId);
    Profesor? ObtenerProfesorPorNif(string nif);
    Profesor InsertarProfesor(Profesor profesor);
    Profesor ModificarProfesor(Profesor profesor);
    bool BorrarProfesor(string nif);
}