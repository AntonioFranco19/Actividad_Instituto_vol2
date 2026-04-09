using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.Repositories;

public interface IModuloRepository
{
    IReadOnlyList<Modulo> ObtenerModulos();
    IReadOnlyList<Modulo> ObtenerModulosPorCicloFormativo(string cicloFormativoId);
    IReadOnlyList<Modulo> ObtenerModulosPorProfesor(string profesorNif);
    Modulo? ObtenerModuloPorId(int id);
    Modulo InsertarModulo(Modulo modulo);
    Modulo ModificarModulo(Modulo modulo);
    bool BorrarModulo(int id);
}