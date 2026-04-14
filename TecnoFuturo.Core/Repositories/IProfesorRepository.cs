using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface IProfesorRepository
{
    IReadOnlyList<ProfesorDTO> ObtenerProfesores();
    IReadOnlyList<ProfesorDTO> ObtenerProfesoresPorCentro(int centroId);
    ProfesorDTO? ObtenerProfesorPorNif(string nif);
    ProfesorDTO InsertarProfesor(Profesor profesor);
    ProfesorDTO ModificarProfesor(Profesor profesor);
    bool BorrarProfesor(string nif);
}